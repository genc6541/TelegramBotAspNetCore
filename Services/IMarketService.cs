using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneSignal.Services
{
    public interface IMarketService
    {
         Task GetOrderBook(IEnumerable<string> coinList);
    }
}
