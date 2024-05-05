using FluentValidation;

namespace ZymLabs.NSwag.FluentValidation.WebApi.Tests;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}


public class WeatherForecastValidator : CustomValidator<WeatherForecast>
{
    public WeatherForecastValidator()
    {
        RuleFor(p => p.Summary)
            .NotEmpty()
            .MaximumLength(75);

        RuleFor(p => p.TemperatureC)
                .NotEmpty()
                .NotNull()
                .Must(p => p >= -20 && p <= 55)
                    .WithMessage("Temperature must be between -20 and 55");
    }
}