using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

#pragma warning disable CS0067,CS8625,CS8618,CS8600,CS0618

namespace WordsBot.Common.Test
{
  public class MockTelegramBotClient : ITelegramBotClient
  {
    public event EventHandler<EventArgs> OnNoMessages;
    private readonly Queue<string[]> _messages;
    public int MessageGroupsLeft => _messages.Count;
    public List<Message> SentMessages { get; set; } = new();

    public MockTelegramBotClient(params string[] messages)
    {
      _messages = new Queue<string[]>(messages.Select(message => message.Split('-').ToArray()));
    }

    public async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
      if (request is GetUpdatesRequest getUpdatesRequest)
      {
        await Task.Delay(10, cancellationToken);

        if (!_messages.TryDequeue(out string[] messages))
        {
          if (OnNoMessages != null)
          {
            OnNoMessages.Invoke(this, EventArgs.Empty);
          }
          return (TResponse)(object)Array.Empty<Update>();
        }

        return (TResponse)(object)messages.Select((message, i) => new Update()
        {
          Message = new Message()
          {
            From = new User { Id = 1234 },
            Chat = new Chat { Id = 1234 },
            Text = messages[i]
          },
          Id = getUpdatesRequest.Offset + i + 1
        }).ToArray();
      }
      else throw new NotImplementedException();
    }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromMilliseconds(50);

