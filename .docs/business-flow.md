# Disaster Relief Coordination Platform

## Business Process Specification

Version: 1.0 (BA Fixed Edition)

---

## Overview

The Disaster Relief Coordination Platform records the complete lifecycle of a relief request from submission to resolution.
Each request has a unique ID.
Every operation performed on a request creates an immutable Event.
SupplyChainEvents are append-only and cannot be modified or deleted.
The integrity of the event timeline is ensured using a SHA-256 Hash Chain mechanism.

---

## I. Authentication
### 1. Đăng ký
Người dùng đăng ký tài khoản bằng các thông tin cơ bản gồm họ tên, số điện thoại, email và mật khẩu. Sau khi hoàn tất, hệ thống sẽ gửi mã OTP qua SMS để xác thực số điện thoại. Người dùng nhập OTP để kích hoạt tài khoản, sau đó được chuyển trở lại trang đăng nhập.
Mỗi tài khoản mới khi được tạo sẽ có Role mặc định là Requester, giúp người dùng có thể sử dụng ngay các chức năng cơ bản của hệ thống mà không cần trải qua quy trình xét duyệt.

### 2. Đăng nhập
Người dùng đăng nhập bằng tài khoản đã đăng ký. Đối với lần đăng nhập đầu tiên, hệ thống sẽ yêu cầu người dùng bổ sung các thông tin còn thiếu như ảnh đại diện, địa chỉ, tỉnh thành và cho phép cấp quyền vị trí.
Việc yêu cầu vị trí nhằm phục vụ các chức năng như tìm trạm tiếp tế gần nhất, xác định người dùng có nằm trong vùng ảnh hưởng của một đợt cứu trợ hay không và gửi các thông báo theo khu vực.
Sau khi hoàn tất, người dùng được chuyển vào hệ thống và sử dụng các chức năng tương ứng với Role hiện tại.

### 3. Đổi mật khẩu
Người dùng có thể đổi mật khẩu trong mục Profile. Hệ thống yêu cầu nhập mật khẩu hiện tại, mật khẩu mới và xác nhận mật khẩu mới. Nếu thông tin hợp lệ, mật khẩu sẽ được cập nhật ngay.

### 4. Quên mật khẩu
Người dùng chọn chức năng Quên mật khẩu tại trang đăng nhập và nhập số điện thoại đã đăng ký. Hệ thống gửi OTP để xác thực. Sau khi xác thực thành công, người dùng được phép đặt lại mật khẩu mới và quay lại đăng nhập.

## II. User
### 5. Chỉnh sửa Profile
Người dùng truy cập trang Profile từ menu cá nhân và chọn biểu tượng chỉnh sửa để cập nhật thông tin như họ tên, email, ảnh đại diện hoặc địa chỉ.
Thông tin này không chỉ phục vụ hiển thị hồ sơ mà còn được sử dụng trong các nghiệp vụ điều phối và xác định vị trí khi tham gia cứu trợ.

### 6. Quản lý thông báo
Người dùng có thể truy cập danh sách thông báo thông qua biểu tượng Chuông trên thanh điều hướng.
Hệ thống sẽ gửi thông báo trong các trường hợp như:
- Đơn quyên góp được duyệt hoặc bị từ chối.
- Hồ sơ Volunteer được duyệt hoặc bị từ chối.
- Có yêu cầu cứu trợ mới trong phạm vi khu vực của người dùng.
- Được Coordinator mời tham gia nhiệm vụ.
- Trạng thái nhiệm vụ thay đổi hoặc hoàn thành.
- Các thay đổi liên quan đến yêu cầu cứu trợ mà người dùng đã tạo.
Người dùng có thể chọn từng thông báo để xem chi tiết dưới dạng Modal hoặc được điều hướng đến màn hình tương ứng.

