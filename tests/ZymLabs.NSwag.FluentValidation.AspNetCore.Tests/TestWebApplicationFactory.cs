using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ZymLabs.NSwag.FluentValidation.AspNetCore.Tests;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder

            // Runs after Startup.ConfigureServices() to replace services in Startup class
            .ConfigureTestServices(services =>
            {
                // HttpContextServiceProviderValidatorFactory requires access to HttpContext
                services.AddHttpContextAccessor();

                services.AddControllers();
                services.AddValidatorsFromAssemblyContaining<TestValidator>();
                services.AddFluentValidationAutoValidation(_ =>
                {
                });
            })
            .UseEnvironment("Test");
    }

    public HttpClient GetAnonymousClient()
    {
        return CreateClient();
    }
}
