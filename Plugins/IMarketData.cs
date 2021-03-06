﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Meta;

namespace Plugins
{
    public interface IMarketData
    {
        Task<decimal> GetCurrentRate(string baseCurrency, string currency);
        //Task<decimal> GetCurrentRate(string baseCurrency, IEnumerable<string> currencies);
        Task<decimal> GetHistoricRate(string baseCurrency, string currency, DateTime time);
        Task<CoinMeta> GetCoinInfo(string symbol);
        bool IsFiat(string symbol);
    }
}
