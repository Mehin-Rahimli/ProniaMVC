using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProniaMVC.Controllers;
using ProniaMVC.DAL;
using ProniaMVC.Models;
using ProniaMVC.Services.Implementations;
using ProniaMVC.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddScoped<IHttpContextAccessor, IHttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);


builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 8;
    opt.Password.RequireNonAlphanumeric=false;
    opt.User.RequireUniqueEmail = true;

    opt.Lockout.AllowedForNewUsers = true;
    opt.Lockout.MaxFailedAccessAttempts = 3;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();



builder.Services.AddScoped<ILayoutService,LayoutService>();
var app = builder.Build();



app.UseAuthentication(); //Login olan userin datalarin saxlamaq ucun(mueyyenlesdirmek ucun)
app.UseAuthorization(); //Login olan userin rolunu mueyyenlesdirmek ucun

app.UseStaticFiles();

app.MapControllerRoute(

    "admin",
    "{area:exists}/{controller=home}/{action=index}/{id?}"
    );

app.MapControllerRoute(

    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();

