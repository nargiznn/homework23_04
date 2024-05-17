using System;
namespace Pustok.ViewModels
{
	public class BasketItemViewModel
	{
        public int Id { get; set; }
        public string BookName { get; set; }
        public string BookImage { get; set; }
        public decimal BookPrice { get; set; }
        public int Count { get; set; }
        public int BookId { get; internal set; }
    }
}

