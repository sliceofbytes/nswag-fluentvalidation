using FluentValidation.Validators;

namespace ZymLabs.NSwag.FluentValidation;

/// <summary>
/// FluentValidationRule.
/// </summary>
/// <remarks>
/// Creates new instance of <see cref="FluentValidationRule"/>.
/// </remarks>
/// <param name="name">Rule name.</param>
public class FluentValidationRule(string name)
{
    /// <summary>
    /// Rule name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Predicate to match property validator.
    /// </summary>
    public Func<IPropertyValidator, bool> Matches { get; set; } = _ => false;

    /// <summary>
    /// Modify Swagger schema action.
    /// </summary>
    public Action<RuleContext> Apply { get; set; } = _ => { };
}