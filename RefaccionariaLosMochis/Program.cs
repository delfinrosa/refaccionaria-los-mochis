using CapaDatos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using OpenAI; // Actualizado para usar la nueva biblioteca
using OpenAI.Chat; // Ajustado para la nueva estructura
using OpenAI.Models; // Ajustado para la nueva estructura
using System.IO;
using System.Threading.Tasks;
using RefaccionariaLosMochis.Services;

var builder = WebApplication.CreateBuilder(args);

// Inicialización de la conexión a la base de datos
Conexion.Initialize(builder.Configuration);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Registro del servicio de correo electrónico
builder.Services.AddScoped<IEmailService, EmailService>();
// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Index"; // Ruta para la página de login
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // Duración de la cookie de autenticación
        options.SlidingExpiration = true; // Renovación automática si hay actividad
    });

// Configuración para sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Asegura cookies solo en HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Refuerza la política de cookies si es necesario
});

// Registrar la clase de conexión
builder.Services.AddTransient<Conexion>();

// Configuración de límites de subida de archivos grandes
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // Tamaño máximo permitido (100MB)
});

// Inicializar OpenAI API
builder.Services.AddSingleton(new OpenAI.OpenAIClient(builder.Configuration["OpenAI:ApiKey"]));

// Configuración de Rotativa para generar PDFs
Rotativa.AspNetCore.RotativaConfiguration.Setup(builder.Environment.WebRootPath, "Rotativa");

var app = builder.Build();

// Configuración del middleware de manejo de excepciones
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseSession(); // Habilitar sesiones
app.UseRouting();

// Activar la autenticación y las sesiones en el pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Index}/{id?}");

app.Run();
