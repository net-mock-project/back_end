using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RescueHub.Domain.Enums;

namespace RescueHub.API.Models.Auth
{
    public class SendOtpRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[\p{L}]+(?:[\s'-][\p{L}]+)*$", ErrorMessage = "Full name is invalid.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and contain exactly 10 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and privacy policy.")]
        public bool IsAgreeTerms { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Ngày sinh không được lớn hơn ngày hiện tại
            if (DateOfBirth > DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Date of birth cannot be in the future.",
                    new[] { nameof(DateOfBirth) });
            }
        }
    }
}