using Microsoft.EntityFrameworkCore;
using WindowsService;
using WindowsService.Data;

var host = Host.CreateDefaultBuilder(args)
   .ConfigureServices((hostContext, services) =>
	{
		services.AddDbContext<AppDbContext>(options =>
			options.UseSqlite(hostContext.Configuration.GetConnectionString("SqlLiteConnection")));

		services.AddHttpClient<ExchangeRatesClient>();
		services.AddHostedService<ExchangeRatesHostedService>();
	})
   .UseWindowsService()
   .Build();

host.Run();