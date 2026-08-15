using System.ComponentModel.DataAnnotations;

namespace RescueHub.API.Models.Users;

public class UpdateAvatarRequest : IValidatableObject
{
    [Required(ErrorMessage = "Avatar is required.")]
    public IFormFile Avatar { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        // Không cho upload file rỗng
        if (Avatar == null || Avatar.Length == 0)
        {
            yield return new ValidationResult(
                "Avatar file cannot be empty.",
                new[] { nameof(Avatar) });

            yield break;
        }

        // Giới hạn dung lượng tối đa 5 MB
        const long maxFileSize = 5 * 1024 * 1024;

        if (Avatar.Length > maxFileSize)
        {
            yield return new ValidationResult(
                "Avatar file size must not exceed 5 MB.",
                new[] { nameof(Avatar) });
        }

        // Chỉ chấp nhận JPG/JPEG và PNG
        var allowedContentTypes = new[]
        {
            "image/jpeg",
            "image/png"
        };

        if (!allowedContentTypes.Contains(
                Avatar.ContentType,
                StringComparer.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "Avatar must be a JPG, JPEG, or PNG image.",
                new[] { nameof(Avatar) });
        }
    }
}