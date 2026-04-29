# Huong Dan Test End-to-End Melodify

Tai lieu nay huong dan chi tiet cach khoi dong he thong, ket noi database qua TablePlus va thuc hien test luong nguoi dung tren giao dien de kiem tra cac API/Controller Action.

---

## 1. Thong Tin Ket Noi Database Qua TablePlus

De quan sat du lieu thay doi thuc te khi test giao dien, ket noi toi co so du lieu tu xa (Remote MySQL) thong qua TablePlus:

- **Connection Type**: MySQL
- **Host**: sql12.freesqldatabase.com
- **Port**: 3306
- **User**: sql12830687
- **Password**: AFVSJcLVZl
- **Database**: sql12830687

### Cac bang can theo doi trong TablePlus:
- `AspNetUsers`: Danh sach nguoi dung dang ky.
- `Tracks`: Danh sach bai hat.
- `Playlists`: Cac playlist duoc tao.
- `LikedTracks`: Cac bai hat duoc yeu thich boi tung user.

---

## 2. Huong Dan Khoi Dong Project

Chay lenh sau tai thu muc goc cua du an de khoi dong ung dung:

```bash
dotnet run
```

Sau khi ung dung khoi dong successfully, truy cap:
- URL HTTP: http://localhost:5103
- URL HTTPS: https://localhost:7019

---

## 3. Quy Trinh Test Giao Dien Va Kiem Tra API

Mo **Chrome Developer Tools (F12)**, chuyen qua tab **Network** de theo doi tat ca cac request HTTP/API duoc gui len backend.

### Buoc 1: Test Authentication (Dang Nhap / Dang Ky)
1. Truy cap trang chu va click vao **Login**.
2. Nhap tai khoan test mac dinh duoc seed:
   - **Email**: user@melodify.com
   - **Password**: User123
3. Quan sat tab Network thay request POST gui den `/Account/Login`. Neu dang nhap thanh cong, trinh duyet nhan Cookie session va chuyen huong ve trang chu.
4. Dang xuat (Logout) de gui request den `/Account/Logout` va thu dang ky tai khoan moi tai `/Account/Register`.

### Buoc 2: Test Lay va Phat Nhac (Playback API)
1. Tai trang chu, click vao bieu tuong Play bat ky tren mot bai hat hoac Album.
2. Trinh phat nhac o duoi cung se xuat hien va phat am thanh.
3. Kiem tra tab Network xem trinh duyet co load file am thanh qua URL tu `soundhelix.com` hay khong.
4. Moi khi nhac phat, he thong se gui mot yeu cau ngam de tang luot nghe. Xem request goi API de cap nhat `PlayCount`.

### Buoc 3: Test Tim Kiem (Search API)
1. Chuyen den trang **Search**.
2. Nhap tu khoa tim kiem (vi du: "Son Tung").
3. Thuat toan debouncing 300ms se tu dong gui mot request GET den `/Search/SearchJson?query=Son Tung` ma khong can reload trang.
4. Kiem tra tab Network de xac nhan chi co 1 request duoc goi sau khi ban ngung go 300ms, va phan hoi tra ve dinh dang JSON chua danh sach Tracks, Artists va Albums phu hop.

### Buoc 4: Test Thu Vien (Library, Like va Playlist API)
1. **Like bai hat**:
   - Bam nut Heart tren mot bai hat bat ky tren giao dien.
   - Theo doi tab Network goi request POST den `/Library/LikeTrack`.
   - Mo TablePlus kiem tra bang `LikedTracks` xem da xuat hien ban ghi tuong ung hay chua.
2. **Tao Playlist**:
   - Truy cap trang Library va click **Create Playlist**.
   - Thuc hien tao playlist moi va kiem tra request POST den `/Playlists/Create`.
   - Kiem tra bang `Playlists` trong TablePlus de thay dong du lieu moi.
3. **Them bai hat vao Playlist**:
   - Click bieu tuong ba cham tren bai hat va chon **Add to Playlist**.
   - Check request POST den `/Playlists/AddTrack` trong tab Network va kiem tra bang `PlaylistTracks` trong TablePlus.

### Buoc 5: Test Giao Dien Admin (Chung nang Quan Ly)
1. Dang xuat khoi tai khoan thuong va dang nhap voi tai khoan Admin:
   - **Email**: admin@melodify.com
   - **Password**: Admin123
2. Giao dien Admin se co them cac action quan ly tai `/Admin/Tracks`, `/Admin/Albums`, `/Admin/Artists`.
3. Thu thuc hien cac thao tac Them, Sua, Xoa va kiem tra API tuong ung de dam bao quyen Admin hoat dong dung.
