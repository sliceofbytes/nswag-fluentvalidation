using FluentValidation;
using FluentValidation.Validators;

namespace ZymLabs.NSwag.FluentValidation;

/// <summary>
/// Extensions for some swagger specific work.
/// </summary>
internal static class ValidationExtensions
{
    /// <summary>
    /// Contains <see cref="IValidationRule"/> and additional info.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ValidationRuleContext"/> struct.
    /// </remarks>
    /// <param name="validationRule">PropertyRule.</param>
    /// <param name="isCollectionRule">Is a CollectionPropertyRule.</param>
    public readonly struct ValidationRuleContext(IValidationRule validationRule, bool isCollectionRule)
    {
        /// <summary>
        /// PropertyRule.
        /// </summary>
        public readonly IValidationRule ValidationRule = validationRule;

        /// <summary>
        /// Flag indication whether the <see cref="IValidationRule"/> is the CollectionRule.
        /// </summary>
        public readonly bool IsCollectionRule = isCollectionRule;
    }

    /// <summary>
    /// Is supported swagger numeric type.
    /// </summary>
    public static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;

    /// <summary>
    /// Returns not null enumeration.
    /// </summary>
    public static IEnumerable<TValue> NotNull<TValue>(this IEnumerable<TValue>? collection)
    {
        return collection ?? [];
    }

    /// <summary>
    /// Returns validation rules by property name ignoring name case.
    /// </summary>
    /// <param name="validator">Validator.</param>
    /// <param name="name">Property name.</param>
    /// <returns>enumeration.</returns>
    public static IEnumerable<ValidationRuleContext> GetValidationRulesForMemberIgnoreCase(this IValidator validator, string name)
    {
        return (validator as IEnumerable<IValidationRule>)
               .NotNull()
               .GetPropertyRules()
               .Where(validationRuleContext => HasNoCondition(validationRuleContext.ValidationRule) && validationRuleContext.ValidationRule.PropertyName.EqualsIgnoreAll(name));
    }

    /// <summary>
    /// Returns property validators by property name ignoring name case.
    /// </summary>
    /// <param name="validator">Validator.</param>
    /// <param name="name">Property name.</param>
    /// <returns>enumeration.</returns>
    public static IEnumerable<IPropertyValidator> GetValidatorsForMemberIgnoreCase(this IValidator validator, string name)
    {
        return GetValidationRulesForMemberIgnoreCase(validator, name)
                .SelectMany(
                    validationRuleContext =>
                    {
                        return validationRuleContext.ValidationRule.Components.Select(c => c.Validator);
                    });
    }

    /// <summary>
    /// Returns all IValidationRules that are PropertyRule.
    /// If rule is CollectionPropertyRule then isCollectionRule set to true.
    /// </summary>
    internal static IEnumerable<ValidationRuleContext> GetPropertyRules(
        this IEnumerable<IValidationRule> validationRules)
    {
        foreach (var validationRule in validationRules)
        {
            bool isCollectionRule = validationRule.GetType() == typeof(ICollectionRule<,>);
            yield return new ValidationRuleContext(validationRule, isCollectionRule);
        }
    }

    /// <summary>
    /// Returns a <see cref="bool"/> indicating if the <paramref name="propertyRule"/> is conditional.
    /// </summary>
    internal static bool HasNoCondition(this IValidationRule propertyRule)
    {
        return !propertyRule.HasCondition && !propertyRule.HasAsyncCondition;
    }
}