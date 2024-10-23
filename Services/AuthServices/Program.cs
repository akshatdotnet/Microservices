using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthService.Application.Repository;
using AuthService.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// --- Add Services to Container --- //

// Load JWT configuration from appsettings
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettingsSection["SecretKey"]);
//var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // Ensure HTTPS for security
    options.SaveToken = true; // Save token in the request for reuse
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Validate the signing key
        IssuerSigningKey = new SymmetricSecurityKey(secretKey), // Set symmetric security key
        ValidateIssuer = true, // Validate the JWT issuer
        ValidateAudience = true, // Validate the audience
        ValidIssuer = jwtSettingsSection["Issuer"], // Issuer from configuration
        ValidAudience = jwtSettingsSection["Audience"], // Audience from configuration
        ValidateLifetime = true, // Ensure token expiration is checked
        ClockSkew = TimeSpan.Zero // Set no tolerance for expired tokens
    };
});


// Add application services
builder.Services.AddScoped<IAuthService, AuthService.Application.Repository.AuthService>();

// Add infrastructure services
builder.Services.AddScoped<ITokenService, JwtTokenService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Build and Configure the HTTP Pipeline --- //
var app = builder.Build();

// Use Swagger in Development environment only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use HTTPS redirection (highly recommended in production)
app.UseHttpsRedirection();

// Enable authentication middleware (JWT Authentication)
app.UseAuthentication(); // Added to validate JWT on requests

// Enable authorization
app.UseAuthorization();

// Map API controller routes
app.MapControllers();

// Run the application
app.Run();
