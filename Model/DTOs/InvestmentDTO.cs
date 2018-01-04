using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTOs
{
    public class InvestmentDTO
    {
        public Guid Id { get; private set; }

        public DateTime DateTime { get; private set; }

        public decimal FeeAmount { get; private set; }
        public string FeeCurrency { get; private set; }

        public decimal BuyAmount { get; private set; }
        public string BuyCurrency { get; private set; }
        public decimal BuyFiatRate { get; set; }
        public decimal BuyFiatAmount { get; set; }
        public decimal SellAmount { get; private set; }
        public string SellCurrency { get; private set; }
        public decimal Rate { get; private set; }

        public decimal CurrentFiatValue { get; set; }
        public decimal CurrentFiatRate { get; set; }

        public Guid ExchangeId { get; private set; }
        public string ExchangeName { get; set; }

    }
}
