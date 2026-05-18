using MarketplaceApi.Application.Interfaces;
using MarketplaceApi.Application.Services;
using MarketplaceApi.Domain.Interfaces;
using MarketplaceApi.Infrastructure.Data;
using MarketplaceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MarketplaceApi.Shared.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);


// 1. Agrega el soporte para controladores
builder.Services.AddControllers();
//servicio a la bd

// Solo para probar si el error desaparece:
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // Esto es vital para que reconozca la estructura de carpetas
        b => b.MigrationsAssembly("MarketplaceApi.Infrastructure")
    ));
// Registro del repositorio genérico
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// Registro del servicio de autenticación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IHorarioService, HorarioService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<JwtService>();
// Agrega esta línea antes de var app = builder.Build();
builder.Services.AddHttpContextAccessor();

// configurar cors para permitir peticiones desde el frontend (Next.js)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextjs",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
// Add services to the container.
// Configuración de Swagger at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configurar JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key no configurada");
var key = Encoding.ASCII.GetBytes(jwtKey);

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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors("AllowNextjs");

// Habilitar Swagger solo en desarrollo (puedes quitar el 'if' si lo quieres siempre)
if (app.Environment.IsDevelopment())
{
    // Agrega esta línea antes de UseAuthorization
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketplace API v1");
        c.RoutePrefix = string.Empty; // Esto hace que Swagger salga en la raíz (http://localhost:5000/)
    });
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//Mapea las rutas de los controladores
app.MapControllers();

app.Run();

