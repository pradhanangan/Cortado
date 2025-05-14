using Shared.Common;
using Bookings.Application;
using Bookings.Infrastructure;
using Customers.Application;
using Customers.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Products.Application;
using Products.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using Shared.Common.Logging;
using Cortado.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Configure logging using Shared.Common
LoggingConfiguration.ConfigureSerilog(builder);

//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(resource => resource.AddService("Cortado-Test600"))
//    .WithMetrics(metrics => {
//        metrics
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation();

//        metrics.AddOtlpExporter(option => {
//            option.Endpoint = new Uri("http://localhost:18889");
//        });
//    })
//    .WithTracing(tracing => {
//        tracing
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddEntityFrameworkCoreInstrumentation();


//        tracing.AddOtlpExporter(option => {
//            option.Endpoint = new Uri("http://localhost:18889");
//        });
//    });

//builder.Logging.ClearProviders(); // Clear default logging providers
//builder.Logging
//    .AddOpenTelemetry(logging =>
//    {
//        logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Cortado-Test400"));
//        logging.AddOtlpExporter(option =>
//        {
//            option.Endpoint = new Uri("http://localhost:18889"); // OpenTelemetry OTLP/gRPC endpoint
//            option.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//        });
//    });


// Allow CORs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllCors",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      });
});

//Add services to the container.
builder.Services.AddCustomersApplicationServices(builder.Configuration);
builder.Services.AddCustomersInfrastructureServices(builder.Configuration);

builder.Services.AddBookingsApplicationServices(builder.Configuration);
builder.Services.AddBookingsInfrastructureServices(builder.Configuration);

builder.Services.AddProductsApplicationServices(builder.Configuration);
builder.Services.AddProductsInfrastructureServices(builder.Configuration);

builder.Services.AddCommonServices(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
    //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); // This will pick the first action
});

builder.Services.AddRazorPages();  // Add this line

builder.Services.AddAuthentication(options =>
{
    ////options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    ////options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultScheme = "MyCookieAuth";
    //options.DefaultChallengeScheme = "MyCookieAuth";
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    //.AddCookie("MyCookieAuth", options =>
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "MyCookieAuth";
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    })
     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
     {
         options.MapInboundClaims = false;
         options.TokenValidationParameters = new TokenValidationParameters
         {
             IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
             {
                 // get JsonWebKeySet from AWS 
                 var json = new WebClient().DownloadString("https://cognito-idp.ap-southeast-2.amazonaws.com/ap-southeast-2_DDjbonXfo/.well-known/jwks.json");

                 // serialize the result 
                 var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;

                 // cast the result to be the type expected by IssuerSigningKeyResolver 
                 return (IEnumerable<SecurityKey>)keys;
             },
             ValidateIssuer = true,
             ValidateAudience = false,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = "https://cognito-idp.ap-southeast-2.amazonaws.com/ap-southeast-2_DDjbonXfo",
             RoleClaimType = "cognito:groups",
             //NameClaimType ="sub"
             //ValidAudience = "7qin2t9mgeicjmtpe3lcv0ae9s"     // Cognito App Client Id
             //AudienceValidator = (audiences, securityToken, validationParameters) =>
             //{
             //    //This is necessary because Cognito tokens doesn't have "aud" claim. Instead the audience is set in "client_id"
             //    return validationParameters.ValidAudience.Contains(((JwtSecurityToken)securityToken).Payload["client_id"].ToString());
             //}
         };
         //options.MetadataAddress = "https://cognito-idp.ap-southeast-2.amazonaws.com/ap-southeast-2_DDjbonXfo/.well-known/jwks.json";
     });

//builder.WebHost.UseUrls("http://*:5000");

var app = builder.Build();

app.UseCors("AllowAllCors");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Modular Monolith API v1");
        //c.SwaggerEndpoint("https://localhost:7204/swagger/v1/swagger.json", "Other Project 1 API V1");
        //c.SwaggerEndpoint("https://localhost:7279/swagger/v1/swagger.json", "Other Project 2 API V1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
