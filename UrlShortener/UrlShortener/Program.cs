using UrlShortener;
using UrlShortener.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.
builder.Services.AddTransient<IUrlHandler, UrlHandler>();
builder.Services.AddTransient<IUrlRepository, UrlRepository>();
builder.Services.AddTransient<IHelper, Helper>();
builder.Services.AddTransient<IDatabaseHelper, DatabaseHelper>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApiDocument(); // https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

// this is unsecure and for development purposes only
// only here to show that I know how to write one
// even if it exposes everything ...
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// this is unsecure and for development purposes only 
app.UseCors("AllowAll");

app.MapControllers();

app.Run();


