﻿using Microsoft.EntityFrameworkCore;

namespace QuartzHostedService.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options)
	{
	}

	public DbSet<ExchangeRates> ExchangeRates { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	}
}