    public Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> entities = null, bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      var res = new Message { Text = text };
      SentMessages.Add(res);
      return Task.FromResult(res);
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
      return Task.CompletedTask;
    }

    // ---------------
    // NOT IMPLEMENTED
    // ---------------


    public int BotId => throw new NotImplementedException();
    public bool IsReceiving => throw new NotImplementedException();
    public int MessageOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    long? ITelegramBotClient.BotId => throw new NotImplementedException();

    public event EventHandler<ApiRequestEventArgs> MakingApiRequest;
    public event EventHandler<ApiResponseEventArgs> ApiResponseReceived;
    public event EventHandler<UpdateEventArgs> OnUpdate;
    public event EventHandler<MessageEventArgs> OnMessage;
    public event EventHandler<MessageEventArgs> OnMessageEdited;
    public event EventHandler<InlineQueryEventArgs> OnInlineQuery;
    public event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen;
    public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery;
    public event EventHandler<ReceiveErrorEventArgs> OnReceiveError;
    public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError;
    public event AsyncEventHandler<ApiRequestEventArgs> OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs> OnApiResponseReceived;

    public Task AddStickerToSetAsync(int userId, string name, InputOnlineFile pngSticker, string emojis, MaskPosition maskPosition = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task AnswerCallbackQueryAsync(string callbackQueryId, string text = null, bool showAlert = false, string url = null, int cacheTime = 0, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task AnswerInlineQueryAsync(string inlineQueryId, IEnumerable<InlineQueryResultBase> results, int? cacheTime = null, bool isPersonal = false, string nextOffset = null, string switchPmText = null, string switchPmParameter = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, string errorMessage, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task AnswerShippingQueryAsync(string shippingQueryId, IEnumerable<ShippingOption> shippingOptions, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task AnswerShippingQueryAsync(string shippingQueryId, string errorMessage, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task CreateNewStickerSetAsync(int userId, string name, string title, InputOnlineFile pngSticker, string emojis, bool isMasks = false, MaskPosition maskPosition = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task DeleteChatPhotoAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task DeleteChatStickerSetAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task DeleteMessageAsync(ChatId chatId, int messageId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task DeleteStickerFromSetAsync(string sticker, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task DeleteWebhookAsync(CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Stream> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> EditMessageCaptionAsync(ChatId chatId, int messageId, string caption, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), ParseMode parseMode = ParseMode.Default) => throw new NotImplementedException();
    public Task EditMessageCaptionAsync(string inlineMessageId, string caption, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), ParseMode parseMode = ParseMode.Default) => throw new NotImplementedException();
    public Task<Message> EditMessageLiveLocationAsync(ChatId chatId, int messageId, float latitude, float longitude, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task EditMessageLiveLocationAsync(string inlineMessageId, float latitude, float longitude, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> EditMessageMediaAsync(ChatId chatId, int messageId, InputMediaBase media, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task EditMessageMediaAsync(string inlineMessageId, InputMediaBase media, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> EditMessageReplyMarkupAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task EditMessageReplyMarkupAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> EditMessageTextAsync(ChatId chatId, int messageId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task EditMessageTextAsync(string inlineMessageId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<string> ExportChatInviteLinkAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> ForwardMessageAsync(ChatId chatId, ChatId fromChatId, int messageId, bool disableNotification = false, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<ChatMember[]> GetChatAdministratorsAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<ChatMember> GetChatMemberAsync(ChatId chatId, int userId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<int> GetChatMembersCountAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Telegram.Bot.Types.File> GetFileAsync(string fileId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<GameHighScore[]> GetGameHighScoresAsync(int userId, long chatId, int messageId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<GameHighScore[]> GetGameHighScoresAsync(int userId, string inlineMessageId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Telegram.Bot.Types.File> GetInfoAndDownloadFileAsync(string fileId, Stream destination, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<User> GetMeAsync(CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult((new User()));
    public Task<StickerSet> GetStickerSetAsync(string name, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<UserProfilePhotos> GetUserProfilePhotosAsync(int userId, int offset = 0, int limit = 0, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<WebhookInfo> GetWebhookInfoAsync(CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task KickChatMemberAsync(ChatId chatId, int userId, DateTime untilDate = default(DateTime), CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task LeaveChatAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task PinChatMessageAsync(ChatId chatId, int messageId, bool disableNotification = false, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task PromoteChatMemberAsync(ChatId chatId, int userId, bool? canChangeInfo = null, bool? canPostMessages = null, bool? canEditMessages = null, bool? canDeleteMessages = null, bool? canInviteUsers = null, bool? canRestrictMembers = null, bool? canPinMessages = null, bool? canPromoteMembers = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task RestrictChatMemberAsync(ChatId chatId, int userId, ChatPermissions permissions, DateTime untilDate = new DateTime(), CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Message> SendAnimationAsync(ChatId chatId, InputOnlineFile animation, int duration = 0, int width = 0, int height = 0, InputMedia thumb = null, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendAudioAsync(ChatId chatId, InputOnlineFile audio, string caption = null, ParseMode parseMode = ParseMode.Default, int duration = 0, string performer = null, string title = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), InputMedia thumb = null) => throw new NotImplementedException();
    public Task<Message> SendPollAsync(ChatId chatId, string question, IEnumerable<string> options, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task SendChatActionAsync(ChatId chatId, ChatAction chatAction, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendContactAsync(ChatId chatId, string phoneNumber, string firstName, string lastName = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), string vCard = null) => throw new NotImplementedException();
    public Task<Message> SendDocumentAsync(ChatId chatId, InputOnlineFile document, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), InputMedia thumb = null) => throw new NotImplementedException();
    public Task<Message> SendGameAsync(long chatId, string gameShortName, bool disableNotification = false, int replyToMessageId = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendInvoiceAsync(int chatId, string title, string description, string payload, string providerToken, string startParameter, string currency, IEnumerable<LabeledPrice> prices, string providerData = null, string photoUrl = null, int photoSize = 0, int photoWidth = 0, int photoHeight = 0, bool needName = false, bool needPhoneNumber = false, bool needEmail = false, bool needShippingAddress = false, bool isFlexible = false, bool disableNotification = false, int replyToMessageId = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendLocationAsync(ChatId chatId, float latitude, float longitude, int livePeriod = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message[]> SendMediaGroupAsync(ChatId chatId, IEnumerable<InputMediaBase> media, bool disableNotification = false, int replyToMessageId = 0, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message[]> SendMediaGroupAsync(IEnumerable<IAlbumInputMedia> inputMedia, ChatId chatId, bool disableNotification = false, int replyToMessageId = 0, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendPhotoAsync(ChatId chatId, InputOnlineFile photo, string caption = null, ParseMode parseMode = ParseMode.Default, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendStickerAsync(ChatId chatId, InputOnlineFile sticker, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.Default, bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SendVenueAsync(ChatId chatId, float latitude, float longitude, string title, string address, string foursquareId = null, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), string foursquareType = null) => throw new NotImplementedException();
    public Task<Message> SendVideoAsync(ChatId chatId, InputOnlineFile video, int duration = 0, int width = 0, int height = 0, string caption = null, ParseMode parseMode = ParseMode.Default, bool supportsStreaming = false, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), InputMedia thumb = null) => throw new NotImplementedException();
    public Task<Message> SendVideoNoteAsync(ChatId chatId, InputTelegramFile videoNote, int duration = 0, int length = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken), InputMedia thumb = null) => throw new NotImplementedException();
    public Task<Message> SendVoiceAsync(ChatId chatId, InputOnlineFile voice, string caption = null, ParseMode parseMode = ParseMode.Default, int duration = 0, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task SetChatDescriptionAsync(ChatId chatId, string description = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task SetChatPermissionsAsync(ChatId chatId, ChatPermissions permissions, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task SetChatPhotoAsync(ChatId chatId, InputFileStream photo, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task SetChatStickerSetAsync(ChatId chatId, string stickerSetName, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task SetChatTitleAsync(ChatId chatId, string title, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> SetGameScoreAsync(int userId, int score, long chatId, int messageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task SetGameScoreAsync(int userId, int score, string inlineMessageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task SetStickerPositionInSetAsync(string sticker, int position, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task SetWebhookAsync(string url, InputFileStream certificate = null, int maxConnections = 0, IEnumerable<UpdateType> allowedUpdates = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public void StartReceiving(UpdateType[] allowedUpdates = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Message> StopMessageLiveLocationAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task StopMessageLiveLocationAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Poll> StopPollAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public void StopReceiving() => throw new NotImplementedException();
    public Task<bool> TestApiAsync(CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task UnbanChatMemberAsync(ChatId chatId, int userId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task UnpinChatMessageAsync(ChatId chatId, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Telegram.Bot.Types.File> UploadStickerFileAsync(int userId, InputFileStream pngSticker, CancellationToken cancellationToken = default(CancellationToken)) => throw new NotImplementedException();
    public Task<Update[]> GetUpdatesAsync(int offset = 0, int limit = 0, int timeout = 0, IEnumerable<UpdateType> allowedUpdates = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Message> SendPollAsync(ChatId chatId, string question, IEnumerable<string> options, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, bool? isAnonymous = null, PollType? type = null, bool? allowsMultipleAnswers = null, int? correctOptionId = null, bool? isClosed = null, string explanation = null, ParseMode explanationParseMode = ParseMode.Default, int? openPeriod = null, DateTime? closeDate = null) => throw new NotImplementedException();
    public Task<Message> SendDiceAsync(ChatId chatId, bool disableNotification = false, int replyToMessageId = 0, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, Emoji? emoji = null) => throw new NotImplementedException();
    public Task SetChatAdministratorCustomTitleAsync(ChatId chatId, int userId, string customTitle, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<BotCommand[]> GetMyCommandsAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task SetMyCommandsAsync(IEnumerable<BotCommand> commands, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Message> SendInvoiceAsync(int chatId, string title, string description, string payload, string providerToken, string startParameter, string currency, IEnumerable<LabeledPrice> prices, string providerData = null, string photoUrl = null, int photoSize = 0, int photoWidth = 0, int photoHeight = 0, bool needName = false, bool needPhoneNumber = false, bool needEmail = false, bool needShippingAddress = false, bool isFlexible = false, bool disableNotification = false, int replyToMessageId = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default, bool sendPhoneNumberToProvider = false, bool sendEmailToProvider = false) => throw new NotImplementedException();
    public Task CreateNewAnimatedStickerSetAsync(int userId, string name, string title, InputFileStream tgsSticker, string emojis, bool isMasks = false, MaskPosition maskPosition = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task AddAnimatedStickerToSetAsync(int userId, string name, InputFileStream tgsSticker, string emojis, MaskPosition maskPosition = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task SetStickerSetThumbAsync(string name, int userId, InputOnlineFile thumb = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task SetWebhookAsync(string url, InputFileStream certificate = null, string ipAddress = null, int maxConnections = 0, IEnumerable<UpdateType> allowedUpdates = null, bool dropPendingUpdates = false, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task DeleteWebhookAsync(bool dropPendingUpdates = false, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task LogOutAsync(CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }


    public Task<MessageId> CopyMessageAsync(ChatId chatId, ChatId fromChatId, int messageId, string caption = null, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, int replyToMessageId = 0, bool disableNotification = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendPhotoAsync(ChatId chatId, InputOnlineFile photo, string caption = null, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendAudioAsync(ChatId chatId, InputOnlineFile audio, string caption = null, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, int duration = 0, string performer = null, string title = null, InputMedia thumb = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendDocumentAsync(ChatId chatId, InputOnlineFile document, InputMedia thumb = null, string caption = null, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, bool disableContentTypeDetection = false, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendStickerAsync(ChatId chatId, InputOnlineFile sticker, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendVideoAsync(ChatId chatId, InputOnlineFile video, int duration = 0, int width = 0, int height = 0, InputMedia thumb = null, string caption = null, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, bool supportsStreaming = false, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendAnimationAsync(ChatId chatId, InputOnlineFile animation, int duration = 0, int width = 0, int height = 0, InputMedia thumb = null, string caption = null, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendVoiceAsync(ChatId chatId, InputOnlineFile voice, string caption = null, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, int duration = 0, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendVideoNoteAsync(ChatId chatId, InputTelegramFile videoNote, int duration = 0, int length = 0, InputMedia thumb = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message[]> SendMediaGroupAsync(ChatId chatId, IEnumerable<IAlbumInputMedia> media, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendLocationAsync(ChatId chatId, float latitude, float longitude, int livePeriod = 0, int heading = 0, int proximityAlertRadius = 0, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendVenueAsync(ChatId chatId, float latitude, float longitude, string title, string address, string foursquareId = null, string foursquareType = null, string googlePlaceId = null, string googlePlaceType = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendContactAsync(ChatId chatId, string phoneNumber, string firstName, string lastName = null, string vCard = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendPollAsync(ChatId chatId, string question, IEnumerable<string> options, bool? isAnonymous = null, PollType? type = null, bool? allowsMultipleAnswers = null, int? correctOptionId = null, string explanation = null, ParseMode explanationParseMode = ParseMode.Default, IEnumerable<MessageEntity> explanationEntities = null, int? openPeriod = null, DateTime? closeDate = null, bool? isClosed = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendDiceAsync(ChatId chatId, Emoji? emoji = null, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<UserProfilePhotos> GetUserProfilePhotosAsync(long userId, int offset = 0, int limit = 0, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task KickChatMemberAsync(ChatId chatId, long userId, DateTime untilDate = default, bool? revokeMessages = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task UnbanChatMemberAsync(ChatId chatId, long userId, bool onlyIfBanned = false, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<ChatMember> GetChatMemberAsync(ChatId chatId, long userId, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task RestrictChatMemberAsync(ChatId chatId, long userId, ChatPermissions permissions, DateTime untilDate = default, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task PromoteChatMemberAsync(ChatId chatId, long userId, bool? isAnonymous = null, bool? canManageChat = null, bool? canChangeInfo = null, bool? canPostMessages = null, bool? canEditMessages = null, bool? canDeleteMessages = null, bool? canManageVoiceChats = null, bool? canInviteUsers = null, bool? canRestrictMembers = null, bool? canPinMessages = null, bool? canPromoteMembers = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task SetChatAdministratorCustomTitleAsync(ChatId chatId, long userId, string customTitle, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> EditMessageTextAsync(ChatId chatId, int messageId, string text, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> entities = null, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task EditMessageTextAsync(string inlineMessageId, string text, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> entities = null, bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> EditMessageCaptionAsync(ChatId chatId, int messageId, string caption, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task EditMessageCaptionAsync(string inlineMessageId, string caption, ParseMode parseMode = ParseMode.Default, IEnumerable<MessageEntity> captionEntities = null, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> EditMessageLiveLocationAsync(ChatId chatId, int messageId, float latitude, float longitude, float horizontalAccuracy = 0, int heading = 0, int proximityAlertRadius = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task EditMessageLiveLocationAsync(string inlineMessageId, float latitude, float longitude, float horizontalAccuracy = 0, int heading = 0, int proximityAlertRadius = 0, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendInvoiceAsync(long chatId, string title, string description, string payload, string providerToken, string currency, IEnumerable<LabeledPrice> prices, int maxTipAmount = 0, int[] suggestedTipAmounts = null, string startParameter = null, string providerData = null, string photoUrl = null, int photoSize = 0, int photoWidth = 0, int photoHeight = 0, bool needName = false, bool needPhoneNumber = false, bool needEmail = false, bool needShippingAddress = false, bool sendPhoneNumberToProvider = false, bool sendEmailToProvider = false, bool isFlexible = false, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SendGameAsync(long chatId, string gameShortName, bool disableNotification = false, int replyToMessageId = 0, bool allowSendingWithoutReply = false, InlineKeyboardMarkup replyMarkup = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Message> SetGameScoreAsync(long userId, int score, long chatId, int messageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task SetGameScoreAsync(long userId, int score, string inlineMessageId, bool force = false, bool disableEditMessage = false, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<GameHighScore[]> GetGameHighScoresAsync(long userId, long chatId, int messageId, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<GameHighScore[]> GetGameHighScoresAsync(long userId, string inlineMessageId, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<Telegram.Bot.Types.File> UploadStickerFileAsync(long userId, InputFileStream pngSticker, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task CreateNewStickerSetAsync(long userId, string name, string title, InputOnlineFile pngSticker, string emojis, bool isMasks = false, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task AddStickerToSetAsync(long userId, string name, InputOnlineFile pngSticker, string emojis, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task CreateNewAnimatedStickerSetAsync(long userId, string name, string title, InputFileStream tgsSticker, string emojis, bool isMasks = false, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task AddAnimatedStickerToSetAsync(long userId, string name, InputFileStream tgsSticker, string emojis, MaskPosition maskPosition = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task SetStickerSetThumbAsync(string name, long userId, InputOnlineFile thumb = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task UnpinChatMessageAsync(ChatId chatId, int messageId = 0, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task UnpinAllChatMessages(ChatId chatId, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<ChatInviteLink> CreateChatInviteLinkAsync(ChatId chatId, DateTime? expireDate = null, int? memberLimit = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<ChatInviteLink> EditChatInviteLinkAsync(ChatId chatId, string inviteLink, DateTime? expireDate = null, int? memberLimit = null, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public Task<ChatInviteLink> RevokeChatInviteLinkAsync(ChatId chatId, string inviteLink, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    Task<Telegram.Bot.Types.File> ITelegramBotClient.GetFileAsync(string fileId, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    Task<Telegram.Bot.Types.File> ITelegramBotClient.GetInfoAndDownloadFileAsync(string fileId, Stream destination, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    Task<Telegram.Bot.Types.File> ITelegramBotClient.UploadStickerFileAsync(long userId, InputFileStream pngSticker, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}