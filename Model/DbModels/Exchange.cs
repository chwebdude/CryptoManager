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

        public string Name { get; set; }

        public bool SupportsPublicKey { get; set; }
        public string PublicKey { get; set; }

        public bool SupportsPrivateKey { get; set; }
        public string PrivateKey { get; set; }
    }
}