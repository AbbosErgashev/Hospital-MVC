using Hospital.Repositories;
using Hospital.Repositories.Implementation;
using Hospital.Repositories.Interfaces;
using Hospital.Services;
using Hospital.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IHospitalInfo, HospitalInfoService>();
builder.Services.AddTransient<IRoomService, RoomService>();
builder.Services.AddTransient<IContactService, ContactService>();
builder.Services.AddTransient<IApplicationUserService, ApplicationUserService>();
builder.Services.AddTransient<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

DataSedding(app);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Home}/{controller=Home}/{action=Index}");

app.Run();

void DataSedding(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}