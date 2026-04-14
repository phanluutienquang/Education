# Client Authentication vs User Authentication

## 🎯 Tổng quan

Trong hệ thống API hiện đại, đặc biệt là các platform như Education Management, chúng ta cần **2 lớp xác thực**:

1. **Client Authentication** - Xác thực ỨNG DỤNG nào đang gọi API
2. **User Authentication** - Xác thực NGƯỜI DÙNG nào đang dùng ứng dụng

---

## 📊 So sánh nhanh

| Aspect | Client Authentication | User Authentication |
|--------|----------------------|---------------------|
| **Đối tượng** | Application (Mobile App, Web Portal, Partner API) | Con người (Student, Instructor, Admin) |
| **Credentials** | ClientId + ApiSecret | Username/Email + Password |
| **Thời gian sống** | Dài hạn (năm, vô thời hạn) | Ngắn hạn (session, có thể đổi) |
| **Lưu trữ** | Hardcode trong code app | User tự nhập khi login |
| **Số lượng** | Ít (5-10 apps) | Nhiều (hàng ngàn users) |
| **Mục đích** | Kiểm soát access của apps | Kiểm soát access của users |

---

## 🔍 Chi tiết: Client Authentication

### **ClientId + ApiSecret là gì?**

```
ClientId  = "mobile-app"          // Tên định danh của app
ApiSecret = "abc123secret"        // Password của app (bí mật)
```

### **Use Cases thực tế:**

#### 1. Mobile Application
```yaml
ClientId: "mobile-app"
ApiSecret: "********"
AllowedScopes:
  - courses.read
  - exams.submit
  - certifications.view
RateLimitPerMinute: 100
ValidFrom: 2024-01-01
ValidTo: null  # Vô thời hạn
```

**Tại sao cần?**
- Biết app mobile đang gọi API
- Giới hạn 100 requests/phút
- Chỉ cho phép đọc courses, submit exams, xem certs

#### 2. Web Portal
```yaml
ClientId: "web-portal"
ApiSecret: "********"
AllowedScopes:
  - courses.read
  - courses.write
  - users.manage
  - exams.create
RateLimitPerMinute: 500
```

**Tại sao cần?**
- Web portal cần nhiều permissions hơn mobile
- Rate limit cao hơn vì traffic nhiều hơn

#### 3. Partner Integration
```yaml
ClientId: "university-xyz-api"
ApiSecret: "********"
AllowedScopes:
  - courses.read
  - certifications.verify
AllowedIPs:
  - "203.113.131.0/24"  # IP của đối tác
RateLimitPerMinute: 1000
```

**Tại sao cần?**
- Đối tác chỉ được đọc courses và verify certs
- Chỉ chấp nhận requests từ IPs của đối tác
- Rate limit cao cho integration

---

## 🔍 Chi tiết: User Authentication

### **Username + Password là gì?**

```
Username = "nguyenvana@student.edu.vn"
Password = "********"  // User tự nhập
```

### **Use Cases thực tế:**

#### 1. Student Login
```yaml
Username: "nguyenvana@student.edu.vn"
Password: "MatKhauBiMat123!"
Role: "Student"
Permissions:
  - View enrolled courses
  - Submit exams
  - View grades
  - Download certificates
```

#### 2. Instructor Login
```yaml
Username: "tranthib@giaoVien.edu.vn"
Password: "GiaoVien@123"
Role: "Instructor"
Permissions:
  - Create/Edit courses
  - Grade exams
  - View student progress
  - Manage assignments
```

#### 3. Admin Login
```yaml
Username: "admin@smartcertify.edu.vn"
Password: "Admin@SecurePass"
Role: "Administrator"
Permissions:
  - Manage users
  - Manage all courses
  - System configuration
  - View analytics
```

---

## 🎨 Flow hoàn chỉnh

### **Scenario: Student dùng Mobile App để học**

