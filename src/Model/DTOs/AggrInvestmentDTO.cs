using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTOs
{
    public class AggrInvestmentDTO
    {
        public IList<InvestmentDTO> Investments { get; set; }
        public Dictionary<string, decimal> TokenProfits { get; set; }
        public decimal TotalWorth { get; set; }
        public decimal TotalTradeInvest { get; set; }
        public decimal Profit { get; set; }
    }
}
