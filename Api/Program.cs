using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // This is completely ignore by the TestServer...
        builder.Services.Configure<KestrelServerOptions>(options => options.ConfigureHttpsDefaults(options =>
            {
                options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                options.OnAuthenticate = (connectionContext, sslServerAuthenticationOptions) =>
                {
                    // Configure SSL options per request here...
                };
                options.ClientCertificateValidation = (cert, chain, policyErrors) =>
                {
                    // Just accept any certificate, but it must be present. Otherwise this method won't ever be called.
                    return true;
                };
            }));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(options =>
            {
                options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
                options.ValidateCertificateUse = false;
                options.Events = new CertificateAuthenticationEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Fail("Invalid certificate");
                        return Task.CompletedTask;
                    },
                    OnCertificateValidated = context =>
                    {
                        // When a certificate is valid we can base our Principal (our authenticated entity) on the certificate (and other info that we need).
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                            new Claim(ClaimTypes.Name, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                        };

                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                        context.Success();

                        return Task.CompletedTask;
                    }
                };
            });

        var app = builder.Build();

        app.UseAuthentication();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
