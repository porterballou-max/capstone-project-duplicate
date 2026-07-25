using Capstoners2026.Web.Data;
using Capstoners2026.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GrantService>();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Seed data
// Defines the admin role and creates an admin user 
// Also adds Committee role 
// Also adds Committee Chair role 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRole = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    var adminEmail = "admin@4760weber.edu";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "AdminPassword999%");
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }

    // Committee Role setup 
    const string committeeRole = "Committee";
    if (!await roleManager.RoleExistsAsync(committeeRole))
    {
        await roleManager.CreateAsync(new IdentityRole(committeeRole));
    }

    // Committee Chair role setup
    const string committeeChairRole = "CommitteeChair";
    if (!await roleManager.RoleExistsAsync(committeeChairRole))
    {
        await roleManager.CreateAsync(new IdentityRole(committeeChairRole)); 
    }

}

app.Use(async (context, next) =>
{
    if (!context.User.Identity!.IsAuthenticated &&
        context.Request.Path == "/")
    {
        context.Response.Redirect("/Account/Login");
        return;
    }
    await next();
});

app.UseRouting();
app.UseAuthorization();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
