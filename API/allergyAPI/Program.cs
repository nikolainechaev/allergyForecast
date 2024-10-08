using allergyAPI.Services;

var builder = WebApplication.CreateBuilder(args);
// 1st Section: Middleware
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpecificOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// 1.5st Section: My personal injections
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>(client =>
{
    var ApiKey = Environment.GetEnvironmentVariable("GEOCODING_ENV") ?? throw new InvalidOperationException("API key not set.");
    client.DefaultRequestHeaders.Add("x-api-key", ApiKey);

});
builder.Services.AddHttpClient<IPollenForecastService, PollenForecastService>();
builder.Services.AddHttpClient<IPlantInfoService, PlantInfoService>();

// 2nd Section: Apply dependencies and Build
var app = builder.Build();
app.UseCors("SpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseResponseCaching();
app.MapControllers();

// 3rd Section: Run web server
app.Run();
