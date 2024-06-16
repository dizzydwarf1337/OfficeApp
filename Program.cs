using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using SmartItApp.Models;




var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SmartItAppContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<Employee, IdentityRole<int>>(options => options.SignIn.RequireConfirmedAccount = true).
        AddEntityFrameworkStores<SmartItAppContext>()
        .AddDefaultTokenProviders()
        .AddDefaultUI();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Supervisor", policy => policy.RequireRole("Supervisor"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Employee", policy => policy.RequireRole("Employee"));
    options.AddPolicy("PM", policy => policy.RequireRole("PM"));
    options.AddPolicy("HR", policy => policy.RequireRole("HR"));

});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var roles = new[] { "Supervisor","Admin", "Employee", "HR", "PM" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();



app.Run();