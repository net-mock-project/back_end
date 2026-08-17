using FluentValidation;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces;
using RescueHub.Domain.Interfaces.Users;

namespace RescueHub.Application.Features.Users.Commands;

public record UpdateAvatarCommand(
    Guid UserId,
    Stream FileStream,
    string FileName
) : IRequest<string?>;

public class UpdateAvatarCommandHandler
    : IRequestHandler<UpdateAvatarCommand, string?>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvatarCommandHandler(
        IFileStorageService fileStorageService,
        IUserService userService,
        IUnitOfWork unitOfWork)
    {
        _fileStorageService = fileStorageService;
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task<string?> Handle(
        UpdateAvatarCommand request,
        CancellationToken cancellationToken)
    {
        // Upload ảnh lên Cloudinary và lấy URL
        var profileUrl = await _fileStorageService.UploadAvatarAsync(
            request.UserId,
            request.FileStream,
            request.FileName,
            cancellationToken);

        await _unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            // Cập nhật đường dẫn avatar của User
            var user = await _userService.UpdateAvatarAsync(
                request.UserId,
                profileUrl,
                cancellationToken);

            if (user == null)
            {
                await _unitOfWork.RollbackAsync(
                    cancellationToken);

                return null;
            }

            // Lưu thay đổi xuống database
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await _unitOfWork.CommitAsync(
                cancellationToken);

            return profileUrl;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    public class UpdateAvatarCommandValidator
        : AbstractValidator<UpdateAvatarCommand>
    {
        private static readonly string[] AllowedExtensions =
        {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

        public UpdateAvatarCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.FileStream)
                .NotNull()
                .WithMessage("Avatar file is required.");

            RuleFor(x => x.FileStream)
                .Must(stream => stream != null && stream.Length > 0)
                .WithMessage("Avatar file cannot be empty.");

            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("File name is required.");

            RuleFor(x => x.FileName)
                .Must(HaveAllowedExtension)
                .WithMessage("Avatar must be a JPG, JPEG, PNG, or WEBP image.");
        }

        private static bool HaveAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            return AllowedExtensions.Contains(
                extension,
                StringComparer.OrdinalIgnoreCase);
        }
    }
}