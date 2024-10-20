using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Azure.Storage.Blobs;
using FileService.API;
using FileService.Infrastructure;
using FileService.Application.Interfaces;
var builder = WebApplication.CreateBuilder(args);


// Get configuration value from appsettings.json
var fileStoragePath = builder.Configuration.GetSection("FileStorageSettings:LocalStoragePath").Value;
// Register LocalFileStorage with the file storage path
builder.Services.AddScoped<IFileStorage>(provider => new LocalFileStorage(fileStoragePath));


// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IFileService, FileService.Application.Repository.FileService>();
builder.Services.AddSingleton<ILeadRepository, FileService.Infrastructure.LeadRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()    // Allows any origin
               .AllowAnyMethod()    // Allows any HTTP method (GET, POST, etc.)
               .AllowAnyHeader();   // Allows any headers
    });
});

// Configure JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"]
    };
});

builder.Services.AddAuthorization();


// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Files Upload Service", Version = "v1" });

    // Add security definitions for JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // Support for File Upload in Swagger
    c.OperationFilter<FileUploadOperationFilter>();

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
        
});

//builder.Services.AddSingleton<BlobServiceClient>(serviceProvider =>
//{
//    var connectionString = builder.Configuration["ConnectionStrings:AzureBlobStorage"];
//    //var connectionString = builder.Configuration["BlobStorage:ConnectionString"];
//    return new BlobServiceClient(connectionString);
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Upload API v1");
        //c.RoutePrefix = string.Empty; // To serve Swagger at the app root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");  // Apply CORS policy

app.UseAuthorization();

app.UseStaticFiles(); // Enable serving of static files from wwwroot

app.MapControllers();

app.Run();
