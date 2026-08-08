# Task summary: Disaster Relief Coordination Platform
Mock Project | Group 5 persons | 3 Sprint 

## Contexts
Sau mỗi đợt thiên tai (bão, lũ lụt, sạt lở...), việc kết nối giữa các điểm cần cứu trợ, đội tình nguyện viên và nguồn vật tư (thực phẩm, thuốc men, nhu yếu phẩm) hiện phần lớn vẫn diễn ra thủ công qua mạng xã hội, gây chồng chéo, chậm trễ và phân bổ nguồn lực không đồng đều. Nhóm sẽ xây dựng một nền tảng web giúp chính quyền địa phương, tổ chức thiện nguyện và tình nguyện viên phối hợp cứu trợ nhanh chóng, minh bạch và có dữ liệu theo dõi thời gian thực.

## Objectives
- Rút ngắn thời gian từ khi tiếp nhận yêu cầu cứu trợ đến khi được xử lý.
- Tối ưu việc phân bổ tình nguyện viên và vật tư dựa trên vị trí địa lý và mức độ ưu tiên.
- Cung cấp bức tranh tổng thể, theo thời gian thực cho ban điều phối để ra quyết định.
- Đảm bảo minh bạch trong quá trình tiếp nhận và phân phối nguồn lực cứu trợ.

## Auth
- Quản trị viên hệ thống (Admin) — quản lý người dùng, khu vực, phân quyền.
- Điều phối viên (Coordinator) — duyệt yêu cầu, phân công tình nguyện viên/vật tư.
- Tình nguyện viên (Volunteer) — đăng ký kỹ năng, nhận nhiệm vụ, cập nhật tiến độ.
- Người yêu cầu cứu trợ / Đại diện điểm nóng (Requester) — tạo yêu cầu, theo dõi trạng thái.

## Functional Requirement
- Quản lý tình nguyện viên & kỹ năng
    - Đăng ký hồ sơ tình nguyện viên kèm kỹ năng, khu vực hoạt động, thời gian rảnh.
    - Xác thực/duyệt hồ sơ tình nguyện viên bởi điều phối viên.

- Tiếp nhận & xử lý yêu cầu cứu trợ
    - Tạo yêu cầu cứu trợ kèm vị trí (bản đồ), loại nhu cầu, mức độ khẩn cấp, số người ảnh hưởng.
    - Bộ máy gợi ý/ghép cặp (matching) tình nguyện viên và vật tư phù hợp theo kỹ năng, khoảng cách, mức ưu tiên.
    - Theo dõi vòng đời yêu cầu: Mới → Đã phân công → Đang xử lý → Hoàn tất.

- Quản lý kho vật tư & quyên góp
    - Quản lý tồn kho vật tư theo từng điểm tập kết/kho.
    - Ghi nhận quyên góp, xuất/nhập kho khi phân bổ cho yêu cầu cứu trợ.

- Bản đồ & Bảng điều khiển thời gian thực
    - Bản đồ hiển thị các yêu cầu đang mở, vị trí tình nguyện viên, kho vật tư.
    - Dashboard thống kê: số yêu cầu theo trạng thái, thời gian xử lý trung bình, khu vực nóng.

- Thông báo & Báo cáo
    - Thông báo trong ứng dụng/email khi có phân công mới hoặc cập nhật trạng thái.
    - Xuất báo cáo tổng kết theo đợt cứu trợ (PDF/Excel).

## Non-Functional Requirement
- Cập nhật trạng thái gần thời gian thực (khuyến khích dùng SignalR/WebSocket).
- Phân quyền chi tiết theo vai trò (RBAC) trên cả API và giao diện.
- Chịu tải hợp lý khi nhiều người dùng cùng thao tác trong tình huống khẩn cấp.
- Nhật ký (audit log) cho các thao tác duyệt/phân công quan trọng.

## TechStacks

