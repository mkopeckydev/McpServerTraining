using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace McpServerTraining;

public sealed class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public TokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var configuredToken = _configuration["Authentication:Token"];
        if (string.IsNullOrWhiteSpace(configuredToken))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authentication token is not configured."));
        }

        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeader) ||
            !authorizationHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var providedToken = authorizationHeader.ToString()["Bearer ".Length..].Trim();
        var configuredTokenBytes = Encoding.UTF8.GetBytes(configuredToken);
        var providedTokenBytes = Encoding.UTF8.GetBytes(providedToken);

        if (!CryptographicOperations.FixedTimeEquals(configuredTokenBytes, providedTokenBytes))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid authentication token."));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "McpClient") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers.WWWAuthenticate = "Bearer";
        return Task.CompletedTask;
    }
}
