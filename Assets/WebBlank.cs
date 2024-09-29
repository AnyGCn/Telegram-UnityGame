using UnityEngine.UI;
using SparkX;
using UnityEngine;

public class WebBlank : Text
{
    public TextAsset textAsset;
    private const string baseInviteUrl = "https://t.me/AnyGGBot/testGame?startapp={0}";

    // Start is called before the first frame update
    void Start()
    {
        text = SparkXSDK.GetTelegramWebAppInitData().ToString();
    }

    public void Invite()
    {
        SparkXSDK.ShareViaTelegram(baseInviteUrl, "Dacongming", "you dan ni jiu lai.🙂");
    }
}
