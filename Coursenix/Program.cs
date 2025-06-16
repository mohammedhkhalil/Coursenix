using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Coursenix.Models;
using Coursenix.Repository;
using System.Security.Claims;
using System.Text.Json;
using System.Reflection.Emit;

var builder = WebApplication.CreateBuilder(args);

// Add MVC support
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EmailService>();
// Add session support
builder.Services.AddSession();

// Register the DbContext with SQL Server
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity services using AppUser instead of IdentityUser
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

// Configure application cookies (for login, logout, access denied redirects)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 0;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session before authentication/authorization
app.UseSession();

// Enable authentication and authorization
app.UseAuthentication(); // This must come before UseAuthorization
app.UseAuthorization();

// Define the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedAdminAsync(services);
    await SeedTeachersAsync(services);
    await SeedStudentsAsync(services);
    var context = services.GetRequiredService<Context>();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Student", "Teacher" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


static async Task SeedAdminAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>(); 
    var context = serviceProvider.GetRequiredService<Context>(); 

    string adminEmail = configuration["AdminUser:Email"];
    string adminPassword = configuration["AdminUser:Password"];
    string adminName = configuration["AdminUser:Name"];

    var existingUser = await userManager.FindByEmailAsync(adminEmail);
    if (existingUser == null)
    {
        var adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Name = adminName,
            EmailConfirmed = true,
            RoleType = "Admin"
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            await userManager.AddToRoleAsync(adminUser,"Admin");
            await userManager.AddClaimsAsync(adminUser, new List<Claim>
            {
                new Claim("FullName", adminUser.Name),
                new Claim("Email", adminUser.Email)
            });
            var adminRecord = new Admin
            {
                AppUserId = adminUser.Id,
                Email = adminEmail
            };

            context.Admins.Add(adminRecord);
            await context.SaveChangesAsync();
            Console.WriteLine("Admin user created and seeded successfully.");
        }
        else
        {
            Console.WriteLine("Admin user creation failed:");
            foreach (var error in result.Errors)
                Console.WriteLine($"- {error.Description}");
        }
    }
    else
    {
        Console.WriteLine("Admin user already exists.");
    }
}
static async Task SeedTeachersAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(); 
    var context = serviceProvider.GetRequiredService<Context>();

    var json = await File.ReadAllTextAsync("TeachersUsers.json");
    var users = JsonSerializer.Deserialize<List<AppUser>>(json);

    foreach (var user in users)
    {
        var existing = await userManager.FindByEmailAsync(user.Email);
        if (existing == null)
        {
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("Teacher"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Teacher"));
                }
                await userManager.AddToRoleAsync(user,"Teacher");
                await userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim("FullName", user.Name),
                    new Claim("Email", user.Email)
                });

                context.Teachers.Add(new Teacher
                {
                    AppUserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                });
                await context.SaveChangesAsync();
            }
        }
    }
}

static async Task SeedStudentsAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var context = serviceProvider.GetRequiredService<Context>();

    var json = await File.ReadAllTextAsync("StudentsUsers.json");
    var users = JsonSerializer.Deserialize<List<AppUser>>(json);

    foreach (var user in users)
    {
        var existing = await userManager.FindByEmailAsync(user.Email);
        if (existing == null)
        {
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("Student"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Student"));
                }
                await userManager.AddToRoleAsync(user,"Student");
                await userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim("FullName", user.Name),
                    new Claim("Email", user.Email)
                });

                context.Students.Add(new Student
                {
                    AppUserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ParentPhoneNumber = user.PhoneNumber,
                });
                await context.SaveChangesAsync();
            }
        }
    }
}


app.Run();
