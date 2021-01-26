using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoronaTest.Core.Validations
{
    /// <summary>
    /// Validation for sozial security number
    /// </summary>
    
    public class SSNrValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string ssnr))
            {
                throw new ArgumentException("Value is not a string", nameof(value));
            }

            if (!CheckSsnr(ssnr))
            {
                return new ValidationResult($"Sozialversicherungsnummer {ssnr} ist ungültig");
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public static bool CheckSsnr(string ssnr)
        {
            if (ssnr == null || ssnr.Length != 10)
            {
                return false;
            }

            var digitweights = new int[] { 3, 7, 9, 0, 5, 8, 4, 2, 1, 6 };
            int productSum = 0;

            for (int i = 0; i < ssnr.Length; i++)
            {
                if (ssnr[i] < '0' || ssnr[i] > '9')
                {
                    return false;
                }

                productSum += (ssnr[i] - '0') * digitweights[i];
            }

            var checkdigit = productSum % 11;

            if ((ssnr[3] - '0') != checkdigit)
            {
                return false;
            }

            return true;
        }
    }
}
