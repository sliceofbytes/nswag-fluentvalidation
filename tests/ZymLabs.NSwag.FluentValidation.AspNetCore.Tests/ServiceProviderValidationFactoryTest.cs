using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ZymLabs.NSwag.FluentValidation.AspNetCore.Tests;

public class ServiceProviderValidationFactoryTest
{
    [Fact]
    public void CreateInstanceReturnsValidator()
    {
        // Arrange
        var mockValidator = new TestValidator();
        var validatorType = typeof(TestValidator);

        // Mock ServiceProvider
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(validatorType))
            .Returns(mockValidator);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope
            .Setup(x => x.ServiceProvider)
            .Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(serviceScope.Object);

        serviceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        // Mock HttpContextAccessor
        var httpContextAccessor = new Mock<IHttpContextAccessor>();

        // Mock HttpContext
        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider.Object
        };

        httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

        // Act
        var validator = httpContextAccessor.Object.HttpContext?.RequestServices.GetService<TestValidator>();

        // Assert
        Assert.IsType<TestValidator>(validator);
    }

    [Fact]
    public void CreateInstanceReturnsNullWhenValidatorNotExist()
    {
        // Arrange
        IValidator? mockValidator = null;
        var validatorType = typeof(TestValidator);

        // Mock ServiceProvider
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(validatorType))
            .Returns(mockValidator);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope
            .Setup(x => x.ServiceProvider)
            .Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(serviceScope.Object);

        serviceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        // Mock HttpContextAccessor
        var httpContextAccessor = new Mock<IHttpContextAccessor>();

        // Mock HttpContext
        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider.Object
        };

        httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

        // Act
        var validator = httpContextAccessor.Object.HttpContext?.RequestServices.GetService<TestValidator>();

        // Assert
        Assert.Null(validator);
    }
}