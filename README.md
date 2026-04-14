# Education
Giai đoạn 1: Thiết lập nền tảng (Project Skeleton & Domain)
Đừng vội code UI hay API phức tạp. Hãy bắt đầu bằng việc xây dựng "bộ khung".

Task 1.1: Khởi tạo Solution và dự án theo 4 lớp Clean Architecture: .Domain, .Application, .Infrastructure, .API.
Task 1.2: Định nghĩa các thực thể (Entities) cốt lõi trong .Domain. Nên bắt đầu với nhóm: Course, UserProfile, Role.
Task 1.3: Setup lớp Application cơ bản: Cài đặt AutoMapper (MappingProfile) và các Base DTOs.
Giai đoạn 2: Tầng dữ liệu (Infrastructure & Persistence)
Sau khi có thực thể, bạn cần nơi để lưu trữ chúng.

Task 2.1: Cấu hình DbContext trong .Infrastructure. Sử dụng EF Core với SQL Server.
Task 2.2: Thực hiện bài toán Migration đầu tiên để tạo DB.
Task 2.3: Triển khai Generic Repository hoặc các Repository cụ thể (như CourseRepository) để tránh việc DbContext bị rò rỉ ra lớp ngoài.
"
    Giai đoạn 1: Thiết lập nền tảng
Task 1.1: Phân tách 4 Layer - "Ai biết cái gì?"
Bạn cần phân tách rạch ròi trách nhiệm của từng project:

