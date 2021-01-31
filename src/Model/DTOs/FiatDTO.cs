using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTOs
{
    public class FiatDTO
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public decimal Invested { get; set; }
        public decimal Payout { get; set; }
        public string ExchangeName { get; set; }
        public Guid ExchangeId { get; set; }

    }
}
