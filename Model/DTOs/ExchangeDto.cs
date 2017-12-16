using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Model.Enums;

namespace Model.DTOs
{
    public class ExchangeDto
    {
        [Key]
        public Guid Id { get; set; }

        public string Comment { get; set; }
        public string ExchangeName { get; set; }
        public Exchange Exchange { get; set; }
    }
}
