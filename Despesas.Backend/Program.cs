using CrossCutting.CommonDependenceInject;
using Despesas.Application.CommonDependenceInject;
using Despesas.Backend.CommonDependenceInject;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.CommonDependenceInject;

var builder = WebApplication.CreateBuilder(args);

// Add Cors Configurations 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://alexfariakof.com",
            "https://alexfariakof.com:42535",
            "https://localhost", 
            "https://localhost:42535",
            "https://localhost:4200",            
            "https://127.0.0.1",
            "https://127.0.0.1:4200",
            "https://127.0.0.1:42535",
            "https://accounts.google.com")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddSwaggerApiVersioning();


if (builder.Environment.IsStaging() || builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<RegisterContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("Dev.SqlConnectionString") ?? 
        throw new NullReferenceException("SqlConnectionString not defined.")));
}
else
{
    builder.Services.AddDbContext<RegisterContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("SqlConnectionString") ?? 
        throw new NullReferenceException("SqlConnectionString not defined.")));
}

//Add SigningConfigurations
builder.AddSigningConfigurations();

// Add AuthConfigurations
builder.AddAuthenticationConfigurations();

// Add Cryptography Configurations
builder.Services.AddServicesCryptography(builder.Configuration);

// Add CommonDependencesInject 
builder.Services.AddAutoMapper();
builder.Services.AddAmazonS3BucketConfigurations(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddCrossCuttingConfiguration();

if (builder.Environment.IsStaging())
{
    builder.WebHost.UseUrls("https://0.0.0.0:42535", "http://0.0.0.0:42536");
}

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHsts();

app.UseHttpsRedirection();

app.AddSupporteCulturesPtBr();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();

app.AddSwaggerUIApiVersioning();

app.UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseCertificateForwarding()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapFallbackToFile("index.html");
    });

if (app.Environment.IsStaging())
{
    app.Urls.Add("https://0.0.0.0:42535");
    app.Urls.Add("http://0.0.0.0:42536");
}

app.Run();