using Marcador.Api.Services;
using Marcador.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar EF Core con SQL Server (solo una vez)
builder.Services.AddDbContext<MarcadorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Agregar MarcadorService como Scoped, porque depende de DbContext
builder.Services.AddScoped<MarcadorService>();

// Agregar servicios al contenedor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(); // Para usar controladores

var app = builder.Build();

// Inicialización al arrancar la API (si quieres empezar siempre en 0)
using (var scope = app.Services.CreateScope())
{
    var svc = scope.ServiceProvider.GetRequiredService<MarcadorService>();
    svc.InicializarEnCero();   // ← deja todo en 0 al levantar la API
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();  // Esto activa los controladores

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarcadorDbContext>();
    db.Database.Migrate();
}

// Liveness simple
app.MapGet("/healthz", () => Results.Ok("OK"));

// (Opcional) Readiness con chequeo de BD
app.MapGet("/ready", async (MarcadorDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return canConnect ? Results.Ok(new { db = "up" }) : Results.StatusCode(503);
});

app.Run();
