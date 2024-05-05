using FluentValidation;
using NJsonSchema.Generation.TypeMappers;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;
using ZymLabs.NSwag.FluentValidation.WebApi.Tests;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddValidatorsFromAssemblyContaining<WeatherForecastValidator>();

builder.Services.AddOpenApiDocument((document, serviceProvider) =>
{
    var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetService<FluentValidationSchemaProcessor>();

    // Add the fluent validations schema processor
    document.SchemaSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);

    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());

    //document.OperationProcessors.Add(new SwaggerGlobalAuthProcessor());

    document.OperationProcessors.Add(new OperationProcessor(context =>
    {
        // Use ControllerType to fetch the controller name
        string controllerName = context.ControllerType.Name;
        if (controllerName.EndsWith("Controller"))
        {
            controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
        }

        context.OperationDescription.Operation.Tags = [controllerName];
        return true;
    }));

    document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(TimeSpan), schema =>
    {
        schema.Type = NJsonSchema.JsonObjectType.String;
        schema.IsNullableRaw = true;
        schema.Pattern = @"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$";
        schema.Example = "02:00:00";
    }));

    //document.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());
});

builder.Services.AddScoped(provider =>
{
    var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
    var loggerFactory = provider.GetService<ILoggerFactory>();

    return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
});

//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi(options =>
{
    options.DefaultModelsExpandDepth = -1;
    options.DocExpansion = "none";
    options.AdditionalSettings.Add("displayRequestDuration", true);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
