using Microsoft.EntityFrameworkCore;
using backend.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using backend.Mappers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var pruebaDotnetCors = "_pruebaDotnetCors";

builder.Services.AddCors(options => 
  {
    options.AddPolicy
    (
      name: pruebaDotnetCors,
      policy => 
      {
        policy.WithOrigins("http://localhost:4200")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
      }
    );
  });

// Add services to the container.

builder.Services.AddControllers();
    /* .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            options.JsonSerializerOptions.WriteIndented = true;
        }); */
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ConnSqlServer>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("connStringSqlServer")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// JWT

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
    options.Events = new JwtBearerEvents
    {
      OnMessageReceived = context =>
      {
        context.Token = context.HttpContext.Request.Cookies["authToken"];
        return Task.CompletedTask;
      }
    };
});

// Mapper
var mapperConfig = new MapperConfiguration(m => {
  m.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddMvc();


var app = builder.Build();

var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("admin1234", 13);
Console.WriteLine("hash");
Console.WriteLine(passwordHash);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(pruebaDotnetCors);
// app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
