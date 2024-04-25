using System;
using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
	public class Author:BaseEntity
	{
        [MaxLength(30)]
        [MinLength(3)]
        [Required]
        public string Fullname { get; set; }
    }
}

