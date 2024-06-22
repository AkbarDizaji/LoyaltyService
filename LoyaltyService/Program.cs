using Application;
using Infrastructure;
using LoyaltyService.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("log.txt")
        .CreateLogger();

    //log.Information("Hello, Serilog!");

    var builder = WebApplication.CreateBuilder(args);


    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var redisConnectionString = configuration["Redis:ConnectionString"];
        Log.Information(redisConnectionString);
        return ConnectionMultiplexer.Connect(redisConnectionString);
    });
    IdentityModelEventSource.ShowPII = true;
    var secret = "1k0GC0D57Tpb1c5duCHLAppgGQ7WHmVg";

    var secretBytes = Convert.FromBase64String(secret);

    var symmetricKey = new SymmetricSecurityKey(secretBytes);


    TokenValidationParameters CreateTokenValidationParameters()
    {
        var result = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidIssuer = "http://localhost:8080/realms/public",

            ValidateAudience = false,
            ValidAudience = "account",

            ValidateIssuerSigningKey = false,

            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)),
            //comment this and add this line to fool the validation logic
            SignatureValidator = delegate (string token, TokenValidationParameters parameters)
            {
                var jwt = new JsonWebToken(token);
                return jwt;
            },

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero,
        };

        result.RequireSignedTokens = false;

        return result;
    }
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {

        options.Authority = "http://keycloak:8080/realms" + "/" + builder.Configuration["KeyCloak:realm"]; // Keycloak server url
        options.Audience = builder.Configuration["KeyCloak:resource"];
        options.RequireHttpsMetadata = false; // change to true in production
        options.TokenValidationParameters = CreateTokenValidationParameters();
    });






    builder.Services.AddSwaggerGen(x =>
    {
        var headerName = "Authorization";
        x.AddSecurityDefinition(headerName,
            new OpenApiSecurityScheme
            {
                Description = $"Api key needed to access the endpoints. {headerName}: myApiKey",
                In = ParameterLocation.Header,
                Name = headerName,
                Type = SecuritySchemeType.ApiKey
            });

        x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = headerName,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = headerName }
            },
            Array.Empty<string>()
        }
    });

    });
    InfrastructureDependencyRegistration.RegisterServices(builder.Services, builder.Configuration);
    ApplicationDependencyRegistration.RegisterServices(builder.Services, builder.Configuration);

    var app = builder.Build();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRouting();
    app.UseAuthorization();

    app.UseMiddleware<WiringMiddleware>();
    app.UseMiddleware<RequestCachingMiddleware>();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    app.UseMiddleware<PrometheusMiddleware>();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapMetrics();
    });
    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    throw;
}

