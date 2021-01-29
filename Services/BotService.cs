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
        private readonly ITelegramBotClient bot;
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

        public async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            Coins coins;
            bool isExistCoin = Enum.TryParse(message.Text, out coins);
            if (isExistCoin) {
                CoinService coinService = new CoinService(this);
                Task.Run(async () =>
                {
                    string result100 = await coinService.TradeDesicionSerivice(message.Text, 100);
                    SendTextMessage($"{message.Text} 100 depth trade desicion : {result100}", 1232817668);
                });
                Task.Run(async () =>
                {
                    string result500 = await coinService.TradeDesicionSerivice(message.Text, 500);
                    SendTextMessage($"{message.Text} 500 depth trade desicion : {result500}", 1232817668);

                });

                return;
            }


            switch (message.Text)
                {
                   case  "/start":
                        SendTextMessage("Welcome Bro!", message.Chat.Id);
                        break;
                   case "/healthcheck": 
                        SendTextMessage("Relax my friend, I'm working..", message.Chat.Id);
                        break;
                   case "/target":
                        SendTextMessage("I will make you very rich..", message.Chat.Id);
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
