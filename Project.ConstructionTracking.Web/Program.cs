using Microsoft.EntityFrameworkCore;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Repositories;
using Project.ConstructionTracking.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ContructionTrackingDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ContructionTrackingStrings")));


builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
//scope
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUnitRepo, UnitRepo>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRepo, ProjectRepo>();

builder.Services.AddScoped<ITrackingService, TrackingService>();
builder.Services.AddScoped<ITrackingRepo, TrackingRepo>();

builder.Services.AddScoped<IProjectFormService, ProjectFormService>();
builder.Services.AddScoped<IProjectFormRepo, ProjectFormRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    pattern: "{controller=Unit}/{action=Index}/{id?}");

app.Run();
