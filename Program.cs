global using dotnet_rpg.Models;
global using dotnet_rpg.Services.CharacterService;
global using dotnet_rpg.Dtos.Character;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using dotnet_rpg.Data;
global using Microsoft.AspNetCore.Mvc;
global using dotnet_rpg.Dtos.User;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using Swashbuckle.AspNetCore.Filters;
global using Microsoft.OpenApi.Models;
global using dotnet_rpg.Dtos.Weapon;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Authorization;
global using dotnet_rpg.Services.WeaponsService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Veritabanı bağlamayı ekliyoruz.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Test methodlarını ekliyoruz.
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        Description = """Standard Authorization header using the Bearer scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

//AutoMapper'ı ekliyoruz.
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//Enjekte ediyoruz.
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IWeaponService, WeaponService>();

//Authentication u programa ekleme.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters{
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

//db bağlantıları için ekliyoruz.
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Authentication'u etkinleştiriyoruz.
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