Domain: Chỉ chứa các Class thuần (C# Classes, Enums). Không có logic tính toán, không có database, không có library lạ.
Application: Đây là nơi bạn định nghĩa các Interfaces. Ví dụ: ICourseRepository nằm ở đây, nhưng "thân xác" (implementation) của nó lại nằm ở Infrastructure. Tại sao? Để lớp Application có thể gọi hàm GetById() mà không cần quan tâm nó lấy từ SQL hay Excel.
Infrastructure: Chứa cấu hình kết nối DB (DbContext) và các logic liên quan đến I/O như gửi Email, ghi Log.
API: Chỉ đóng vai trò tiếp nhận Request, gọi Service từ Application và trả về Response.
Task 1.2: Chi tiết các Thực thể (Entities) - "Luật chơi"
Dựa trên dự án mẫu, đây là những lưu ý quan trọng cho các thực thể của bạn:

Course (Khóa học):
Mối quan hệ: Một khóa học do một 

UserProfile
 tạo ra. Bạn cần trường CreatedBy (int) và một thuộc tính điều hướng (Navigation Property) kiểu 

UserProfile
.
Tương lai: Khóa học sẽ chứa nhiều câu hỏi (Question) và nhiều bài thi (Exam). Hãy chuẩn bị sẵn các ICollection ở đây.
UserProfile (Người dùng):
Đừng nhầm lẫn với bảng Login. Đây là bảng chứa thông tin hiển thị (DisplayName, ProfileImageUrl).
Trường AdObjId: SmartCertify dùng trường này để liên kết với Azure AD B2C. Dù bạn dùng hệ thống Login nào, hãy luôn có một chuỗi Unique ID từ hệ thống đó để map vào đây.
Role (Vai trò):
Phân chia vai trò như: Administrator, Instructor, Student.
Task 1.3: Application Setup - "Chuẩn bị đường ống"
Base DTO (Data Transfer Object): Hãy tạo các API Response chuẩn. Ví dụ: BaseResponse<T> chứa IsSuccess, Message, và Data.
AutoMapper: Khi tạo CourseDto, hãy đảm bảo nó che giấu các thông tin nhạy cảm của Entity (như ID của người tạo nếu không cần thiết).
Giai đoạn 2: Tầng dữ liệu (Infrastructure & Persistence)
Task 2.1: Cấu hình DbContext - "Trái tim của Persistence"
Trong dự án này, việc cấu hình Fluent API trong 

OnModelCreating
 rất quan trọng:

Gợi ý: Hãy dùng HasDefaultValueSql("getdate()") cho các trường ngày tháng. Điều này giúp Database tự quản lý thời gian khởi tạo thay vì bạn phải gán tay trong code C#.
Delete Behavior: Hãy cân nhắc dùng DeleteBehavior.ClientSetNull thay vì Cascade. Ví dụ: Khi xóa một User, đừng để nó tự động xóa sạch các Khóa học mà User đó đã tạo (dễ mất mát dữ liệu quan trọng).
Task 2.2: Migration - "Dấu chân lịch sử"
Khi bạn chạy Migration đầu tiên:

Lưu ý: Lớp khởi tạo (Program.cs ở API) cần biết ConnectionString. Bạn nên để nó trong appsettings.json.
Đánh giá: Nếu Migration tạo ra một file ..._InitialCreate.cs với đầy đủ liên kết khóa ngoại (Foreign Keys) chính xác như thiết kế Entity, nghĩa là bạn đã thành công ở Task 1.2.
Task 2.3: Repository Pattern - "Lớp bảo vệ cuối cùng"
Dự án SmartCertify mẫu chia Repository rất rõ ràng:

Generic Repository: Chứa các hàm dùng chung như GetByIdAsync, ListAllAsync.
Specific Repository (vd: CourseRepository): Tại sao cần? Vì có những logic lọc dữ liệu phức tạp chỉ dành riêng cho khóa học (ví dụ: Lấy khóa học kèm theo danh sách câu hỏi đã được sắp xếp). Generic Repository sẽ không gánh được các logic đặc thù này.

"


Giai đoạn 3: Module Quản lý Khóa học (Module đầu tiên)
Hãy chọn module Course làm module "mẫu" để thông luồng từ Database lên API.

Task 3.1: Viết Interface và Service cho Course (ICourseService, CourseService) trong lớp .Application.
Task 3.2: Cài đặt FluentValidation để kiểm tra dữ liệu đầu vào.
Task 3.3: Viết Controller cho Course với đầy đủ các phương thức: GET (danh sách/chi tiết), POST (tạo), PUT (cập nhật), DELETE.
Task 3.4: Cấu hình Swagger/Scalar trong dự án API để test các endpoint này.
Giai đoạn 4: Module Thi cử & Câu hỏi (Logic phức tạp)
Đây là phần lõi của ứng dụng (Quiz/Exam).

Task 4.1: Hoàn thiện thực thể Exam, Question, Choice. Chú ý quan hệ 1-n giữa Question và Choice.
Task 4.2: Viết logic chấm điểm hoặc nộp bài thi (Submit Exam) trong Service.
Task 4.3: Xây dựng API quản lý câu hỏi dành cho giảng viên/admin.
Giai đoạn 5: Định danh & Bảo mật (Security & Identity)
Phần này khó hơn vì liên quan đến Azure AD B2C hoặc JWT.

Task 5.1: Tích hợp xác thực (Authentication). Nếu bạn chưa muốn dùng Azure AD B2C ngay, hãy bắt đầu bằng JWT nội bộ đơn giản.
Task 5.2: Phân quyền (Authorization) dựa trên Role (Admin/Instructor/Student).
Task 5.3: API quản lý thông tin cá nhân (UserProfile).
Giai đoạn 6: Tính năng nâng cao & Azure Services
Khi hệ thống đã chạy ổn định, bạn mới thêm các "trang sức" này vào.

Task 6.1: File Storage: Tích hợp Azure Blob Storage để tải lên ảnh bìa khóa học hoặc chứng chỉ.
Task 6.2: Background Services: Viết Worker tự động xử lý thông báo hoặc dọn dẹp dữ liệu rác.
Task 6.3: Azure Functions: Tích hợp gửi Email tự động khi người dùng hoàn thành khóa học.
Một vài lời khuyên nhỏ khi bạn code:
Code đến đâu test đến đó: Mỗi khi xong 1 Controller, hãy dùng Swagger hoặc Postman để đảm bảo dữ liệu vào/ra đúng ý.
Đừng copy-paste cả file: Hãy copy từng phương thức một, đọc hiểu nó làm gì rồi mới paste vào dự án mới của bạn.
Tập trung vào .Application: Đây là nơi chứa "linh hồn" của dự án (Business Logic). Hãy dành nhiều thời gian để tổ chức Service và DTO ở đây thật sạch sẽ.
Bạn muốn tôi đi sâu vào chi tiết cách triển khai Task nào nhất trong các bước trên? Tôi có thể hướng dẫn bạn code mẫu cho Task đó.


