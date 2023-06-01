using ChachaPronounce.Common.Models.Enums;
using ChachaPronounce.Common.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ChachaPronounce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public AudioController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet("{type}/{vocabulary}")]
        public async Task<IActionResult> GetAudioFileAsync([FromRoute] PronounceType type, [FromRoute] string vocabulary)
        {
            var stream = await _storageService.GetFileStreamAsync(type, vocabulary);

            if (stream == null)
            {
                return BadRequest("找不到單字");
            }

            return File(stream, "audio/mp4");
        }
    }
}