| Hạng mục | Công nghệ đề xuất |
|---|---|
| Frontend| Angular + TypeScript, gọi API qua REST (JSON), quản lý state phù hợp với framework (NgRx/Redux/Pinia tuỳ chọn) |
|Backend | ASP.NET Core Web API (.NET 8), kiến trúc N-layer hoặc Clean Architecture, Entity Framework Core |
| Database | SQL Server (hoặc PostgreSQL) cho dữ liệu quan hệ; có thể bổ sung Redis cho cache nếu làm phần mở rộng |
| Auth | JWT (access token + refresh token), phân quyền theo Role/Claim |
| Test | xUnit/NUnit cho backend; Jest/Karma/Vitest cho frontend (tuỳ framework) |
| Implement | Docker hoá ứng dụng; triển khai thử nghiệm trên một VPS/Cloud miễn phí (Render, Azure Free Tier, Railway...) |
| Tools | Git + GitHub/GitLab, quy ước nhánh (Gitflow đơn giản: main/dev/feature)|

## Kế hoạch triển khai theo Sprint ( 3 Sprint)

**Sprint 1 — Tuần 1:** Phân tích & Thiết kế nền tảng
- Mục tiêu:
    - Phân tích yêu cầu chi tiết, xây dựng Backlog và User Story cho toàn dự án.
    - Thiết kế cơ sở dữ liệu (ERD), thiết kế API contract (Swagger/OpenAPI).
    - Thiết kế UI/UX (wireframe → mockup) cho các màn hình chính.
    - Khởi tạo mã nguồn: cấu trúc dự án Backend (.NET) và Frontend, thiết lập xác thực (JWT) cơ bản.

- Sản phẩm bàn giao:
    - Tài liệu đặc tả yêu cầu (SRS) & Product Backlog.
    - Sơ đồ ERD, API contract nháp.
    - Bộ mockup UI cho các màn hình chính.
    - Repository khởi tạo, cấu trúc project chạy được (empty-state).

**Sprint 2 — Tuần 2:** Phát triển tính năng cốt lõi
- Mục tiêu:
    - Xây dựng các module chức năng chính theo mục IV (CRUD nghiệp vụ, quy trình xử lý chính).
    - Tích hợp Frontend với Backend qua API thực tế (không còn mock data).
    - Viết unit test cho các nghiệp vụ quan trọng ở tầng Service/Backend.
    - Demo nội bộ cuối sprint (Sprint Review) để nhận phản hồi và điều chỉnh.

- Sản phẩm bàn giao:
    - Các module chức năng chính hoạt động end-to-end.
    - Bộ test case + kết quả unit test.
    - Biên bản Sprint Review/Retrospective.

**Sprint 3 — Tuần 3:** Hoàn thiện, kiểm thử & Triển khai
- Mục tiêu:
    - Phát triển các tính năng nâng cao/mở rộng (mục VIII).
    - Kiểm thử tích hợp (integration test) và kiểm thử chấp nhận người dùng (UAT).
    - Tối ưu hiệu năng, xử lý lỗi, hoàn thiện UI/UX.
    - Đóng gói, triển khai (deploy) và chuẩn bị tài liệu, kịch bản demo cuối kỳ.

- Sản phẩm bàn giao:
    - Ứng dụng hoàn chỉnh được triển khai (deploy) và truy cập được.
    - Tài liệu hướng dẫn cài đặt/vận hành, báo cáo cuối kỳ.
    - Slide + kịch bản demo trước hội đồng.

## Gợi ý tính năng/kỹ thuật nâng cao (tuỳ chọn)
Nhóm có thể lựa chọn thêm 2-3 hạng mục dưới đây để tăng chiều sâu kỹ thuật của dự án, tuỳ theo năng lực và thời gian còn lại:
- Tích hợp bản đồ (Leaflet/Google Maps) để chọn và hiển thị vị trí.
- Thuật toán ghép cặp nâng cao có trọng số (kỹ năng, khoảng cách, tải công việc hiện tại).
- Thông báo thời gian thực bằng SignalR.
- Containerize bằng Docker và pipeline CI/CD cơ bản.