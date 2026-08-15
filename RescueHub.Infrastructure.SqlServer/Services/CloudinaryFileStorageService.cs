using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Infrastructure.SqlServer.Configurations;

namespace RescueHub.Infrastructure.SqlServer.Services;

public class CloudinaryFileStorageService : IFileStorageService
{
    // SDK Cloudinary dùng để upload và quản lý ảnh
    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorageService(
        IOptions<CloudinaryOptions> options)
    {
        // Lấy cấu hình Cloudinary đã bind từ User Secrets / configuration
        var cloudinaryOptions = options.Value;

        // Tạo thông tin xác thực để kết nối Cloudinary
        var account = new Account(
            cloudinaryOptions.CloudName,
            cloudinaryOptions.ApiKey,
            cloudinaryOptions.ApiSecret);

        // Khởi tạo Cloudinary client
        _cloudinary = new Cloudinary(account);

        // Sử dụng HTTPS cho URL trả về
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadAvatarAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken)
    {
        // Mỗi user sử dụng một PublicId cố định cho avatar
        var publicId = $"rescuehub/avatars/{userId}";

        // Cấu hình thông tin ảnh cần upload lên Cloudinary
        var uploadParams = new ImageUploadParams
        {
            // File ảnh được gửi lên dưới dạng Stream
            File = new FileDescription(
                fileName,
                fileStream),

            // Định danh ảnh trên Cloudinary
            PublicId = publicId,

            // Ghi đè avatar cũ nếu PublicId đã tồn tại
            Overwrite = true,

            // Yêu cầu làm mới cache CDN sau khi ghi đè ảnh
            Invalidate = true
        };

        // Upload ảnh lên Cloudinary
        var result = await _cloudinary.UploadAsync(
            uploadParams,
            cancellationToken);

        // Nếu Cloudinary trả lỗi thì dừng xử lý
        if (result.Error != null)
        {
            throw new InvalidOperationException(
                result.Error.Message);
        }

        // Trả về URL HTTPS của ảnh để lưu vào User.ProfileUrl
        return result.SecureUrl.AbsoluteUri;
    }
}