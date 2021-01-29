using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneSignal.Services;
using Serilog;

namespace OneSignal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoinController : ControllerBase
    {
        
        private readonly ILogger<MarketController> _logger;
        private readonly IScanService scanService;
        private readonly IBotService botService;

        public CoinController(ILogger<MarketController> logger, IScanService scanService, IBotService botService)
        {
            _logger = logger;
            this.scanService = scanService;
            this.botService = botService;
        }

        [HttpGet]
        public async Task  Get()
        {
            botService.SendTextMessage("Single coin signal scanning started!", 1232817668);

            while (true)
            {
                await scanService.StartUp();
                Thread.Sleep(30000);
            }

        }
    }
}
