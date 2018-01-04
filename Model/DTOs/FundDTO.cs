using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTOs
{
    public class FundDTO
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string ExchangeName { get; set; }
        public Guid ExchangeId { get; set; }
        public decimal WorthFiat { get; set; }
        public decimal CurrentFiatRate { get; set; }
    }
}
