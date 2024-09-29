using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramModule;

public partial class TelegramBotInstance
{
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
#pragma warning disable CS8604  // possible null condition is already handled in the switch statement  
        switch (update.Type)
        {
            case UpdateType.Message:
                await HandleMessageAsync(botClient, update, cancellationToken);
                break;
            case UpdateType.CallbackQuery:
                await HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken);
                break;
            default:
                // Do not process other update types.
                break;
        }
#pragma warning restore CS8604
    }

    private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery updateCallbackQuery, CancellationToken cancellationToken)
    {
        try
        {
            switch (updateCallbackQuery.Data)
            {
                case "start":
                    await botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                        url: "https://t.me/AnyGGBot/testGame", cancellationToken: cancellationToken);
                    break;
                case "game":
                    await botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                        url: "https://t.me/AnyGGBot/testGame", cancellationToken: cancellationToken);
                    break;
                default:
                    await botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                        cancellationToken: cancellationToken);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await botClient.AnswerCallbackQueryAsync(updateCallbackQuery.Id,
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        switch (update.Message.Type)
        {
            case MessageType.Text:
                await HandleTextMessageAsync(botClient, update, cancellationToken);
                break;
        }
    }
    
    private async Task HandleTextMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        if (message.From != null && !_users.ContainsKey(message.From.Id))
        {
            _users.Add(chatId, message.From);
            _logger.LogInformation($"Meet a new user: {message.From.FirstName} {message.From.LastName}");
        }
        
        _logger.LogInformation($"Received a '{messageText}' message in chat {chatId}.");
        switch (messageText)
        {
            case "/start":
                await ReplyToCommandStart(botClient, chatId, update, cancellationToken);
                break;
            case "/game":
                await botClient.SendGameAsync(chatId, _inlineQueryResults[0].Id, cancellationToken: cancellationToken);
                break;
            case "/buy":
                await botClient.SendInvoiceAsync(chatId, "Test Product", "Test Description", "aaa", "", "XTR",
                    new List<LabeledPrice> { new LabeledPrice("Test Product", 100) },
                    cancellationToken: cancellationToken);
                return;
            case "/buylink":
                string buylink = await botClient.CreateInvoiceLinkAsync("buy test", "just for buy test", "aaa", "", "XTR", new List<LabeledPrice> { new LabeledPrice("Test Product", 100) },
                    cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(chatId, buylink, cancellationToken: cancellationToken);
                return;
            default:
                break;
        }
    }
    
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation(ErrorMessage);
        return Task.CompletedTask;
    }

    private string StartReplyImageId = "";
    private Task ReplyToCommandStart(ITelegramBotClient botClient, ChatId chatId, Update update, CancellationToken cancellationToken)
    {
        InviteInlineKeyboardButton.Url = string.Format(InviteInlineKeyboardButtonQuery, update.Message.From.Id);
        if (String.IsNullOrEmpty(StartReplyImageId))
        {
            return botClient.SendPhotoAsync(
                chatId: chatId,
                photo: new InputFileUrl(_config.StartReplyImage),
                caption: _config.StartReplyText,
                cancellationToken: cancellationToken,
                replyMarkup: _startMarkup);
        }
        else
        {
            return botClient.SendPhotoAsync(
                chatId: chatId,
                photo: new InputFileId(StartReplyImageId),
                caption: _config.StartReplyText,
                cancellationToken: cancellationToken,
                replyMarkup: _startMarkup);
        }
    }
}
