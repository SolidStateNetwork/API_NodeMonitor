using API_NodeMonitor.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS-Policy hinzufügen
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "WebAppPolicy",
                      policy =>
                      {
                          policy.AllowAnyOrigin() 
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// 2. Controller und API-Dokumentation (Swagger)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. HttpClient und unsere Services registrieren
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IBitcoinService, BitcoinService>();
builder.Services.AddSingleton<IMoneroService, MoneroService>();

var app = builder.Build();

// 4. Konfigurieren der HTTP-Request-Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("WebAppPolicy");

app.UseAuthorization();

app.MapControllers();

// Die Anwendung starten
app.Run();