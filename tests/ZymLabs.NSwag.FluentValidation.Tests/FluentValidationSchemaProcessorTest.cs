﻿using Moq;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using Xunit;

namespace ZymLabs.NSwag.FluentValidation.Tests;

public class FluentValidationSchemaProcessorTest
{
    [Fact]
    public void ProcessIncludesDefaultRuleLength()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Equal(10, schema.Properties["Length"].MinLength);
        Assert.Equal(20, schema.Properties["Length"].MaxLength);
    }

    [Fact]
    public void ProcessIncludesDefaultRuleEmailAddress()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        string? pattern = schema.Properties["EmailAddress"].Pattern;

        // Assert
        Assert.NotNull(pattern);

        Assert.NotEmpty(pattern);
    }

    [Fact]
    public void ProcessIncludesDefaultRuleNotEmpty()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Equal(1, schema.Properties["NotEmpty"].MinLength);
        Assert.False(schema.Properties["NotEmpty"].IsNullable(SchemaType.OpenApi3));

        var notEmptyChildProperty = schema.Properties["NotEmptyChild"];
        Assert.Empty(notEmptyChildProperty.OneOf);
        Assert.NotNull(notEmptyChildProperty.Reference);
    }

    [Fact]
    public void ProcessIncludesDefaultRuleNotNull()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Contains("NotNull", schema.RequiredProperties);

        var test = schema.Properties["NotNull"];
        bool value = test.IsNullable(SchemaType.OpenApi3);
        Assert.False(value);

        var notNullChildProperty = schema.Properties["NotNullChild"];
        Assert.Empty(notNullChildProperty.OneOf);
        Assert.NotNull(notNullChildProperty.Reference);
    }

    [Fact]
    public void ProcessIncludesDefaultRuleRegex()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Equal(@"(\d{4})-(\d{2})-(\d{2})", schema.Properties["RegexField"].Pattern);
    }

    [Fact]
    public void ProcessIncludesDefaultRuleValueInRange()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Equal(5, schema.Properties["ValueInRange"].Minimum);
        Assert.Equal(10, schema.Properties["ValueInRange"].Maximum);
    }

    [Fact]
    public void ProcessIncludesDefaultRuleValueInRangeDouble()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Equal(Convert.ToDecimal(2.2), schema.Properties["ValueInRangeDouble"].ExclusiveMinimum);
        Assert.Equal(Convert.ToDecimal(7.5f), schema.Properties["ValueInRangeDouble"].ExclusiveMaximum);
    }

    [Fact]
    public void ProcessIncludesDefaultRuleValueInRangeFloat()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Equal(Convert.ToDecimal(1.1f), schema.Properties["ValueInRangeFloat"].Minimum);
        Assert.Equal(Convert.ToDecimal(5.3f), schema.Properties["ValueInRangeFloat"].Maximum);
    }

    [Fact]
    public void ProcessIncludesIncludedValidators()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Contains("IncludeField", schema.RequiredProperties);
    }

    [Fact]
    public void ProcessModifiesSchemaToContainValidations()
    {
        // Arrange
        var jsonSchemaGeneratorSettings = CreateJsonSchemaGeneratorSettings();

        // Act
        var schema = JsonSchema.FromType<MockValidationTarget>(jsonSchemaGeneratorSettings);

        // Assert
        Assert.Contains("NotNull", schema.RequiredProperties);
        Assert.Equal(1, schema.Properties["NotEmpty"].MinLength);
    }

    private JsonSchemaGeneratorSettings CreateJsonSchemaGeneratorSettings()
    {
        var testValidator = new MockValidationTargetValidator();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(testValidator);

        var validatorFactory = serviceProvider.Object;

        var fluentValidationSchemaProcessor = new FluentValidationSchemaProcessor(validatorFactory);

        var jsonSchemaGeneratorSettings = new NewtonsoftJsonSchemaGeneratorSettings();
        jsonSchemaGeneratorSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);

        return jsonSchemaGeneratorSettings;
    }
}