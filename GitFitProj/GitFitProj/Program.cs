using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using GitFitProj.Controllers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using GitFitProj;




var builder = WebApplication.CreateBuilder(args);



// Swagger/OpenAPI Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GitFit API",
        Version = "v1",
        Description = "An API for a fitness tracking application",
        Contact = new OpenApiContact
        {
            Name = "GitFit Support",
            Email = "support@gitfitapi.com"
        }
    });
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // UseFullTypeNameInSchemaIds replacement for .NET Core
    c.CustomSchemaIds(type => type.ToString());
});







builder.Services.AddAuthorization();




// Localization Configuration
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");



// Controller and Swagger Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GitFitContext>(options =>
    options.UseSqlServer(connectionString));

//More resilience against transient failures
builder.Services.AddDbContext<GitFitContext>(options =>
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,       // Maximum number of retries on failed attempts
                maxRetryDelay: TimeSpan.FromSeconds(30),  // Maximum delay between retries
                errorNumbersToAdd: null  // SQL error numbers to consider for retries
            );
        })
);








var app = builder.Build();

//Localization for languages
var supportedCultures = new[] {"de-DE", "zh-CN" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GitFit v1");
        //c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}
// Middleware to handle exceptions globally
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// Map the controllers
app.MapControllers();
// Run the app
app.Run();