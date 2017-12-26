using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.DbModels
{
    public class FiatBalance
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ExchangeId { get; set; }
        public string Currency { get; set; }
        public decimal Invested { get; set; }
        public decimal Payout { get; set; }
    }
}
