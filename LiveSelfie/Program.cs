using LiveSelfie.BAL;
using LiveSelfie.Common;
using LiveSelfie.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILiveSelfieDAL, LiveSelfieDAL>();
builder.Services.AddScoped<ILiveSelfieBAL, LiveSelfieBAL>();
builder.Services.AddScoped<ICommonFun, CommonFun>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365); // 1 year
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LiveSelfie}/{action=Index}/{id?}");

app.Run();
