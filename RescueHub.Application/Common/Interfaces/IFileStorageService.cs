namespace RescueHub.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAvatarAsync(
            Guid userId,
            Stream fileStream,
            string fileName,
            CancellationToken cancellationToken);
    }
}