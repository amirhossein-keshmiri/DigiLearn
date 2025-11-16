using Common.Application;
using DigiLearn.Api.Infrastructure;
using DigiLearn.Api.Infrastructure.JwtUtil;
using DigiLearn.Api.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Shop.Api.Infrastructure;
using TicketModule;
using UserModule.Core;
using System.Text.Json.Serialization;
using CoreModule.Config;
using Common.Application.FileUtil.Interfaces;
using Common.Application.FileUtil.Services;
using Microsoft.Extensions.Options;
using DigiLearn.Api.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILocalFileService, LocalFileService>();
builder.Services.AddScoped<IFtpFileService, FtpFileService>();

// Register the TeacherApiActionFilter as a scoped service
builder.Services.AddScoped<TeacherApiActionFilter>();

// Add services to the container.

builder.Services.AddControllers()
   .AddJsonOptions(options =>
   {
     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
   })
    .ConfigureApiBehaviorOptions(option =>
    {
      option.InvalidModelStateResponseFactory = (context =>
      {
        var result = new ApiResult()
        {
          IsSuccess = false,
          MetaData = new()
          {
            AppStatusCode = AppStatusCode.BadRequest,
            Message = ModelStateUtil.GetModelStateErrors(context.ModelState)
          }
        };
        return new BadRequestObjectResult(result);
      });
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
  option.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "DigiLearn API",
    Version = "v1",
    Description = "API for DigiLearn Management",
    Contact = new OpenApiContact
    {
      Name = "DigiLearn Support",
      Email = "support@digilearn.com"
    }
  });

  // Include XML comments for better documentation
  var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  if (File.Exists(xmlPath))
  {
    option.IncludeXmlComments(xmlPath);
  }

  option.SchemaFilter<EnumSchemaFilter>();
  var jwtSecurityScheme = new OpenApiSecurityScheme
  {
    Scheme = "bearer",
    BearerFormat = "JWT",
    Name = "JWT Authentication",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Description = "Enter Token",

    Reference = new OpenApiReference
    {
      Id = JwtBearerDefaults.AuthenticationScheme,
      Type = ReferenceType.SecurityScheme
    }
  };

  option.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

  option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

  // Support for file uploads in Swagger UI
  option.OperationFilter<SwaggerFileOperationFilter>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.RegisterApiDependency(builder.Configuration);
CommonBootstrapper.RegisterCommonApplication(builder.Services);
builder.Services.InitUserModule(builder.Configuration);
builder.Services.InitTicketModule(builder.Configuration);
builder.Services.InitCoreModule(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add CORS configuration
builder.Services.AddCors(options =>
{
  options.AddPolicy("ReactAppPolicy", corsBuilder =>
  {
    var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ??
                        new[] { "http://localhost:3001" };

    corsBuilder.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "DigiLearn API v1");
    options.RoutePrefix = "swagger"; // Access Swagger UI at /swagger
  });
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

// Enable CORS with the specified policy
app.UseCors("ReactAppPolicy");

app.Use(async (context, next) =>
{
  await next();
  var status = context.Response.StatusCode;
  if (status == 401)
  {
    //var path = context.Request.Path;
    //context.Response.Redirect($"/auth/login?redirectTo={path}");
  }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseApiCustomExceptionHandler();
app.MapControllers();

app.Run();
