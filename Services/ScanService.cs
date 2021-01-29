using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneSignal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OneSignal
{
    public class ScanService : IScanService
    {
        List<Task> tasks;
        private static readonly HttpClient client = new HttpClient();
        private readonly IBotService botService;
        private Dictionary<string, DateTime> MessageList;

        public ScanService(IBotService botService)
        {
            this.botService = botService;
            MessageList = new Dictionary<string, DateTime>();
        }

        public async Task StartUp()
        {
            tasks = new List<Task>();

            List<string> coinList = Enum.GetNames(typeof(Coins)).ToList();

            var takeCount = coinList.Count / 5;

            List<IEnumerable<string>> listOfPartition = new List<IEnumerable<string>>();
            for (int i = 0; i < coinList.Count(); i += takeCount)
            {
                listOfPartition.Add(coinList.Skip(i).Take(takeCount));
            }

            foreach (var item in listOfPartition)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await GetOrderBook(item);
                }));

            }
            await Task.WhenAll(tasks);


        }


        public async Task GetOrderBook(IEnumerable<string> coinList)
        {

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            //var timer = new System.Threading.Timer((e) =>
            //{
            //    botService.SendTextMessage("Test message", 1232817668);

            //}, null, startTimeSpan, periodTimeSpan);

            foreach (var item in coinList)
            {

                StringBuilder url = new StringBuilder();
                url.Append("https://api.binance.com/api/v1/depth?symbol=");
                url.Append(item);
                url.Append("BTC&limit=100");

                HttpResponseMessage response = await client.GetAsync(url.ToString());
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var jo = JObject.Parse(responseBody);
                var bids = jo.Properties().Children().FirstOrDefault().Root["bids"];
                var asks = jo.Properties().Children().FirstOrDefault().Root["asks"];


                double price = 0;
                double amount = 0;
                List<string> tradeDecisions = new List<string>();
                int totalAsks = 0;
                foreach (var a in asks)
                {
                    if (a.Next != null)
                    {
                        price = Convert.ToDouble(a.Next[0].ToString());
                        amount = Convert.ToDouble(a.Next[1].ToString());
                        double total = price * amount;
                        int rounded = Convert.ToInt32(Math.Ceiling(total));
                        totalAsks += rounded;
                    }
                }

                int totalBids = 0;
                foreach (var a in bids)
                {
                    if (a.Next != null)
                    {
                        price = Convert.ToDouble(a.Next[0].ToString());
                        amount = Convert.ToDouble(a.Next[1].ToString());
                        double total = price * amount;
                        int rounded = Convert.ToInt32(Math.Ceiling(total));
                        totalBids += rounded;
                    }
                }

                int allAmount = totalAsks + totalBids;
                int askRate = 100 * totalAsks / allAmount;
                int bidRate = 100 - askRate;

             
                if ((bidRate > askRate) && (bidRate >= 55)) {
                    string message = string.Empty;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("BUY signal in");
                    sb.Append(" " + item);
                    message = sb.ToString();


                    if (!string.IsNullOrEmpty(message))
                    {

                        if (!MessageList.ContainsKey(item))
                        {
                            botService.SendTextMessage(message, 1232817668);
                            MessageList.Add(message, DateTime.Now);
                        }
                        else
                        {

                            foreach (KeyValuePair<string, DateTime> dict in MessageList)
                            {
                                if (dict.Key == item)
                                {
                                    TimeSpan ts = dict.Value - DateTime.Now;
                                    if (ts.TotalMinutes > 30)
                                    {
                                        botService.SendTextMessage(message, 1232817668);
                                        MessageList.Add(message, DateTime.Now);
                                    }
                                }
                            }
                        }

                    }
                }


                if ((askRate > bidRate) && (askRate >= 55))
                {
                    string message = string.Empty;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELL signal in");
                    sb.Append(" " + item);
                    message = sb.ToString();

                    if (!string.IsNullOrEmpty(message))
                    {

                        if (!MessageList.ContainsKey(item))
                        {
                            botService.SendTextMessage(message, 1232817668);
                            MessageList.Add(message, DateTime.Now);
                        }
                        else {

                            foreach (KeyValuePair<string, DateTime> dict in MessageList)
                            {
                                if (dict.Key == item)
                                {
                                    TimeSpan ts = dict.Value - DateTime.Now;
                                    if (ts.TotalMinutes > 30)
                                    {
                                        botService.SendTextMessage(message, 1232817668);
                                        MessageList.Add(message, DateTime.Now);
                                    }
                                }
                            }
                        }

                       
                    }
                }

            }

        }

    }
}
