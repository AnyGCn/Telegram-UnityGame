using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Web;
using System.Security.Cryptography;
using System.Text;

public interface IValidateTokenService
{
    bool ValidateToken(string token, ILogger logger, out NameValueCollection? queryParams);
}

public class ValidateTelegramTokenService : IValidateTokenService
{
    private readonly string _botToken;
    private readonly HMACSHA256 _privateKey;
    
    public ValidateTelegramTokenService(string botToken)
    {
        _botToken = botToken;
        _privateKey = GetPrivateKey(botToken);
    }
    
    public bool ValidateToken(string encodeUrl, ILogger logger, out NameValueCollection? queryParams)
    {
        string decodeUrl = WebUtility.UrlDecode(encodeUrl);
        queryParams = HttpUtility.ParseQueryString(decodeUrl);
        List<string> dataCheckArray = new List<string>();
        string? hash = queryParams["hash"];
        if (hash == null)
        {
            logger.LogInformation("No hash in query parameters.");
            return false;
        }
        
        foreach (string? key in queryParams.AllKeys)
        {
            if (key != null && key != "hash")
            {
                dataCheckArray.Add($"{key}={queryParams[key]??String.Empty}");
            }
        }

        dataCheckArray.Sort();
        string dataCheckString = string.Join('\n', dataCheckArray);
        logger.LogInformation(dataCheckString);
        string computeHash = ByteArrayToHexViaLookup32UnsafeDirect(_privateKey.ComputeHash(new System.Text.ASCIIEncoding().GetBytes(dataCheckString))).Replace("-","");
        logger.LogInformation("Compare Hash: " + hash);
        logger.LogInformation("Compute hash: " + computeHash);
        if (computeHash != hash)
        {
            return false;
        }
        
        string? authDate = queryParams["auth_date"];
        if (authDate == null)
        {
            logger.LogInformation("No auth_date in query parameters.");
            return false;
        }
        
        // 30分钟内有效
        if (DateTime.Now > DateTimeOffset.FromUnixTimeSeconds(long.Parse(authDate)).AddMinutes(30))
        {
            return false;
        }
        
        return true;
    }
    
    private static readonly uint[] _lookup32Unsafe = CreateLookup32Unsafe();
    private static readonly unsafe uint* _lookup32UnsafeP = (uint*)GCHandle.Alloc(_lookup32Unsafe,GCHandleType.Pinned).AddrOfPinnedObject();

    private static uint[] CreateLookup32Unsafe()
    {
        var result = new uint[256];
        for (int i = 0; i < 256; i++)
        {
            string s=i.ToString("x2");
            if(BitConverter.IsLittleEndian)
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            else
                result[i] = ((uint)s[1]) + ((uint)s[0] << 16);
        }
        return result;
    }
    
    private static unsafe string ByteArrayToHexViaLookup32UnsafeDirect(byte[] bytes)
    {
        var lookupP = _lookup32UnsafeP;
        var result = new string((char)0, bytes.Length * 2);
        fixed (byte* bytesP = bytes)
        fixed (char* resultP = result)
        {
            uint* resultP2 = (uint*)resultP;
            for (int i = 0; i < bytes.Length; i++)
            {
                resultP2[i] = lookupP[bytesP[i]];
            }
        }
        return result;
    }
    
    private static HMACSHA256 GetPrivateKey(string botToken)
    {
        var encoding = new System.Text.ASCIIEncoding();
        byte[] botTokenBytes = encoding.GetBytes(botToken);
        var key = new HMACSHA256(encoding.GetBytes("WebAppData"));
        return new HMACSHA256(key.ComputeHash(botTokenBytes));
    }
    
    public static string CreateToken(string message, string secret)
    {
        secret = secret ?? "";
        var encoding = new System.Text.ASCIIEncoding();
        byte[] keyByte = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(message);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }
    }
}