```
┌─────────────────────────────────────────────────────────┐
│ Step 1: App khởi động và authenticate với API           │
└─────────────────────────────────────────────────────────┘
                      ↓
    ┌────────────────────────────────────┐
    │ Mobile App gửi request:            │
    │ POST /api/auth/login               │
    │ Body:                              │
    │ {                                  │
    │   "clientId": "mobile-app",        │
    │   "apiSecret": "abc123secret"      │
    │ }                                  │
    └────────────────────────────────────┘
                      ↓
    ┌────────────────────────────────────┐
    │ API Server kiểm tra:               │
    │ ✓ ClientId tồn tại?                │
    │ ✓ ApiSecret đúng?                  │
    │ ✓ IsEnabled = true?                │
    │ ✓ ValidFrom <= now <= ValidTo?     │
    │ ✓ AllowedScopes có "mobile"?       │
    └────────────────────────────────────┘
                      ↓
    ✅ Trả về APP TOKEN:
    {
      "access_token": "eyJhbGciOiJIUzI1NiIs...",
      "refresh_token": "abc123xyz...",
      "expires_in": 900,              // 15 phút
      "scope": "courses.read exams.submit"
    }

──────────────────────────────────────────────────────────

┌─────────────────────────────────────────────────────────┐
│ Step 2: Student login vào app                           │
└─────────────────────────────────────────────────────────┘
                      ↓
    ┌────────────────────────────────────┐
    │ Student nhập vào form:             │
    │ Email: nguyenvana@student.edu.vn   │
    │ Password: ********                 │
    └────────────────────────────────────┘
                      ↓
    ┌────────────────────────────────────┐
    │ Mobile App gửi request (với        │
    │ App Token từ Step 1):              │
    │ POST /api/users/login              │
    │ Headers:                           │
    │   Authorization: Bearer eyJhbGc... │
    │ Body:                              │
    │ {                                  │
    │   "email": "nguyenvana@...",       │
    │   "password": "********"           │
    │ }                                  │
    └────────────────────────────────────┘
                      ↓
    ┌────────────────────────────────────┐
    │ API Server kiểm tra:               │
    │ ✓ App Token valid?                 │
    │ ✓ User tồn tại?                    │
    │ ✓ Password đúng?                   │
    │ ✓ User active?                     │
    └────────────────────────────────────┘
                      ↓
    ✅ Trả về USER SESSION:
    {
      "userId": 123,
      "displayName": "Nguyễn Văn A",
      "email": "nguyenvana@student.edu.vn",
      "role": "Student",
      "avatar": "https://...",
      "enrolledCourses": [...]
    }

──────────────────────────────────────────────────────────

┌─────────────────────────────────────────────────────────┐
│ Step 3: Student gọi API để lấy danh sách khóa học       │
└─────────────────────────────────────────────────────────┘
                      ↓
    ┌────────────────────────────────────┐
    │ Mobile App gửi request:            │
    │ GET /api/courses                   │
    │ Headers:                           │
    │   Authorization: Bearer eyJhbGc... │
    │   X-User-Id: 123                   │
    └────────────────────────────────────┘
                      ↓
    ┌────────────────────────────────────┐
    │ API Server kiểm tra 2 lớp:         │
    │                                    │
    │ Layer 1 - Client Auth:             │
    │ ✓ App Token valid?                 │
    │ ✓ App có scope "courses.read"?     │
    │                                    │
    │ Layer 2 - User Auth:               │
    │ ✓ User 123 tồn tại?                │
    │ ✓ User có permission read courses? │
    └────────────────────────────────────┘
                      ↓
    ✅ Trả về danh sách khóa học:
    {
      "courses": [
        {
          "id": 1,
          "title": "Lập trình C#",
          "instructor": "Trần Thị B",
          ...
        },
        ...
      ]
    }
```

---

## 🛡️ Tại sao cần CẢ HAI lớp?

### **Nếu chỉ có User Authentication (không có Client Auth):**

❌ **Không biết app nào đang gọi API**
- Mobile app, web app, partner API đều dùng chung 1 endpoint
- Không thể rate limit per app
- Không thể revoke access của 1 app cụ thể

