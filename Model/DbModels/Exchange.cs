using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.DbModels
{
    public class Exchange
    {
        [Key]
        public Guid Id { get; set; }

        public string Comment { get; set; }
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}