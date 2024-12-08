using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RecipeApplication;
using RecipeApplication.Authorization;
using RecipeApplication.Data;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		var connString = Configuration.GetConnectionString("DefaultConnection");

		services.AddDbContext<AppDbContext>(options => options.UseSqlite(connString!));
		services.AddDefaultIdentity<ApplicationUser>(options =>
				options.SignIn.RequireConfirmedAccount = true)
		   .AddEntityFrameworkStores<AppDbContext>();

		services.AddScoped<RecipeService>();
		services.AddRazorPages();

		services.AddScoped<IAuthorizationHandler, IsRecipeOwnerHandler>();
		services.AddAuthorizationBuilder()
		   .AddPolicy("CanManageRecipe", p => p.AddRequirements(new IsRecipeOwnerRequirement()));
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapRazorPages();
			endpoints.MapControllers();
		});
	}
}