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

// Inicializaci�n de la conexi�n a la base de datos
Conexion.Initialize(builder.Configuration);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Registro del servicio de correo electr�nico
builder.Services.AddScoped<IEmailService, EmailService>();
// Configuraci�n de autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Index"; // Ruta para la p�gina de login
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // Duraci�n de la cookie de autenticaci�n
        options.SlidingExpiration = true; // Renovaci�n autom�tica si hay actividad
    });

// Configuraci�n para sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1); // Tiempo de expiraci�n de la sesi�n
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Asegura cookies solo en HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Refuerza la pol�tica de cookies si es necesario
});

// Registrar la clase de conexi�n
builder.Services.AddTransient<Conexion>();

// Configuraci�n de l�mites de subida de archivos grandes
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // Tama�o m�ximo permitido (100MB)
});

// Inicializar OpenAI API
builder.Services.AddSingleton(new OpenAI.OpenAIClient(builder.Configuration["OpenAI:ApiKey"]));

// Configuraci�n de Rotativa para generar PDFs
Rotativa.AspNetCore.RotativaConfiguration.Setup(builder.Environment.WebRootPath, "Rotativa");

var app = builder.Build();

// Configuraci�n del middleware de manejo de excepciones
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseSession(); // Habilitar sesiones
app.UseRouting();

// Activar la autenticaci�n y las sesiones en el pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Index}/{id?}");

app.Run();
