using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5179", "http://192.168.1.150:5179");
builder.Services.AddMvc ();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ContextoDb> (
    options => options.UseMySql (
            builder.Configuration ["ConnectionStrings:DefaultConnection"], new MariaDbServerVersion (new Version (10, 4, 21))
    )
);
builder.Services.AddControllers ();

builder.Services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme).AddJwtBearer (options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration ["TokenAuthentication:Issuer"],
        ValidAudience = builder.Configuration ["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey (System.Text.Encoding.UTF8.GetBytes (builder.Configuration ["TokenAuthentication:SecretKey"]))
    };
});

builder.Services.AddAuthorization (options => {
    options.AddPolicy ("Cliente", policy => {
        policy.RequireRole ("Cliente");
    });
    options.AddPolicy ("Cocina", policy => {
        policy.RequireRole ("Cocina");
    });
    options.AddPolicy ("Restaurante", policy => {
        policy.RequireAssertion (context => (
            context.User.HasClaim (claim => claim.Type == ClaimTypes.Role && (claim.Value == "Cocina" || claim.Value == "Cliente"))
        ));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseStaticFiles ();

app.Run ();