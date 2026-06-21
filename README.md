# MELODIFY - Hướng dẫn chạy hệ thống

Melodify là ứng dụng web nghe nhạc trực tuyến được phát triển trên nền tảng ASP.NET Core MVC, Entity Framework Core và MySQL. Hướng dẫn này giúp người dùng clone code từ repository và chạy ứng dụng trên máy cục bộ.

## Yêu cầu hệ thống
- .NET SDK 9.0 trở lên
- Trình duyệt web hiện đại (Chrome, Edge, Firefox, Safari)

## Cấu hình Database
Hệ thống được cấu hình sẵn trong file launchSettings.json để kết nối với cơ sở dữ liệu MySQL từ xa (sql12.freesqldatabase.com). Người dùng có thể chạy ứng dụng ngay mà không cần thiết lập MySQL cục bộ.

Nếu muốn thay đổi kết nối đến MySQL cục bộ của bạn, hãy thay đổi giá trị DefaultConnection trong file appsettings.json hoặc appsettings.Development.json:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=melodify;Uid=root;Pwd=mat_khau_cua_ban;"
}
```

## Hướng dẫn Clone và Chạy Ứng dụng

1. Clone repository về máy cục bộ:
```bash
git clone https://github.com/vinhhuynh-ngoc/MELODIFY.git
cd MELODIFY
```

2. Di chuyển đến thư mục nguồn của dự án:
```bash
cd src
```

3. Restore các gói thư viện:
```bash
dotnet restore
```

4. Build dự án:
```bash
dotnet build
```

5. Run ứng dụng:
```bash
dotnet run
```

Hệ thống sẽ được khởi chạy và lắng nghe mặc định tại địa chỉ: http://localhost:5103

## Thông tin Tài khoản Mặc định
Sau khi chạy ứng dụng và database được khởi tạo, dữ liệu mẫu (Seed Data) sẽ tự động được thêm vào hệ thống với các tài khoản mặc định sau:

1. Tài khoản Quản trị (Admin):
- Email: admin@melodify.com
- Mật khẩu: Admin123

2. Tài khoản Người dùng thường (User):
- Email: user@melodify.com
- Mật khẩu: User123

## Cấu trúc thư mục chính của dự án
- docs/: Thư mục chứa tài liệu hệ thống
- src/: Thư mục chứa mã nguồn chính của ứng dụng web ASP.NET Core
- thesis/: Thư mục chứa các báo cáo và file khóa luận liên quan

## Liên hệ
- Người thực hiện: Huỳnh Ngọc Vinh - VX24TTK2
