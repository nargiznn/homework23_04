using System;
namespace Pustok.Models
{
	public class Slider
	{
		public int Id { get; set; }
		public string? Title1 { get; set; }
        public string? Title2 { get; set; }
        public string? BtnText { get; set; }
        public string? BtnUrl { get; set; }
        public string? Desc { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
    }
}

