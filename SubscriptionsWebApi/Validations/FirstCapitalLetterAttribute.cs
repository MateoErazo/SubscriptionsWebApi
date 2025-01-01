﻿using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.Validations
{
    public class FirstCapitalLetterAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) { 
                return ValidationResult.Success; 
            }

            string firstLetter = (value.ToString()[0]).ToString();

            if (firstLetter != firstLetter.ToUpper())
            {
                return new ValidationResult("The first letter should be uppercase.");
            }

            return ValidationResult.Success;
        }
    }
}
