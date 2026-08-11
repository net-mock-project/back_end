using System.Text.Json.Serialization;

namespace RescueHub.Application.Contracts.Querying
{
    /// <summary>
    /// Kết quả phân trang cho một tập hợp các mục: danh sách các bản ghi kèm metaData.
    /// Được các Query handler dựng sẵn để trả về cho các API endpoint.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của các mục trong trang</typeparam>
    public class PaginationResponse<T>
    {
        /// <summary>
        /// Danh sách các mục trong trang hiện tại.
        /// </summary>
        [JsonPropertyName("items")]
        public IReadOnlyList<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Số trang hiện tại được hiển thị.
        /// </summary>
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Tổng số mục trên mỗi trang.
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số mục trong toàn bộ tập hợp dữ liệu (không chỉ trang hiện tại).
        /// </summary>
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Tống số trang trong toàn bộ tập dữ liệu
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Số trang hiện tại có trang trước hay không. Nếu PageNumber > 1 thì có trang trước.
        /// </summary>
        [JsonPropertyName("hasPrevious")]
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// Số trang hiện tại có trang sau hay không. Nếu PageNumber < TotalPages thì có trang sau.
        /// </summary>
        [JsonPropertyName("hasNext")]
        public bool HasNext => PageNumber < TotalPages;

        /// <summary>
        /// Hàm khởi tạo mặc định cho PaginationResponse.
        /// </summary>
        public PaginationResponse() { }

        /// <summary>
        /// Hàm khởi tạo cho PaginationResponse với các tham số cụ thể.
        /// </summary>
        /// <param name="items">Danh sách các mục trong trang hiện tại</param>
        /// <param name="totalCount">Tổng số mục trong toàn bộ tập hợp dữ liệu</param>
        /// <param name="pageNumber">Số trang hiện tại</param>
        /// <param name="pageSize">Tổng số mục trên mỗi trang</param>
        public PaginationResponse(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
