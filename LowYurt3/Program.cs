using LowYurt.Services;
using LowYurt3.Models;
using MercadoPago.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// ================================
// 1. Configurar Entity Framework
// ================================
builder.Services.AddDbContext<LowYurtContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion")));

// ================================
// 2. Configurar controladores y vistas
// ================================
builder.Services.AddControllersWithViews();

// ================================
// 3. Configurar autenticación con cookies (SOLO UNA VEZ)
// ================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Si no está autenticado lo manda aquí
        options.LoginPath = "/Account/Login";

        // Si no tiene permisos lo manda aquí
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// ================================
// 4. Configurar autorización
// ================================
builder.Services.AddAuthorization();

// NUEVO: Configurar sesiones (esto no afecta la autenticación, solo agrega almacenamiento temporal)
builder.Services.AddDistributedMemoryCache();  // Almacenamiento en memoria para sesiones (ideal para desarrollo)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);  // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true;  // Seguridad: cookie solo accesible por HTTP
    options.Cookie.IsEssential = true;  // Cumple con GDPR y privacidad
});
builder.Services.AddHttpContextAccessor();

// Configurar MercadoPago
var mpConfig = builder.Configuration.GetSection("MercadoPago");
MercadoPagoConfig.AccessToken = mpConfig["AccessToken"];

// PayPal
builder.Services.AddScoped<PayPalService>();

var app = builder.Build();

// ================================
// 5. Middleware
// ================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// NUEVO: Usar sesiones aquí (después de Routing, antes de Authentication)
app.UseSession();

// Autenticación y autorización deben estar en este orden
app.UseAuthentication();
app.UseAuthorization();

// ================================
// 6. Rutas
// ================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();