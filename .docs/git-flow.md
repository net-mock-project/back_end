## Quy chuẩn Git Workflow

### 1. Quy tắc đặt tên Nhánh (Branch Naming)
Luôn tạo branch mới từ main. Tên branch viết thường, cách nhau bằng dấu gạch ngang (-), kèm theo ID của Issue trên GitHub.
**Cú pháp:** <type>/<issue-id>-<tên-ngắn-gọn>

**Các type chính:**
| Type | Mục đích |
|------|----------|
| feat | Tính năng mới |
| fix | Sửa lỗi bug |
| hotfix | Sửa lỗi khẩn cấp trên production |
| chore | Cấu hình, cài thư viện |
| docs | Cập nhật tài liệu |

**Ví dụ:**
- feat/seat-booking — Tính năng đặt ghế
- fix/ui-checkout — Sửa lỗi giao diện thanh toán
- hotfix/payment-crash — Sửa gấp lỗi thanh toán

### 2. Quy tắc Commit Message
Sử dụng tiếng Anh, ngắn gọn. Không viết hoa chữ cái đầu tiên sau dấu hai chấm. Mỗi commit chỉ làm một việc duy nhất.
**Cú pháp:** <type>(<scope/optional>): <thông điệp>

**Các type phổ biến:**
| Type | Mục đích |
|------|----------|
| feat | Thêm tính năng mới |
| fix | Sửa lỗi |
| refactor | Chỉnh sửa code gọn hơn, không đổi tính năng |
| chore | Cấu hình (Docker, package.json...) |
| docs | Cập nhật tài liệu, README |
| test | Viết hoặc sửa test |
| style | Format code, lint (không đổi logic) |
| ci | Cấu hình CI/CD pipeline |
| perf | Tối ưu hiệu năng |

**Ví dụ:**
- feat(api): add row locking for booking
- fix(ui): fix seat map not updating on mobile
- chore: add rabbitmq to docker-compose
- docs: update API endpoint in README

**Ví dụ:**
- feat(api): add row locking for booking
- fix(ui): fix seat map not updating on mobile
- chore: add rabbitmq to docker-compose
- docs: update API endpoint in README

**Không làm:**
- feat(api): add booking + fix login + update readme → Tách thành 3 commit riêng biệt

### 3. Luồng làm việc (Team Workflow)
**Bước 1** — Tạo branch mới
Luôn đảm bảo main đã được cập nhật mới nhất trước khi tạo branch.
- git checkout main
- git pull origin main
- git checkout -b feat/seat-booking
​
**Bước 2 **— Code & Commit
Code xong tính năng (hoặc một phần tính năng) thì commit lại. Mỗi commit một việc, message rõ ràng.

**Bước 3** — Đồng bộ main trước khi tạo PR
Trước khi push, luôn cập nhật main mới nhất để tránh conflict:
- git checkout main
- git pull origin main
- git checkout feat/seat-booking
- git rebase main​
Giải quyết conflict (nếu có), sau đó push lên GitHub.

**Bước 4** — Tạo Pull Request (PR)
Push branch lên GitHub và tạo PR gộp vào main. Mô tả PR theo template:
- Thay đổi gì? — Mô tả ngắn gọn
- Mô tả cách thức hoạt động? - Workflow, logic, API, DB...
- Chụp ảnh màn hình những thay đổi về UI (nếu có), Testcase trong Postman (nếu có).

Lưu ý trước khi PR:
- Đã rebase main mới nhất.
- Không có code thừa (console.log, comment test, file cũ...).
- Code chạy tốt, đúng trách nhiệm, không gây lỗi cho các tính năng khác.

**Bước 5** — Review & Merge
- Yêu cầu ít nhất 1 thành viên khác Approve trước khi merge.
- Không ai tự merge PR của chính mình.
- Sử dụng Squash and Merge để giữ history gọn gàng.
- Xóa branch sau khi merge (tick "Delete branch after merge" trên GitHub).
- Trước khi Approve, reviewer cần pull branch về local, chạy thử và đọc code để hiểu logic. Comment nếu có vấn đề, không approve vội.

### 4. Thiết lập GitHub Repository
Cấu hình Branch Protection Rules cho nhánh main:
- Require at least 1 approval before merge
- Block force push to main
- Auto-delete head branches after merge
- Require status checks to pass (nếu có CI/CD)

### 5. Quy tắc .gitignore
Không commit các file sau lên repository (đã có rồi):
- File môi trường: .env, .env.local
- Dependencies: node_modules/, vendor/
- Build output: dist/, build/, .next/
- IDE config: .idea/, .vscode/settings.json