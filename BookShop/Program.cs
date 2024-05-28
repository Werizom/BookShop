using BookShop.Configuration.Configurations;
using BookShop.Services.Interfaces;
using BookShop.Services;
using BookShop.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddConfigurationDatabase(builder.Configuration);
builder.Services.AddIdentity();
builder.Services.AddConfigurationDI();
builder.Services.AddConfigurationDIForServices();

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "1414758315816899";
    options.AppSecret = "9f2d276d1bee006729c4ff658503f7cd";
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(Options =>
{
    Options.IdleTimeout = TimeSpan.FromMinutes(10);
    Options.Cookie.HttpOnly = true;
    Options.Cookie.IsEssential = true;
});

builder.Services.AddConfigurateIOptions(builder.Configuration);

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
app.UseAuthentication();
app.UseAuthorization();

builder.Services.AddDbInitializer();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();