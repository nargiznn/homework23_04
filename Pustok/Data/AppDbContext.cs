using System;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;

namespace Pustok.Data
{
	public class AppDbContext:DbContext
	{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Slider> Sliders { get; set; }

    }
}

