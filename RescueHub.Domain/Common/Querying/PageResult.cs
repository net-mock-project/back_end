namespace RescueHub.Domain.Common.Querying
{
    /// <summary>
    /// Container trung lập cho một trang dữ liệu, bao gồm danh sách các mục và thông tin phân trang.
    /// Dùng cho tầng Domain và Application, không phụ thuộc framework.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Triển khai một đối tượng PagedResult với danh sách các mục và thông tin phân trang.
        /// </summary>
        public IReadOnlyList<T> Items { get; }

        /// <summary>
        /// Tổng số lượng các mục trong tập dữ liệu gốc, không chỉ số lượng trong trang hiện tại.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Khởi tạo một đối tượng PagedResult với danh sách các mục và tổng số lượng.
        /// </summary>
        /// <param name="items">Danh sách các mục trong trang hiện tại</param>
        /// <param name="totalCount">Tổng số lượng các mục trong tập dữ liệu gốc</param>
        public PagedResult(IReadOnlyList<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
