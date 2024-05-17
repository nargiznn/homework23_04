﻿using System;
using Pustok.Models;

namespace Pustok.ViewModels
{
	public class BookDetailViewModel
	{
		public Book Book { get; set; }
		public List<Book> RelatedBooks { get; set; }
		public int TotalReviewsCount { get; set; }
		public bool HasUserReview { get; set; }
		public BookReview Review { get; set; }
		public int AvgRate { get; set; }

	}
}