❌ **Không control được scopes**
- Partner API đáng lẽ chỉ được đọc courses
- Nhưng có thể gọi API sửa courses vì không có scope check

❌ **Không có analytics per app**
- Không biết app nào dùng nhiều nhất
- Không biết app nào hay bị lỗi

❌ **Security risk**
- Nếu mobile app bị reverse engineer, lộ credentials
- Kẻ tấn công có thể impersonate bất kỳ user nào
- Không thể disable app đó mà không ảnh hưởng users khác

---

### **Nếu chỉ có Client Authentication (không có User Auth):**

❌ **Không biết ai đang dùng system**
- Mọi student đều như nhau
- Không thể personalization dashboard

❌ **Không có authorization**
- Student có thể access instructor APIs
- Không thể grade exams, create courses

❌ **Không có audit trail**
- Không biết Nguyễn Văn A làm gì, khác Trần Thị B
- Không thể track progress individual

❌ **Không có session management**
- Không thể logout khỏi thiết bị cụ thể
- Không thể force logout user

---

## 🔐 Security Best Practices

### **Cho Client Credentials:**

1. **ApiSecret phải được hash**
   ```csharp
   // KHÔNG NÊN
   storedSecret = "plain-text-secret"
   
   // NÊN
   storedSecret = SHA256("plain-text-secret")
   // hoặc dùng BCrypt/Argon2
   ```

2. **Không hardcode trong client-side code**
   ```javascript
   // ❌ WRONG - Trong JavaScript (public)
   const apiSecret = "abc123secret";
   
   // ✅ RIGHT - Trong backend server
   const apiSecret = process.env.API_SECRET;
   ```

3. **Rotate secrets định kỳ**
   - Mỗi 6-12 tháng đổi ApiSecret
   - Có cơ chế rollback nếu有问题

4. **Giới hạn scopes tối thiểu**
   - Chỉ cho permissions cần thiết
   - Principle of Least Privilege

---

### **Cho User Credentials:**

1. **Password phải được hash mạnh**
   ```csharp
   // Dùng BCrypt hoặc Argon2
   var hashedPassword = BCrypt.HashPassword(plainPassword);
   ```

2. **Implement Account Lockout**
   - Khóa account sau 5 lần fail
   - Gửi email notification

3. **Two-Factor Authentication (2FA)**
   - SMS OTP
   - Authenticator App
   - Email verification

4. **Session Management**
   - Refresh tokens
   - Logout everywhere
   - Device tracking

---

## 📈 Monitoring & Analytics

### **Metrics cần track:**

#### **Client Layer:**
```
- Requests per client per minute
- Success/Failure rate per client
- Most called endpoints per client
- Rate limit violations
- Invalid credential attempts
```

#### **User Layer:**
```
- Login success/failure rate
- Active users per time period
- Session duration
- Failed login attempts per user
- Password reset requests
```

---

## 🎯 Tóm tắt

| Câu hỏi | Trả lời |
|---------|---------|
| **ClientId + ApiSecret** | Xác thực ỨNG DỤNG (machine-to-server) |
| **Username + Password** | Xác thực NGƯỜI DÙNG (human-to-server) |
| **Cần cả 2 vì sao?** | **Layer 1:** Kiểm soát app nào được gọi API<br>**Layer 2:** Kiểm soát ai đang dùng app đó |
| **Thiếu Layer 1** | Không biết app nào gọi, không rate limit được, không revoke app access |
| **Thiếu Layer 2** | Không personalization được, không audit user actions, không authorization |

---

## 📚 Tài liệu tham khảo

- [OAuth 2.0 Client Credentials Flow](https://oauth.net/2/grant-types/client-credentials/)
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
- [ARCHITECTURE.md](./ARCHITECTURE.md) - File thiết kế hệ thống của bạn

---

**Last Updated:** 2026-04-01  
**Author:** Education Platform Team
