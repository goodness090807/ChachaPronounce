using ChachaPronounce.Models;
using ChachaPronounce.Params;
using Microsoft.AspNetCore.Mvc;
using ChachaPronounce.Extensions;
using Microsoft.Extensions.Options;
using ChachaPronounce.Producer;
using ChachaPronounce.Common.Services.LineMessage;
using ChachaPronounce.Common.Services.Storage;
using ChachaPronounce.Common.Models.Enums;
using ChachaPronounce.Common.Models.LineMessage;
using System.Text.RegularExpressions;

namespace ChachaPronounce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILineMessageService _lineMessageService;
        private readonly IStorageService _storageService;
        private readonly IMessageProducer _messageproducer;
        private readonly AppSettings _appsettings;

        public WebhookController(ILineMessageService lineMessageService, IStorageService storageService, IMessageProducer messageProducer
            , IOptions<AppSettings> options)
        {
            _lineMessageService = lineMessageService;
            _storageService = storageService;
            _messageproducer = messageProducer;
            _appsettings = options.Value;
        }

        [HttpPost]
        public async Task ReceiveWebhookAsync([FromBody] WebhookEventParam param)
        {
            var replyToken = param.Events[0].ReplyToken;
            var message = param.Events[0].Message.Text;

            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(replyToken)) return;

            if (message == "!Help")
            {
                await _lineMessageService.SendMessage(replyToken, new() { new ReplyContent.TextMessage(
                    "歡迎使用查查單字\r\n\r\n可以透過左下角鍵盤輸入 \"單字\" 就能開始使用。\r\n\r\n也可以在單字後面輸入 空白 + s(美式發音)、k(英式發音)，可以改變發音的口音。\r\n\r\n使用範例如下：\r\nexample  => 預設為美式口音\r\nexample s => 美式口音\r\nexample k => 英式發音\r\n\r\n如果忘記怎麼使用，\r\n可以輸入 !Help 或是透過選單來查看哦。"
                    ) });
                return;
            }

            (string vocabulary, PronounceType? pronounceType) = MessageExtension.GetUrlParamByMessage(message);

            if (string.IsNullOrEmpty(vocabulary))
            {
                await _lineMessageService.SendMessage(replyToken, new() { new ReplyContent.TextMessage("找不到單字") });
                return;
            };

            if (pronounceType == null)
            {
                await _lineMessageService.SendMessage(replyToken,
                    new() { new ReplyContent.TextMessage("找不到發音!!!請輸入 [單字 s] => 美式發音 或 [單字 k] => 英式發音") });
                return;
            };

            if (!Regex.IsMatch(vocabulary, "^[a-zA-Z']+$"))
            {
                await _lineMessageService.SendMessage(replyToken, new() { new ReplyContent.TextMessage("只能輸入A-Z、a-z和上引號(')") });
                return;
            }

            var fileExist = await _storageService.CheckFileExistAsync(pronounceType.Value, vocabulary);

            // 單字不存在，交由MessageQueue處理
            if (!fileExist)
            {
                _messageproducer.SendVocabularyProcessQueue(new(replyToken, vocabulary, pronounceType.Value));
                return;
            }

            var url = _appsettings.GetAudioUrl(pronounceType.Value, vocabulary);
            var result = await _lineMessageService.SendMessage(replyToken, new() { new ReplyContent.AudioMessage(url) });
            if (!result)
            {
                await _lineMessageService.SendMessage(replyToken, new() { new ReplyContent.TextMessage("發生異常") });
            }
        }
    }
}
