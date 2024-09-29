
using Newtonsoft.Json;
using TelegramModule;

namespace HttpsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 在 appsettings.json 中的 TelegramServer:ApiToken 字段填入你的 Telegram Bot Token
            string botToken = builder.Configuration["TelegramServer:ApiToken"]??String.Empty;
            ValidateTelegramTokenService service = new ValidateTelegramTokenService(botToken);
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<IValidateTokenService, ValidateTelegramTokenService>(provider => service);
            HttpServerInstance httpServer = new HttpServerInstance(builder);
            httpServer.RunAsync();
            
            TelegramBotInstance bot = new(TelegramBotConfig.GetConfiguration());
            bot.RunAsync();
            
            Console.WriteLine("type 'exit' to stop the bot.");
            while (Console.ReadLine() != "exit")
            {
                
            }
        }
    }
}
