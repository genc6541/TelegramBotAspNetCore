using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneSignal.Services;
using Serilog;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace OneSignal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StartController : ControllerBase
    {
        
        private readonly ILogger<MarketController> _logger;
        private readonly IMultiTaskService multiTaskService;
        private readonly IBotService botService;
        private readonly IScanService scanService;

        public StartController(ILogger<MarketController> logger, IMultiTaskService multiTaskService, IBotService botService, IScanService scanService)
        {
            _logger = logger;
            this.multiTaskService = multiTaskService;
            this.botService = botService;
            this.scanService = scanService;
         }

        [HttpGet]
        public string  Get()
        {
            botService.SendTextMessage("All services starting!", 1232817668);
            MarketController marketController = new MarketController(_logger, multiTaskService, botService);
            CoinController coinController = new CoinController(_logger, scanService, botService);
            

            marketController.Get();
            coinController.Get();


            return "All services started!";

        }
    }
}
