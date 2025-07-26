﻿using FluentValidation;
using PeopleManagement.API.Requests;
using PeopleManagement.API.Resources;
using PeopleManagement.Domain.Enums;

namespace PeopleManagement.API.Validations;

public class BasePersonValidator<T> : AbstractValidator<T> where T : PersonRequest
{
    public BasePersonValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256)
            .WithMessage("Name must be at most 256 characters long")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.DateOfBirth)
            .NotNull()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today.AddYears(-150)))
            .WithMessage("Date of birth must be a realistic value")
            .When(p => p.DateOfBirth is not null);


        RuleFor(x => x.CPF)
            .NotEmpty().WithMessage("Invalid CPF")
            .Must(CpfValidator.IsCpfValid).WithMessage("Invalid CPF")
            .When(x => !string.IsNullOrWhiteSpace(x.CPF));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid Email")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Gender)
            .Must(g => Enum.TryParse<GenderType>(g, true, out _))
            .WithMessage("Gender must be 'Male', 'Female', or 'Other'")
            .When(x => !string.IsNullOrWhiteSpace(x.Gender));

        RuleFor(x => x.Naturality)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Naturality));

        RuleFor(x => x.Nationality)
            .MaximumLength(100)
            .Must(n => Countries.CountryNames
                .Any(c => string.Equals(c, n, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Invalid country")
            .When(x => !string.IsNullOrWhiteSpace(x.Nationality));        
    }
}
