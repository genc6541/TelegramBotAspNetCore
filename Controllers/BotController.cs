using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneSignal.Services;
using Telegram.Bot.Types.Enums;

namespace OneSignal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BotController : ControllerBase
    {
        
        private readonly ILogger<MarketController> _logger;
        private readonly IBotService botService;
        public BotController(ILogger<MarketController> logger, IBotService botService)
        {
            _logger = logger;
            this.botService = botService;
        }

        [HttpGet]
        public string Get()
        {

            botService.SendTextMessage("Bu mesaj da benden arkadaş!", 1232817668);
            return "Bot Application Started!";
        }
    }
}
