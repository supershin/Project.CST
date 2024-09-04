using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Repositories;
using Project.ConstructionTracking.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ContructionTrackingDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ContructionTrackingStrings")));

// Add Config appsetting.json
builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

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

builder.Services.AddScoped<IFormOverallService, FormOverallService>();
builder.Services.AddScoped<IFormOverallRepo, FormOverallRepo>();

builder.Services.AddScoped<IGetDDLService, GetDDLService>();
builder.Services.AddScoped<IGetDDLRepo, GetDDLRepo>();

builder.Services.AddScoped<ISummeryUnitFormService, SummeryUnitFormService>();
builder.Services.AddScoped<ISummeryUnitFormRepo, SummeryUnitFormRepo>();

builder.Services.AddScoped<IFormGroupService, FormGroupService>();
builder.Services.AddScoped<IFormGroupRepo, FormGroupRepo>();

builder.Services.AddScoped<IFormChecklistService, FormChecklistService>();
builder.Services.AddScoped<IFormChecklistRepo, FormChecklistRepo>();

builder.Services.AddScoped<IMasterProjectService, MasterProjectService>();
builder.Services.AddScoped<IMasterProjectRepo, MasterProjectRepo>();

builder.Services.AddScoped<IPMApproveService, PMApproveService>();
builder.Services.AddScoped<IPMApproveRepo, PMApproveRepo>();

builder.Services.AddScoped<IMasterFormService, MasterFormService>();
builder.Services.AddScoped<IMasterFormRepo, MasterFormRepo>();

builder.Services.AddScoped<IPJMApproveService, PJMApproveService>();
builder.Services.AddScoped<IPJMApproveRepo, PJMApproveRepo>();

builder.Services.AddScoped<IMasterCompanyService, MasterCompanyService>();
builder.Services.AddScoped<IMasterCompanyRepo, MasterCompanyRepo>();

builder.Services.AddScoped<IMasterUnitService, MasterUnitService>();
builder.Services.AddScoped<IMasterUnitRepo, MasterUnitRepo>();

builder.Services.AddScoped<IUnLockPassConditionService, UnLockPassConditionService>();
builder.Services.AddScoped<IUnLockPassConditionRepo, UnLockPassConditionRepo>();

builder.Services.AddScoped<IMasterUserService, MasterUserService>();
builder.Services.AddScoped<IMasterUserRepo, MasterUserRepo>();

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepo, LoginRepo>();

builder.Services.AddScoped<IQcSummaryService, QcSummaryService>();
builder.Services.AddScoped<IQcSummaryRepo, QcSummaryRepo>();

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Upload")),
    RequestPath = "/Upload"
});

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Login}/{action=Index}/{id?}");
pattern: "{controller=Login}/{action=Index}/{id?}");
app.Run();
