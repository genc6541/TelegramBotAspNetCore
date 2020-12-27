using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;


namespace OneSignal.Services
{

    public class BotService : IBotService
    {
        private readonly ITelegramBotService telegramBotService;
        private ITelegramBotClient bot;
        public BotService(ITelegramBotService telegramBotService)
        {
            this.telegramBotService = telegramBotService;
            this.bot = telegramBotService.CreateTelegramServiceInstance();
        }
        public async void SendTextMessage(string message, long chatId)
        {
            try
            {
                await bot.SendChatActionAsync(chatId, ChatAction.Typing);
                await Task.Delay(500);

                await bot.SendTextMessageAsync(
                    chatId: chatId,
                    text: message
                );
            }
            catch (Exception ex)
            {
                //Some Exception logging process..
            }
        }

        public void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

        
                switch (message.Text)
                {
                   case  "/start": // Welcome and description
                        SendTextMessage("Welcome Bro!", message.Chat.Id);
                        break;
                    default:
                        SendTextMessage("I dont understand your command", message.Chat.Id);
                        break;
                }
            
        }
        public void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }



    }


}
