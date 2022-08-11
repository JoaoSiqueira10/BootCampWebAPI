using BootCamp.WebAPI.Dal;
using BootCamp.WebAPI.Dal.Repositories;
using BootCamp.WebAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Transformar palavra chave em bytes:
var key = Encoding.ASCII.GetBytes(Settings.Secret);
// Para instanciar os serviços
builder.Services.AddAuthentication(x =>
{
    // Adicionamos a autenticação e dentro, as configurações necessárias (deve autenticar com o esquema
    // padrão).
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Chave para compilação:
.AddJwtBearer(x =>
{
    // Nenhum metadado poderá passar:
    x.RequireHttpsMetadata = false;
    // Salvar o token:
    x.SaveToken = true;
    // Parâmetros padrões requisitados:
    x.TokenValidationParameters = new TokenValidationParameters
    {
        // Assinatura:
        ValidateIssuerSigningKey = true,
        // Vai utilizar a key normal sempre que validar o código:
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // Valida se a assinatura está correta:
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ACME", Version = "v1", Description = "API REST" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer " +
        "scheme.\r\n\r\n Enter 'Bearer'[space] and then your token " +
        "in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
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

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IContrato, ContratoDal>();
builder.Services.AddTransient<IFornecedor, FornecedorDal>();
builder.Services.AddTransient<IProduto, ProdutoDal>();

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
    app.UseAuthorization();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
