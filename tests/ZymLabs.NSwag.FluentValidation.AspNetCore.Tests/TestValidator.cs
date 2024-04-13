using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ZymLabs.NSwag.FluentValidation.AspNetCore.Tests;

public class TestValidator : AbstractValidator<HttpContextAccessor>;