using Marcador.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Servicios básicos
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<MarcadorService>();

// ✅ CORS para desarrollo (Angular en 4200)
const string CorsDev = "CorsDesarrollo";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsDev, p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // ✅ En desarrollo NO redirigimos a HTTPS para que el proxy funcione en http://localhost:5000
    // (En prod, detrás de Nginx, sí usarás HTTPS en el proxy)
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsDev);
app.UseAuthorization();
app.MapControllers();

// ✅ Fuerza a escuchar en http://localhost:5000
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

app.Run();
