//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Weather.Application.Interfaces;
//using Weather.Infrastructure.ExternalServices;
//using Weather.Infrastructure.Mongo;

//var builder = WebApplication.CreateBuilder(args);

////////////////////////////////////////////////////////
//// 1️⃣ SERVICES
////////////////////////////////////////////////////////

//builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngular", policy =>
//    {
//        policy.WithOrigins("http://localhost:4200")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//// JWT
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
//        };
//    });

//builder.Services.AddAuthorization();

////////////////////////////////////////////////////////
//// 2️⃣ REGISTER YOUR SERVICES (🔥 THIS FIXES ERROR)
////////////////////////////////////////////////////////

//// HttpClient (Required for OpenWeatherService)
//builder.Services.AddHttpClient();

//// Register Weather Service
//builder.Services.AddScoped<IWeatherService, OpenWeatherService>();

//// Register Factory
//builder.Services.AddScoped<WeatherServiceFactory>();

//// Mongo Settings
//builder.Services.Configure<MongoDbSettings>(
//    builder.Configuration.GetSection("MongoDbSettings"));

//builder.Services.AddScoped<WeatherLogRepository>();

////////////////////////////////////////////////////////
//// 3️⃣ BUILD
////////////////////////////////////////////////////////

//var app = builder.Build();

////////////////////////////////////////////////////////
//// 4️⃣ PIPELINE
////////////////////////////////////////////////////////

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseCors("AllowAngular");

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Weather.Application.Interfaces;
using Weather.Infrastructure.ExternalServices;
using Weather.Infrastructure.Mongo;

var builder = WebApplication.CreateBuilder(args);

//////////////////////////////////////////////////////
// 1 SERVICES
//////////////////////////////////////////////////////

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

//////////////////////////////////////////////////////
// 2 REGISTER SERVICES
//////////////////////////////////////////////////////

// HttpClient
builder.Services.AddHttpClient();

// Weather Service
builder.Services.AddScoped<IWeatherService, OpenWeatherService>();

// Factory
builder.Services.AddScoped<WeatherServiceFactory>();

// ✅ MongoDB Settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// ✅ MongoDB Repository as Singleton (fixes logging issue)
builder.Services.AddSingleton<WeatherLogRepository>();

//////////////////////////////////////////////////////
// 3 BUILD
//////////////////////////////////////////////////////

var app = builder.Build();

//////////////////////////////////////////////////////
// 4 PIPELINE
//////////////////////////////////////////////////////

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();