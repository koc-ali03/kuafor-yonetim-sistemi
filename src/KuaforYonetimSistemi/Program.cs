using KuaforYonetimSistemi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

static async Task SeedAdminAccount(IServiceProvider serviceProvider)
{
    // Varsayýlan admin hesabý bilgileri
    string adminEmail = "g221210059@sakarya.edu.tr";
    string adminPassword = "sau";
    string adminRoleName = "Admin";

    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Admin rolunü oluþtur
    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        var role = new IdentityRole(adminRoleName);
        await roleManager.CreateAsync(role);
    }

    // Admin oluþturulmuþ mu kontrol et
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        // Yoksa oluþtur
        var user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, adminPassword);
        if (result.Succeeded)
        {
            // Kullanýcýya admin rolünü ver
            await userManager.AddToRoleAsync(user, adminRoleName);
            Console.WriteLine("Admin baþarýyla oluþturuldu.");
        }
        else
        {
            Console.WriteLine("Hata, admin kullanýcýsý oluþturulamadý:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }
        }
    }
    else
    {
        Console.WriteLine("Admin kullanicisi zaten kayitli.");
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection bulunamadý.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedAdminAccount(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Hata, veritabanýna baþlangýç verisi eklenemedi");
        Console.WriteLine(ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
