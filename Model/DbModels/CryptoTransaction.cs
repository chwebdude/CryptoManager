using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DbModels
{
    public class CryptoTransaction
    {
        [Key]
        public Guid Id { get; set; }
    }
}
