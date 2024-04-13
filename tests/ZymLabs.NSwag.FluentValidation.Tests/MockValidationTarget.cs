namespace ZymLabs.NSwag.FluentValidation.Tests;

public class MockValidationTarget
{
    public string PropertyWithNoRules { get; set; } = string.Empty;

    public string Length { get; set; } = string.Empty;
    public string NotNull { get; set; } = string.Empty;
    public string NotEmpty { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string EmailAddressNet4 { get; set; } = string.Empty;

    public string RegexField { get; set; } = string.Empty;

    public int ValueInRange { get; set; }
    public int ValueInRangeExclusive { get; set; }

    public float ValueInRangeFloat { get; set; }
    public double ValueInRangeDouble { get; set; }

    public string IncludeField { get; set; } = string.Empty;

    public MockValidationTargetChild NotNullChild { get; set; } = new MockValidationTargetChild();
    public MockValidationTargetChild NotEmptyChild { get; set; } = new MockValidationTargetChild();

    public MockValidationTargetChild NotNullChildEnum { get; set; }
    public MockValidationTargetChild NotEmptyChildEnum { get; set; }
}