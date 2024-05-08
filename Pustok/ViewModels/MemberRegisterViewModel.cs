using System;
using System.ComponentModel.DataAnnotations;

namespace Pustok.ViewModels
{
	public class MemberRegisterViewModel
	{
		[MaxLength(30)]
		[MinLength(5)]
		[Required]
		public string UserName { get; set; }
        [MaxLength(30)]
        [MinLength(8)]
        [Required]
		[EmailAddress]
        public string Email { get; set; }
        [MaxLength(30)]
        [MinLength(8)]
        [Required]
        public string FullName { get; set; }
        [MaxLength(30)]
        [MinLength(5)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [MaxLength(30)]
        [MinLength(5)]
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

	}
}