## III. Relief Request
### 7. Quản lý báo cáo hỗ trợ cứu trợ
Khi phát hiện khu vực cần cứu trợ, người dùng có thể tạo một Yêu cầu cứu trợ bằng cách nhập các thông tin cơ bản như:
- Địa điểm xảy ra.
- Tiêu đề.
- Mô tả tình hình.
- Mô tả nhu cầu cứu trợ.
- Ước lượng số người bị ảnh hưởng.
- Ước lượng phạm vi ảnh hưởng.
Mục tiêu của bước này là để người dân có thể gửi thông tin nhanh nhất có thể mà không cần nhập quá nhiều dữ liệu.
Sau khi gửi, yêu cầu sẽ ở trạng thái Pending và chờ Coordinator xem xét.
Coordinator sẽ là người xác minh và chuẩn hóa lại thông tin như:
- Điều chỉnh tiêu đề.
- Chỉnh sửa mô tả nếu cần.
- Bổ sung thời gian bắt đầu và kết thúc đợt cứu trợ.
- Điều chỉnh các thông tin nghiệp vụ khác trước khi phê duyệt.
Chỉ sau khi được Approve, yêu cầu mới chính thức trở thành một đợt cứu trợ để Coordinator bắt đầu tạo các nhiệm vụ triển khai. Sau khi Relief Request được phê duyệt, hệ thống tự động đề xuất danh sách Volunteer và Warehouse phù hợp dựa trên kỹ năng, khoảng cách và mức độ ưu tiên để Coordinator lựa chọn. Sau khi được Approve hệ thống tự động phát thông báo đến toàn thể người dùng nằm trong phạm vi ảnh hưởng.
Người dùng có thể theo dõi toàn bộ tiến trình xử lý tại mục Yêu cầu cứu trợ của tôi trong Profile.
Để tránh việc một người gửi quá nhiều yêu cầu trùng lặp trong cùng thời điểm, mỗi người chỉ được tồn tại một yêu cầu cứu trợ đang hoạt động. Trong thời gian chờ duyệt, người dùng vẫn có thể chỉnh sửa hoặc hủy yêu cầu.

## IV. Donation
### 8. Quản lý quyên góp cứu trợ
Người dùng muốn quyên góp có thể chọn chức năng Quyên góp tại các màn hình của hệ thống.
Người dùng lựa chọn các vật tư cần quyên góp, sau đó hệ thống sẽ tự động xác định trạm tiếp tế gần nhất để tiếp nhận nhằm giảm thời gian vận chuyển và thuận tiện cho Coordinator quản lý kho.
Sau khi tạo, đơn quyên góp chưa được duyệt ngay mà sẽ ở trạng thái Pending. Coordinator chỉ phê duyệt sau khi đã thực sự tiếp nhận vật tư tại kho.
Trong thời gian chờ duyệt, người dùng vẫn có thể chỉnh sửa hoặc hủy đơn quyên góp nếu tạo nhầm.
Trong thiết kế hệ thống, thông tin vật tư không được lưu trực tiếp trong Donation mà được quản lý thông qua các Transaction phát sinh sau khi Coordinator xác nhận tiếp nhận vật tư. Điều này giúp toàn bộ quá trình nhập kho và xuất kho đều được theo dõi thống nhất bằng Transaction.

## V. Volunteer
### 9. Quản lý hồ sơ Volunteer
Người dùng muốn trở thành Volunteer sẽ gửi hồ sơ Volunteer đến Coordinator để xét duyệt.
Khác với CV truyền thống, hồ sơ Volunteer trong hệ thống không phải là một tệp tin tải lên mà được hình thành từ các thông tin như:
- Kinh nghiệm.
- Danh sách kỹ năng.
- Lịch sử tham gia các đợt cứu trợ.
Sau khi Coordinator phê duyệt, người dùng sẽ được chuyển sang Role Volunteer và có thể sử dụng toàn bộ chức năng dành cho Volunteer.
Trong quá trình hoạt động, mỗi khi Volunteer hoàn thành một đợt cứu trợ, lịch sử tham gia sẽ được bổ sung vào hồ sơ nhằm phản ánh đúng năng lực và kinh nghiệm thực tế của Volunteer theo thời gian.

### 10. Tham gia cứu trợ
Volunteer vẫn được sử dụng toàn bộ chức năng của Requester.
Đối với mỗi đợt cứu trợ đã được phê duyệt, Volunteer có thể truy cập trang chi tiết và chọn I'm Available để thông báo rằng mình sẵn sàng tham gia.
Nút này xuất hiện với tất cả Volunteer nhưng chỉ chấp nhận những Volunteer đáp ứng điều kiện về khu vực và phạm vi hỗ trợ của đợt cứu trợ. Điều này giúp Coordinator chỉ làm việc với những Volunteer thực sự có khả năng tham gia thay vì toàn bộ Volunteer trong hệ thống.
Việc chọn I'm Available không đồng nghĩa với việc được giao nhiệm vụ mà chỉ thể hiện sự sẵn sàng tham gia và giúp Coordinator dễ dàng lựa chọn những Volunteer phù hợp trong quá trình điều phối.

