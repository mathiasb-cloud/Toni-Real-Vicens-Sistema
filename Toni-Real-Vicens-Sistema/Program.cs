using Toni_Real_Vicens_Sistema.Service;
var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<AlumnoService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<FichaService>();

// Añadir soporte para Sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();

var app = builder.Build();

// --- 2. CONFIGURACIÓN DEL PIPELINE (EL ORDEN IMPORTA) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// *** IMPORTANTE: UseSession debe ir DESPUÉS de UseRouting y ANTES de UseAuthorization ***
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");