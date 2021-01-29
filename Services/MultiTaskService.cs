using OneSignal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneSignal
{
    public class MultiTaskService : IMultiTaskService
    {
        List<Task> tasks;
        private readonly IMarketService marketService;

        public MultiTaskService(IMarketService marketService)
        {
            this.marketService = marketService;
        }

        public async Task StartUp() {
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
                    await marketService.GetOrderBook(item);
                }));

            }
            await Task.WhenAll(tasks);


        }

    }
}
