# MELODIFY — Hướng dẫn chạy hệ thống

Mô tả ngắn: ứng dụng web ASP.NET Core (MELODIFY). Tài liệu này hướng dẫn cách cài đặt, cấu hình và chạy toàn bộ hệ thống cục bộ.


**Yêu cầu (Prerequisites)**
- .NET SDK 9.0 (hoặc tương thích với target framework)
- MySQL server (hoặc DB tương thích mà project dùng)
- `dotnet-ef` CLI (để chạy migrations):
```bash
dotnet tool install --global dotnet-ef
```

**Cấu hình**
- Sao chép và chỉnh `appsettings.Development.json` hoặc chỉnh trực tiếp `appsettings.json` để đặt connection string cho database. Ví dụ:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=melodify;user=root;password=your_password"
}
```

- Thay vì commit secrets, bạn có thể dùng biến môi trường (Windows/Mac/Linux):

```bash
export ConnectionStrings__DefaultConnection="server=...;user=...;password=..."
```

**Cài đặt & chạy**

1. Lấy code và restore package:

```bash
git clone <repo-url>
cd MELODIFY
dotnet restore
```

2. Build và áp migrations:

```bash
dotnet build
dotnet ef database update
```

3. Chạy ứng dụng:

```bash
dotnet run
# hoặc
dotnet run --urls "http://localhost:5000"
```

4. Mở trình duyệt tại `http://localhost:5000` (hoặc URL hiển thị trên console).

**Seed dữ liệu & uploads**
- Project chứa `Data/SeedData.cs`. Nếu `SeedData` được gọi trong `Program.cs`, dữ liệu mẫu sẽ tự động được tạo khi lần chạy đầu tiên sau khi đã áp migrations.
- Thư mục `wwwroot/uploads/` và file `wwwroot/tracks.json` được giữ trong repo để lưu sample uploads; đừng xóa nếu muốn giữ dữ liệu mẫu.

**Lưu ý về Git**
- File có thể cài lại (build artifacts, `obj/`, `bin/`, `wwwroot/lib/`, `node_modules/`) đã được thêm vào `.gitignore` và đã được untrack từ repository.
- Các file config như `appsettings*.json`, seed data (`Data/SeedData.cs`) và uploads (`wwwroot/uploads/`, `wwwroot/tracks.json`) được giữ tracked như yêu cầu.

**Phát triển thêm**
- Nếu cần rebuild client libraries, dùng `libman`/`npm` theo cấu hình của project để tái tạo `wwwroot/lib/`.

**Liên hệ**
- Người thực hiện: Huỳnh Ngọc Vinh-VX24TTK2

---
