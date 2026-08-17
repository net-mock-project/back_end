using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Infrastructure.SqlServer.Persistence
{
    /// <summary>
    /// Triển khai mô hình Unit of Work để quản lý các thao tác và giao dịch (Transaction) cơ sở dữ liệu.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Khởi tạo một thể hiện mới của lớp <see cref="UnitOfWork"/>.
        /// </summary>
        /// <param name="context">Context của cơ sở dữ liệu Entity Framework Core.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Bắt đầu một giao dịch (Transaction) mới bất đồng bộ.
        /// </summary>
        /// <param name="cancellationToken">Token để hủy thao tác nếu cần.</param>
        /// <returns>Một <see cref="Task"/> đại diện cho thao tác bất đồng bộ.</returns>
        public async Task BeginTransactionAsync(
            CancellationToken cancellationToken)
        {
            await _context.Database
                .BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Lưu tất cả các thay đổi đã thực hiện trong context vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="cancellationToken">Token để hủy thao tác nếu cần.</param>
        /// <returns>Một <see cref="Task"/> đại diện cho thao tác bất đồng bộ.</returns>
        public async Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Xác nhận (Commit) giao dịch hiện tại vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="cancellationToken">Token để hủy thao tác nếu cần.</param>
        /// <returns>Một <see cref="Task"/> đại diện cho thao tác bất đồng bộ.</returns>
        /// <exception cref="InvalidOperationException">Ném ra khi không có giao dịch nào đang hoạt động.</exception>
        public async Task CommitAsync(
            CancellationToken cancellationToken)
        {
            if (_context.Database.CurrentTransaction == null)
                throw new InvalidOperationException(
                    "No active transaction.");

            await _context.Database
                .CurrentTransaction
                .CommitAsync(cancellationToken);
        }

        /// <summary>
        /// Hoàn tác (Rollback) tất cả các thao tác trong giao dịch hiện tại nếu có.
        /// </summary>
        /// <param name="cancellationToken">Token để hủy thao tác nếu cần.</param>
        /// <returns>Một <see cref="Task"/> đại diện cho thao tác bất đồng bộ.</returns>
        public async Task RollbackAsync(
            CancellationToken cancellationToken)
        {
            if (_context.Database.CurrentTransaction == null)
                return;

            await _context.Database
                .CurrentTransaction
                .RollbackAsync(cancellationToken);
        }
    }
}
