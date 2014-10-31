// because sometimes you have to use MVC :)
 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 
namespace CustomValidationWithDataAnnotations
{
    class Program
    {
        static void Main(string[] args)
        {
            var invalidPerson = new Person()
            {
                FirstName = "Tom",
                Surname = "Tom"
            }; 
 
            var validPerson = new Person()
            {
                FirstName = "Steve", 
                Surname = "Hollidge"
            };
 
            Validate(validPerson);
            Validate(invalidPerson);
        }
 
        private static void Validate(Person person)
        {
            var context = new ValidationContext(person, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(person, context, results, true);
            if (isValid) return;
            foreach (var validationResult in results)
            {
                Console.WriteLine(validationResult.ErrorMessage);
            }
        }
    }
 
    class Person
    {
        [ContainsTheLetterTValidationAttribute]
        public string FirstName { get; set; }
        
        [DifferentValueValidation("FirstName")]
        public string Surname { get; set; }    
    }
 
    public class ContainsTheLetterTValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // check type
            var stringValue = value as string;
            if (stringValue == null)
            {
                return new ValidationResult(string.Format("Unknown type or null value"));
            }
 
            // actual validation comparison occurs here
            return stringValue.ToLowerInvariant().Contains("t")
                ? null
                : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
    
    public class DifferentValueValidationAttribute : ValidationAttribute
    {
        private readonly string other;
        public DifferentValueValidationAttribute(string other)
        {
            this.other = other;
        }
 
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(other);
            if (property == null)
            {
                return new ValidationResult(
                    string.Format("Unknown property: {0}", other)
                );
            }
            var otherValue = property.GetValue(validationContext.ObjectInstance, null);
 
            // actual validation comparison occurs here
            return (value ==otherValue) 
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
                : null;
        }
    }
}