using Common.Application;
using Common.Application.FileUtil.Interfaces;
using Common.Application.FileUtil.Services;
using CoreModule.Config;
using DigiLearn.Web.Infrastructure;
using DigiLearn.Web.Infrastructure.JwtUtil;
using TicketModule;
using UserModule.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILocalFileService, LocalFileService>();
builder.Services.AddScoped<IFtpFileService, FtpFileService>();

builder.Services.AddTransient<TeacherActionFilter>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services
       .InitUserModule(builder.Configuration)
       .InitTicketModule(builder.Configuration)
       .InitCoreModule(builder.Configuration);

CommonBootstrapper.RegisterCommonApplication(builder.Services);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.Use(async (context, next) =>
{
  var token = context.Request.Cookies["digi-token"]?.ToString();
  if (string.IsNullOrWhiteSpace(token) == false)
  {
    context.Request.Headers.Append("Authorization", $"Bearer {token}");
  }
  await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.Use(async (context, next) =>
{
  await next();
  var status = context.Response.StatusCode;
  if (status == 401)
  {
    var path = context.Request.Path;
    context.Response.Redirect($"/auth/login?redirectTo={path}");
  }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
