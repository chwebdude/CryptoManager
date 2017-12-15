using System;
using System.ComponentModel.DataAnnotations;
using Model.Enums;

namespace Model.DbModels
{
    public class CryptoTransaction
    {
        [Key]
        public Guid Id { get; set; }

        public TransactionType Type { get; set; }
        public DateTime DateTime { get; set; }

        public decimal InAmount { get; set; }
        public string InCurrency { get; set; }
        public string InAdress { get; set; }

        public decimal OutAmount { get; set; }
        public string OutCurrency { get; set; }
        public string OutAdress { get; set; }
        
        public decimal FeeAmount { get; set; }
        public string FeeCurrency{ get; set; }

        public decimal BuyAmount { get; set; }
        public string BuyCurrency { get; set; }

        public decimal SellAmount { get; set; }
        public string SellCurrency { get; set; }


        public Guid ExchangeId { get; set; }
        public string Comment { get; set; }
        public string TransactionKey { get; set; }
    }
}
