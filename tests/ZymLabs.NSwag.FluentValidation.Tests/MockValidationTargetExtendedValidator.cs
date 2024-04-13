using FluentValidation;

namespace ZymLabs.NSwag.FluentValidation.Tests;

public class MockValidationTargetExtendedValidator : AbstractValidator<MockValidationTargetExtended>
{
    /// <inheritdoc />
    public MockValidationTargetExtendedValidator() => RuleFor(sample => sample.NotEmpty).NotEmpty();
}