### 11. Quản lý Task
Sau khi Coordinator tạo các nhiệm vụ cho một đợt cứu trợ, Volunteer đã đăng ký tham gia có thể:
- Chủ động đề xuất nhận nhiệm vụ.
- Hoặc nhận lời mời trực tiếp từ Coordinator.
- Đối với lời mời từ Coordinator, Volunteer có thể Chấp nhận hoặc Từ chối.
Khi đã nhận nhiệm vụ, Volunteer có thể theo dõi toàn bộ danh sách nhiệm vụ của mình tại trang Quản lý nhiệm vụ. Đây cũng là nơi tổng hợp toàn bộ các nhiệm vụ đã tham gia trong nhiều đợt cứu trợ khác nhau.
Trong trường hợp không thể tiếp tục thực hiện nhiệm vụ, Volunteer có thể gửi yêu cầu hủy. Tuy nhiên việc hủy chỉ có hiệu lực sau khi Coordinator chấp thuận nhằm đảm bảo quá trình điều phối không bị gián đoạn.

## VI. Coordinator
### 12. Quản lý Volunteer khu vực
Coordinator quản lý toàn bộ Volunteer thuộc khu vực phụ trách.
Ngoài các chức năng CRUD cơ bản, Coordinator còn chịu trách nhiệm xét duyệt hồ sơ Volunteer, theo dõi thống kê và đánh giá tình hình hoạt động của Volunteer trong khu vực.
Danh sách Volunteer đã chọn I'm Available sẽ là nguồn chính để Coordinator tìm kiếm và phân công nhân sự cho từng nhiệm vụ.

### 13. Quản lý yêu cầu cứu trợ khu vực
Coordinator tiếp nhận toàn bộ các yêu cầu cứu trợ đang chờ xử lý trong khu vực.
Sau khi xác minh thông tin và phê duyệt, Coordinator sẽ trực tiếp điều phối đợt cứu trợ thông qua việc:
- Chuẩn hóa thông tin yêu cầu.
- Tạo các Task.
- Chỉnh sửa hoặc bổ sung Task khi cần.
- Phân công Volunteer phù hợp.
- Theo dõi tiến độ thực hiện.
Hẹ thống tự động gợi ý danh sách tình nguyện viên phù hợp với từng nhiệm vụ dựa trên mức độ ưu tiên, khoảng cách, kĩ năng, đặc biệt là mức độ sẵn sàng tham gia.
Sau khi toàn bộ nhiệm vụ hoàn thành, Coordinator kết thúc đợt cứu trợ và có thể xuất báo cáo tổng hợp của đợt cứu trợ phục vụ lưu trữ hoặc thống kê.

### 14. Quản lý khu vật tư
Coordinator quản lý toàn bộ kho vật tư trong khu vực.
- Các chức năng bao gồm:
- Quản lý danh mục kho.
- Quản lý vật tư.
- Tiếp nhận và duyệt các đơn quyên góp.
Quản lý các Transaction nhập kho và xuất kho phục vụ từng nhiệm vụ cứu trợ.
Trong thiết kế hệ thống, mọi thay đổi về số lượng vật tư đều được thực hiện thông qua Transaction. WarehouseInventory chỉ lưu số lượng hiện tại, còn toàn bộ lịch sử nhập và xuất đều được lưu trong Transaction nhằm đảm bảo khả năng truy vết.

## VII. Admin
### 15. Quản lý User
Admin quản lý toàn bộ người dùng trong hệ thống bao gồm các chức năng CRUD cơ bản.
Ngoài ra Admin còn có quyền khóa hoặc mở khóa tài khoản đối với các trường hợp vi phạm, spam hoặc có hành vi không phù hợp.

### 16. Quản lý Coordinator và khu vực
Admin chịu trách nhiệm tạo Coordinator, phân công Coordinator phụ trách từng khu vực và thay đổi khi cần thiết nhằm đảm bảo mỗi khu vực đều có người điều phối.

### 17. Dashboard tổng hợp
Admin theo dõi các số liệu tổng hợp của toàn hệ thống như:
- Số lượng người dùng.
- Volunteer.
- Đợt cứu trợ.
- Quyên góp.
- Kho vật tư.
- Các thống kê khác phục vụ quản lý.

### 18. Audit Log
Admin có thể xem toàn bộ lịch sử thao tác quan trọng của hệ thống để phục vụ việc kiểm tra, truy vết và xử lý các sự cố khi cần thiết.