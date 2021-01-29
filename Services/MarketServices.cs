using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using Serilog;

namespace OneSignal.Services
{
    public class MarketServices : IMarketService
    {
        private readonly IBotService botService;

        private static readonly HttpClient client = new HttpClient();
        private List<string> MessageList;

        public MarketServices(IBotService botService)
        {
            this.botService = botService;
            MessageList = new List<string>();
        }

        public async Task GetOrderBook(IEnumerable<string> coinList)
        {
            foreach (var item in coinList) {

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
                List<int> totalList = new List<int>();
                int count = 0;
                foreach (var a in asks)
                {
                    
                    if (a.Next != null)
                    {

                        price = Convert.ToDouble(a.Next[0].ToString());
                        amount = Convert.ToDouble(a.Next[1].ToString());
                        double total = price * amount;
                        int rounded = Convert.ToInt32(Math.Ceiling(total));
                        string log = "ask-> "+ rounded + " BTC, price: "+ price+ " ,coin:" + item + ", count: " + count;
                        if (rounded >= 20) {
                            Log.Information(log);
                            string messagge = item + " Sell wall "+ rounded +" BTC at " +a.Next[0].ToString();
                            if (!MessageList.Contains(messagge))
                            {
                                botService.SendTextMessage(messagge, 1232817668);
                                MessageList.Add(messagge);
                            }
                        }
                        count++;
                    }
                }

                int count2 = 0;
                foreach (var b in bids)
                {
                    if (b.Next != null)
                    {

                        price = Convert.ToDouble(b.Next[0].ToString());
                        amount = Convert.ToDouble(b.Next[1].ToString());
                        double total = price * amount;
                        int rounded = Convert.ToInt32(Math.Ceiling(total));
                        string log = "ask-> " + rounded + " BTC, price: " + price + " ,coin:" + item + ", count: " + count2;
                     

                        if (rounded >= 20)
                        {
                            Log.Information(log);
                            string messagge = item + " Buy wall " + rounded + " BTC at " + b.Next[0].ToString();
                            if (!MessageList.Contains(messagge)) {
                                botService.SendTextMessage(messagge, 1232817668);
                                MessageList.Add(messagge);
                            }

                        }
                        count2++;
                    }
                }
            }
       
        }
    }
}
