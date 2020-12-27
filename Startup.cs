using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneSignal.Services;
using Telegram.Bot;

namespace OneSignal
{
    public class Startup
    {
       
        public readonly string BotToken = "1210081882:AAEQNMiMjkup0ZL_SNHNbrV11GM6D2Y3EVk";
        public  IServiceProvider _serviceProvider;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ITelegramBotService>(provider => new TelegramBotService(this.BotToken));
            services.AddSingleton<IBotService>(provider => new BotService(provider.GetRequiredService<ITelegramBotService>()));
            _serviceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var bot = _serviceProvider.GetRequiredService<ITelegramBotService>().CreateTelegramServiceInstance();
            var botService = _serviceProvider.GetRequiredService<IBotService>();
            bot.OnMessage += botService.BotOnMessageReceived;
            bot.OnMessageEdited += botService.BotOnMessageReceived;
            bot.OnReceiveError += botService.BotOnReceiveError;
            bot.StartReceiving();

        }
    }
}


