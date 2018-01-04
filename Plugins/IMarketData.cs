using System;

namespace Plugins
{
    public interface IMarketData
    {
        decimal GetCurrentRate(string baseCurrency, string toCurrency);
        decimal GetHistoricRate(string baseCurrency, string toCurrency, DateTime time);
    }
}
