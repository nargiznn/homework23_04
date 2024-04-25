using System;
using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
	public class Genre:BaseEntity
	{
        [MaxLength(20)]
        [MinLength(3)]
        [Required]
        public string Name { get; set; }
        public List<Book> Books { get; set; }

    }
}

