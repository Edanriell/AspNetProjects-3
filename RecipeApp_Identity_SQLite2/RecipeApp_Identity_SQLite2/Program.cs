using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RecipeApplication;
using RecipeApplication.Authorization;
using RecipeApplication.Data;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connString!));
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
		options.SignIn.RequireConfirmedAccount = true)
   .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<RecipeService>();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IAuthorizationHandler, IsRecipeOwnerHandler>();
builder.Services.AddAuthorizationBuilder()
   .AddPolicy("CanManageRecipe", policyBuilder =>
		policyBuilder.AddRequirements(new IsRecipeOwnerRequirement()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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

app.MapRazorPages();
app.MapControllers();

app.Run();