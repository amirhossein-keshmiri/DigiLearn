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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILocalFileService, LocalFileService>();
builder.Services.AddScoped<IFtpFileService, FtpFileService>();

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
});

builder.Services.AddHttpContextAccessor();
builder.Services.RegisterApiDependency(builder.Configuration);
CommonBootstrapper.RegisterCommonApplication(builder.Services);
builder.Services.InitUserModule(builder.Configuration);
builder.Services.InitTicketModule(builder.Configuration);
builder.Services.InitCoreModule(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseApiCustomExceptionHandler();
app.MapControllers();

app.Run();
