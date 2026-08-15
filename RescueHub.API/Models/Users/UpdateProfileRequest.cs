using System.ComponentModel.DataAnnotations;
using RescueHub.Domain.Enums;

namespace RescueHub.API.Models.Users
{
    // Dữ liệu Client gửi lên để cập nhật Profile
    public class UpdateProfileRequest : IValidatableObject
    {
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "Full name must be between 3 and 100 characters.")]
        [RegularExpression(
            @"^[\p{L}]+(?:[\s'-][\p{L}]+)*$",
            ErrorMessage = "Full name is invalid.")]
        public string? FullName { get; set; }

        [RegularExpression(
            @"^0\d{9}$",
            ErrorMessage = "Phone number must start with 0 and contain exactly 10 digits.")]
        public string? Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [EnumDataType(
            typeof(Gender),
            ErrorMessage = "Gender is invalid.")]
        public Gender? Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            // Phải có ít nhất một trường để cập nhật
            if (FullName == null &&
                Phone == null &&
                DateOfBirth == null &&
                Gender == null)
            {
                yield return new ValidationResult(
                    "At least one profile field must be provided.");
            }

            // Ngày sinh không được lớn hơn ngày hiện tại
            if (DateOfBirth.HasValue &&
                DateOfBirth.Value >
                DateOnly.FromDateTime(DateTime.UtcNow))
            {
                yield return new ValidationResult(
                    "Date of birth cannot be in the future.",
                    new[] { nameof(DateOfBirth) });
            }
        }
    }
}