namespace RescueHub.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Bắt đầu một luồng cho một usecase nghiệp vụ
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task BeginTransactionAsync(
            CancellationToken cancellationToken);

        /// <summary>
        /// Lưu thay đổi từ Ram vào database thông qua EF Core.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(
            CancellationToken cancellationToken);

        /// <summary>
        /// Chốt đữ liệu khi có một transaction tồn tại, chỉ save thì vẫn chưa đẩy được vào database.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CommitAsync(
            CancellationToken cancellationToken);

        /// <summary>
        /// Roll back tới đầu trước begin transaction.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RollbackAsync(
            CancellationToken cancellationToken);
    }
}
