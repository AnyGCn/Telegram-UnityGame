using System.Net;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramModule;

public class TelegramBotConfig
{
    public string ApiToken { get; set; }
    public List<BotCommand> BotCommands { get; set; }
    public string InviteUrl { get; set; }
    public string InviteText { get; set; }
    public string StartReplyText { get; set; }
    public string StartReplyImage { get; set; }
    public InlineKeyboardMarkup StartMarkup { get; set; }
    
    public static TelegramBotConfig GetConfiguration()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TelegramBot.json");
        using (StreamReader file = System.IO.File.OpenText(filePath))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                TelegramBotConfig? botConfig = serializer.Deserialize<TelegramBotConfig>(reader);

                if (botConfig == null)
                    throw new Exception("TelegramBot.json is not found.");
                
                return botConfig;
                // 使用 botConfig 作为需要的配置对象
            }
        }
    }
}

public partial class TelegramBotInstance
{
    public Func<string, string, Task<string>>? OnRequireLoginUrl;
    private readonly TelegramBotClient _botClient;
    private readonly CancellationTokenSource _cts;
    private readonly ILogger<TelegramBotInstance> _logger;
    private readonly Dictionary<long, User> _users = new();
    private readonly string? _gameBaseUrl;
    private readonly InlineQueryResult[] _inlineQueryResults;
    private readonly TelegramBotConfig _config;

    public TelegramBotInstance(TelegramBotConfig config)
    {
        _config = config;
        _cts = new ();
        _logger = new Logger<TelegramBotInstance>(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));
        _botClient = new TelegramBotClient(_config.ApiToken);
    }
    
    public async void RunAsync()
    {
        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username}");

        InitializeCommandList();
        InitializeStartReply();
        
        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new ()
        {
            AllowedUpdates = [] // receive all update types except ChatMember related updates
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: _cts.Token
        );
    }
    
    public void Stop()
    {
        // Send cancellation request to stop bot
        _cts.Cancel();
    }

    public void InitializeCommandList()
    {
        _botClient.SetMyCommandsAsync(_config.BotCommands);
    }

    private string InviteInlineKeyboardButtonQuery;
    private InlineKeyboardButton InviteInlineKeyboardButton;

    private InlineKeyboardMarkup _startMarkup;
    
    public void InitializeStartReply()
    {
        _startMarkup = _config.StartMarkup;
        InviteInlineKeyboardButtonQuery = $"https://t.me/share/url?url={WebUtility.UrlEncode(_config.InviteUrl)}{{0}}&text={WebUtility.UrlEncode(_config.InviteText)}";
        InviteInlineKeyboardButton = new InlineKeyboardButton("Invite")
        {
            Url = InviteInlineKeyboardButtonQuery
        };

        List<IEnumerable<InlineKeyboardButton>> inlineKeyboardButtons = new List<IEnumerable<InlineKeyboardButton>>(_startMarkup.InlineKeyboard);
        inlineKeyboardButtons.Add(new[] { InviteInlineKeyboardButton });
        _startMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
    }
    
    private static string GetConfiguration(WebApplicationBuilder builder, string key)
    {
        string? result = builder.Configuration.GetValue<string>(key);
        if (String.IsNullOrEmpty(result))
            throw new Exception($"{key} is not set.");

        return result;
    }
}