using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public partial class Setting
    {
        [Key]
        public string Key { get; set; }
        [MaxLength(500)]
        public string Value { get; set; }
    }
}
