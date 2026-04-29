# Huong dan chay End-to-End Melodify

Tai lieu nay huong dan chi tiet cach khoi dong va trai nghiem toan bo cac tinh nang cua he thong Melodify tren he dieu hanh macOS va Windows.

## Chuan bi CSDL (Chon 1 trong 2 cach)

Vi tren macOS khong ho tro SQL Server LocalDB mac dinh, ban co the chon mot trong hai cach thiet lap CSDL duoi day:

### Cach 1: Chay bang SQLite (Nhanh nhat, khong can cai dat gi them)
Ban chi can dung dong lenh de ghi de bien moi truong khi chay ung dung ma khong can thay doi cac file cau hinh mac dinh cua du an.

Chay lenh sau tai thu muc goc cua du an:
```bash
DatabaseProvider=Sqlite ConnectionStrings__DefaultConnection="Data Source=Melodify.db" dotnet run
```

### Cach 2: Chay bang SQL Server qua Docker
Neu ban muon su dung he quan tri SQL Server dung dac ta:
1. Khoi dong SQL Server container:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrongPassword123" -p 1433:1433 --name sql_server -d mcr.microsoft.com/mssql/server:2022-latest
   ```
2. Thay doi cac bien trong `Properties/launchSettings.json` cua profiles dang chay:
   - `"DatabaseProvider": "SqlServer"`
   - `"ConnectionStrings__DefaultConnection": "Server=localhost,1433;Database=MelodifyDb;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=True"`
3. Chay ung dung:
   ```bash
   dotnet run
   ```

## Quan ly co so du lieu bang TablePlus

De xem va quan ly cac bang du lieu (Users, Tracks, Playlists...) bang TablePlus, ban thiet lap ket noi nhu sau:

### Ket noi voi SQLite (Neu chay theo Cach 1)
1. Mo TablePlus, chon **Create a new connection** -> **SQLite**.
2. Nhap ten ket noi (Vi du: *Melodify Local*).
3. Tai muc **Database file**, nhan choose va tro den file `Melodify.db` nam ngay tai thu muc goc cua du an (`/Users/plxg/workspace/MELODIFY/Melodify.db`).
4. Nhan **Connect**. Ban co the xem truc tiep du lieu da seed tai day.

### Ket noi voi SQL Server Docker (Neu chay theo Cach 2)
1. Mo TablePlus, chon **Create a new connection** -> **Microsoft SQL Server**.
2. Nhap cac thong so cau hinh:
   - **Host**: `127.0.0.1`
   - **Port**: `1433`
   - **User**: `sa`
   - **Password**: `YourStrongPassword123`
   - **Database**: `MelodifyDb`
3. Nhan **Connect** de truy cap va thao tac voi CSDL.

---

## Cac buoc trai nghiem he thong

Sau khi ung dung khoi dong, mo trinh duyet va truy cap dia chi duoc thong bao tren terminal (Vi du: `http://localhost:5103` hoac `http://localhost:5000`).

### Buoc 1: Dang nhap tai khoan trai nghiem
He thong da tu dong khoi tao du lieu mau va tai khoan thu nghiem:
- **Tai khoan User**:
  - Email: `user@melodify.com`
  - Mat khau: `User123`
- **Tai khoan Admin**:
  - Email: `admin@melodify.com`
  - Mat khau: `Admin123`

### Buoc 2: Trai nghiem giao dien nguoi dung (User Role)
Dang nhap voi tai khoan `user@melodify.com`:
1. **Trang chu**: Xem loi chao dong theo thoi gian thuc, nghe thu danh sach bai hat "Kham pha hom nay" bang cach an vao card hoac nut Play, kiem tra thanh dieu khien am thanh o day trang.
2. **Tim kiem**: An vao menu "Tim kiem", go ten bai hat hoac ten nghe si va quan sat ket qua hien thi tuc thi, hoac click vao cac o the loai nhac sinh dong.
3. **Thich & Playlist**:
   - Bam nut Tim (Like) tren bat ky bai hat nao de dua vao danh sach "Bai da thich".
   - Bam nut "+" tren Sidebar hoac trang "Thu vien" de tao Playlist moi.
   - Bam vao nut menu ba cham (...) tren cac dong bai hat de them nhanh vao Playlist vua tao.

### Buoc 3: Trai nghiem tinh nang quan tri (Admin Role)
Dang nhap voi tai khoan `admin@melodify.com`:
1. Bam vao nut **Admin Panel** o goc tren ben phai de vao trang quan ly.
2. **Quan ly Bai hat**:
   - Xem danh sach bai hat co phan trang.
   - Bam "Tai len bai hat moi", keo tha hoac chon file am thanh dinh dang `.mp3` va anh bia de tai len he thong.
3. **Quan ly Nghe si & Album**: Them moi hoac cap nhat anh bia/anh dai dien de dong bo sang trang chi tiet cua phan Frontend.
