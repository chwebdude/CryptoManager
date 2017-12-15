using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Model.Enums;

namespace Model.DbModels
{
    public class Setting
    {
        [Key]        
        public Settings Key { get; set; }
        public string Value { get; set; }
    }
}
