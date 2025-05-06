using BankingApi.Data;
using BankingApi.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure database connection
if (builder.Environment.IsProduction())
{
    // Use Key Vault in production
    var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
    var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    var connectionString = secretClient.GetSecret("BankingDB").Value.Value;
    
    builder.Services.AddDbContext<BankingContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    // Use local connection string in development
    builder.Services.AddDbContext<BankingContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("BankingDB")));
}

// Add authentication
builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
    .AddApiKeyAuthentication(options =>
    {
        options.ApiKey = builder.Configuration["ApiKey"];
    });



var app = builder.Build();

app.UseHttpsRedirection();
app.UseErrorHandling(); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// API Key Authentication Handler (keep this part unchanged)
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string ApiKeyHeaderName = "X-API-Key";

    #pragma warning disable CS0618 // Suppress ISystemClock obsolete warning
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }
    #pragma warning restore CS0618

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key was not provided"));
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key was not provided"));
        }

        if (!string.Equals(providedApiKey, Options.ApiKey, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided"));
        }

        var identity = new ClaimsIdentity(Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public static class ApiKeyAuthenticationExtensions
{
    public static AuthenticationBuilder AddApiKeyAuthentication(
        this AuthenticationBuilder builder,
        Action<ApiKeyAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationOptions.DefaultScheme, 
            configureOptions);
    }
}

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public string ApiKey { get; set; }
}