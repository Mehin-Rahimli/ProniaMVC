using Microsoft.EntityFrameworkCore;
using ProniaMVC.Controllers;
using ProniaMVC.DAL;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer("server=DESKTOP-M68M69T\\SQLEXPRESS;database=ProniaBP217DB;trusted_connection = true;integrated security=true;TrustServerCertificate=true;")
);


var app = builder.Build();
app.UseStaticFiles();

app.MapControllerRoute(

    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();

