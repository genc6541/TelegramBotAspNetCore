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
    public class MarketController : ControllerBase
    {
        
        private readonly ILogger<MarketController> _logger;
        private readonly IMultiTaskService multiTaskService;
        private readonly IBotService botService;

        public MarketController(ILogger<MarketController> logger, IMultiTaskService multiTaskService, IBotService botService)
        {
            _logger = logger;
            this.multiTaskService = multiTaskService;
            this.botService = botService;
        }

        [HttpGet]
        public async Task  Get()
        {
            botService.SendTextMessage("Wall monitor scanning started!", 1232817668);

            while (true) {
                await multiTaskService.StartUp();
                Thread.Sleep(5000);
            }


        }
    }
}
