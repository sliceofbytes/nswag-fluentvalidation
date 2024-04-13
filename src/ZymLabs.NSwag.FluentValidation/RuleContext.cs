using FluentValidation.Validators;
using NJsonSchema.Generation;

namespace ZymLabs.NSwag.FluentValidation;

/// <summary>
/// RuleContext.
/// </summary>
/// <remarks>
/// Creates new instance of <see cref="RuleContext"/>.
/// </remarks>
/// <param name="schemaProcessorContext">SchemaProcessorContext.</param>
/// <param name="propertyKey">Property name.</param>
/// <param name="propertyValidator">Property validator.</param>
public class RuleContext(SchemaProcessorContext schemaProcessorContext, string propertyKey, IPropertyValidator propertyValidator)
{
    /// <summary>
    /// SchemaProcessorContext.
    /// </summary>
    public SchemaProcessorContext SchemaProcessorContext { get; } = schemaProcessorContext;

    /// <summary>
    /// Property name.
    /// </summary>
    public string PropertyKey { get; } = propertyKey;

    /// <summary>
    /// Property validator.
    /// </summary>
    public IPropertyValidator PropertyValidator { get; } = propertyValidator;
}