# DESKTOP OVERLAY MANAGER

> Ứng dụng hiển thị PNG, GIF, WebP, video và các widget nhỏ dưới dạng lớp phủ trên màn hình Windows.

---

## 1. Tổng quan dự án

### 1.1. Tên gợi ý

Một số tên có thể sử dụng cho dự án:

- **OverlayCanvas**
- **DeskOverlay**
- **FloatLayer**
- **ScreenPetal**
- **OverlayBox**
- **LayerDesk**

Tên tạm dùng trong tài liệu này: **OverlayCanvas**.

### 1.2. Ý tưởng chính

OverlayCanvas là một ứng dụng Windows cho phép người dùng đưa các nội dung như:

- PNG
- JPG / JPEG
- GIF
- WebP
- APNG
- SVG
- Video ngắn
- Text
- Clock / Timer
- Widget đơn giản

lên màn hình máy tính dưới dạng các **overlay trong suốt**, luôn hiển thị phía trên hoặc phía dưới các ứng dụng khác tùy cấu hình.

Người dùng có thể chạy nhiều overlay cùng lúc, di chuyển, thay đổi kích thước, độ trong suốt, khóa vị trí, bật/tắt nhanh bằng phím tắt và lưu thành các bộ cấu hình riêng.

Mục tiêu chính của dự án là:

- Chạy nhẹ.
- Không chiếm nhiều CPU/GPU/RAM.
- Không gây ảnh hưởng tới thao tác chuột và bàn phím.
- Giao diện hiện đại, dễ sử dụng.
- Hỗ trợ nhiều overlay đồng thời.
- Cho phép tùy biến sâu nhưng không làm giao diện phức tạp.

---

# 2. Đối tượng sử dụng

Ứng dụng có thể phục vụ nhiều nhóm người dùng khác nhau.

### Người dùng thông thường

Có thể sử dụng để:

- Trang trí desktop.
- Hiển thị GIF nhân vật yêu thích.
- Hiển thị mascot trên desktop.
- Hiển thị ảnh tham khảo khi làm việc.
- Hiển thị lịch hoặc đồng hồ.

### Streamer / Content Creator

Có thể sử dụng để:

- Hiển thị icon.
- Hiển thị animation.
- Hiển thị mascot.
- Hiển thị thông tin hỗ trợ stream.

### Gamer

Có thể sử dụng để:

- Hiển thị checklist.
- Hiển thị ảnh hướng dẫn.
- Hiển thị timer.
- Hiển thị thông tin build nhân vật.
- Hiển thị buff hoặc trạng thái theo cấu hình thủ công.

Ứng dụng **không cần inject vào game**. Overlay chỉ nên sử dụng cửa sổ Windows bình thường để hạn chế xung đột với anti-cheat.

### Designer / Developer

Có thể sử dụng để:

- Hiển thị mockup tham khảo.
- Overlay ảnh tham chiếu lên UI đang phát triển.
- So sánh layout.
- Hiển thị ghi chú nhanh.

---

# 3. Các yêu cầu quan trọng

## 3.1. Hiệu năng

Ứng dụng cần ưu tiên hiệu năng và khả năng chạy nền lâu dài.

Mục tiêu đề xuất khi chạy ở trạng thái bình thường:

- RAM: khoảng 50–150 MB cho ứng dụng chính.
- CPU idle: gần 0%.
- Không render lại overlay nếu nội dung không thay đổi.
- GIF chỉ render theo FPS thực tế của GIF.
- Overlay bị ẩn hoặc ngoài màn hình phải được giảm hoặc dừng render.
- Không dùng Chromium/Electron ở phiên bản đầu.

---

# 4. Công nghệ đề xuất

## 4.1. Nền tảng

Phiên bản đầu tiên nên tập trung vào:

**Windows 10 / Windows 11**.

Không nên cố làm đa nền tảng ngay từ đầu vì chức năng overlay phụ thuộc rất nhiều vào API của hệ điều hành.

---

## 4.2. Ngôn ngữ

Đề xuất:

```text
C#
.NET 10 LTS
```

Lý do:

- Tích hợp Windows API dễ.
- Hỗ trợ tốt WPF.
- Native desktop application.
- Không cần Chromium runtime.
- Dễ triển khai system tray.
- Dễ tạo global hotkey.
- Dễ quản lý startup Windows.
- Dễ thao tác HWND / Win32.

---

## 4.3. UI Framework

Đề xuất:

```text
WPF
```

Kết hợp:

```text
MVVM
Microsoft.Extensions.DependencyInjection
```

Có thể dùng thêm một thư viện UI hiện đại để tạo phong cách giống Windows 11.

Không nên để thư viện UI phụ thuộc trực tiếp vào Overlay Engine.

---

# 5. Kiến trúc hệ thống

Đề xuất chia thành các module sau:

```text
OverlayCanvas
│
├── OverlayCanvas.App
│   ├── Views
│   ├── ViewModels
│   ├── Controls
│   └── Resources
│
├── OverlayCanvas.Core
│   ├── Models
│   ├── Interfaces
│   ├── Events
│   └── Common
│
├── OverlayCanvas.Overlay
│   ├── OverlayManager
│   ├── OverlayWindow
│   ├── Renderer
│   └── MonitorManager
│
├── OverlayCanvas.Media
│   ├── ImageLoader
│   ├── GifRenderer
│   ├── WebPRenderer
│   ├── VideoRenderer
│   └── ThumbnailGenerator
│
├── OverlayCanvas.Platform.Windows
│   ├── Win32
│   ├── GlobalHotkey
│   ├── StartupManager
│   ├── WindowStyleManager
│   └── TrayManager
│
├── OverlayCanvas.Persistence
│   ├── JsonSettings
│   ├── ProfileRepository
│   └── MediaRepository
│
└── OverlayCanvas.Infrastructure
```

---

# 6. Mô hình hoạt động

Ứng dụng gồm hai phần chính.

## Control Center

Là cửa sổ quản lý chính.

Người dùng thực hiện:

- Import ảnh.
- Xóa ảnh.
- Tạo overlay.
- Chỉnh kích thước.
- Chỉnh opacity.
- Chọn màn hình.
- Chọn profile.
- Chỉnh hotkey.

## Overlay Engine

Chịu trách nhiệm:

- Tạo overlay window.
- Render hình ảnh.
- Render GIF.
- Quản lý nhiều overlay.
- Quản lý Z-order.
- Click-through.
- Theo dõi nhiều monitor.

Sơ đồ:

```text
            ┌──────────────────┐
            │   Control Center │
            └────────┬─────────┘
                     │
                     ▼
            ┌──────────────────┐
            │  Overlay Manager │
            └────────┬─────────┘
                     │
       ┌─────────────┼─────────────┐
       ▼             ▼             ▼
 Overlay #1      Overlay #2     Overlay #3
 PNG             GIF            Text
```

---

# 7. Các chức năng chính

# 7.1. Import Media

Người dùng có thể import bằng:

- File Picker.
- Drag & Drop.
- Paste từ clipboard.
- Paste ảnh trực tiếp.
- Paste đường dẫn file.

Định dạng đề xuất:

```text
.png
.jpg
.jpeg
.gif
.webp
.apng
.bmp
.svg
.mp4
.webm
```

Media sau khi import được thêm vào **Media Library**.

---

# 7.2. Media Library

Hiển thị dạng grid.

Ví dụ:

```text
┌─────────┐ ┌─────────┐ ┌─────────┐
│ GIF #1  │ │ PNG #1  │ │ PNG #2  │
│ preview │ │ preview │ │ preview │
└─────────┘ └─────────┘ └─────────┘
```

Mỗi media có:

- Tên.
- Preview.
- Loại file.
- Kích thước.
- Dung lượng.
- Favorite.
- Tags.

Cho phép:

- Search.
- Filter theo loại.
- Sort.
- Rename.
- Remove.
- Duplicate.

---

# 7.3. Tạo Overlay

Người dùng chọn media và nhấn:

```text
Add Overlay
```

Overlay xuất hiện ngay trên desktop.

Mỗi overlay có cấu hình riêng.

---

# 7.4. Di chuyển Overlay

Có hai chế độ.

### Edit Mode

Overlay có thể:

- Kéo.
- Resize.
- Rotate.
- Snap.

### Lock Mode

Overlay không nhận input chuột.

Mouse sẽ đi xuyên qua overlay tới ứng dụng bên dưới.

---

# 7.5. Click-through

Đây là một tính năng quan trọng.

Khi bật:

```text
Click-through = ON
```

overlay sẽ không chặn thao tác chuột.

Ví dụ:

```text
Mouse
   │
   ▼
┌──────────────┐
│ Overlay GIF  │  ← bỏ qua mouse
└──────────────┘
   │
   ▼
┌──────────────┐
│ Game / App   │  ← nhận click
└──────────────┘
```

---

# 7.6. Always on Top

Mỗi overlay có thể chọn:

```text
Normal
Always on Top
Desktop Layer
```

### Normal

Overlay hoạt động như cửa sổ bình thường.

### Always on Top

Overlay luôn nằm phía trên các cửa sổ thông thường.

### Desktop Layer

Overlay nằm gần desktop layer, phù hợp cho widget trang trí.

---

# 7.7. Taskbar Overlay Mode

Người dùng có thể lựa chọn:

```text
Allow overlay above taskbar
```

OFF:

```text
┌─────────────────────────┐
│                         │
│       Desktop           │
│       Overlay           │
│                         │
├─────────────────────────┤
│        Taskbar          │
└─────────────────────────┘
```

ON:

```text
┌─────────────────────────┐
│                         │
│       Desktop           │
│                         │
├───────── Overlay ────────┤
│        Taskbar          │
└─────────────────────────┘
```

Nên để mặc định:

```text
OFF
```

vì overlay trên taskbar có thể che Start Menu hoặc icon hệ thống.

---

# 7.8. Multi Overlay

Người dùng có thể chạy nhiều overlay cùng lúc.

Ví dụ:

```text
Overlay 1 → Character GIF
Overlay 2 → Clock
Overlay 3 → PNG Logo
Overlay 4 → Checklist
Overlay 5 → Timer
```

Overlay Manager quản lý toàn bộ các overlay.

---

# 7.9. Overlay List

Control Center có panel:

```text
Active Overlays

[x] Raiden.gif
[x] Timer
[x] Build.png
[ ] Logo.png
```

Cho phép:

- Show.
- Hide.
- Duplicate.
- Delete.
- Lock.
- Bring to front.

---

# 8. Các thuộc tính của Overlay

Mỗi overlay có thể cấu hình:

```text
Position X
Position Y
Width
Height
Scale
Rotation
Opacity
Visible
Locked
ClickThrough
AlwaysOnTop
TaskbarOverlay
Monitor
FPS Limit
```

Ví dụ model:

```json
{
  "id": "overlay-01",
  "name": "Character",
  "mediaId": "media-15",
  "x": 1400,
  "y": 600,
  "width": 320,
  "height": 320,
  "opacity": 0.9,
  "rotation": 0,
  "visible": true,
  "locked": true,
  "clickThrough": true,
  "alwaysOnTop": true,
  "taskbarOverlay": false,
  "monitor": 1,
  "fpsLimit": 30
}
```

---

# 9. Multi-monitor

Ứng dụng phải hỗ trợ nhiều màn hình.

Ví dụ:

```text
Monitor 1
1920x1080

Monitor 2
2560x1440
```

Mỗi overlay có thể được gắn với:

```text
Primary Monitor
Monitor 1
Monitor 2
Follow active monitor
```

Cần xử lý:

- Thay đổi độ phân giải.
- Rút màn hình phụ.
- Cắm màn hình mới.
- Thay đổi DPI.
- Thay đổi scale Windows.

---

# 10. Global Hotkey

Ứng dụng nên hỗ trợ phím tắt toàn cục.

Ví dụ:

```text
Ctrl + Shift + O
```

Mở Control Center.

```text
Ctrl + Shift + H
```

Ẩn / hiện toàn bộ overlay.

```text
Ctrl + Shift + L
```

Lock / Unlock overlay.

```text
Ctrl + Shift + E
```

Bật Edit Mode.

Người dùng có thể thay đổi hotkey trong Settings.

---

# 11. System Tray

Khi đóng cửa sổ chính, ứng dụng không nhất thiết thoát.

Ứng dụng chạy ở:

```text
System Tray
```

Menu:

```text
OverlayCanvas

Open Control Center
Show All Overlays
Hide All Overlays
Edit Mode
Profiles
Settings
Exit
```

---

# 12. Startup cùng Windows

Settings:

```text
Start OverlayCanvas with Windows
```

Có thể thêm tùy chọn:

```text
Start minimized
Restore last profile
Delay startup 5 seconds
```

Delay startup giúp tránh cạnh tranh CPU/Disk với các ứng dụng khác ngay khi đăng nhập Windows.

---

# 13. Profiles

Một trong những tính năng nên có nhất.

Người dùng có thể tạo các profile như:

```text
Desktop
Gaming
Streaming
Work
Study
```

Ví dụ:

### Gaming

```text
Character.gif
Build.png
Timer
```

### Work

```text
Clock
Todo
Calendar
```

Chỉ cần chọn profile để bật toàn bộ overlay tương ứng.

---

# 14. Scene

Có thể nâng cấp Profile thành **Scene**.

Ví dụ:

```text
Scene: Zenless Zone Zero

Overlay:
- Character.png
- Buff.png
- Rotation.png
- Timer
```

Scene có thể được export và import.

Ví dụ:

```text
zzz-scene.overlaycanvas
```

---

# 15. Auto Scene

Ứng dụng có thể tự chuyển scene dựa theo chương trình đang chạy.

Ví dụ:

```text
ZenlessZoneZero.exe
        ↓
Scene ZZZ
```

```text
Photoshop.exe
        ↓
Scene Design
```

```text
VisualStudio.exe
        ↓
Scene Coding
```

Đây là tính năng rất hữu ích nhưng nên để ở phiên bản sau.

---

# 16. Overlay Groups

Cho phép gom nhiều overlay thành nhóm.

Ví dụ:

```text
ZZZ Group

├── Character
├── Weapon
├── Buff
└── Timer
```

Khi di chuyển group, toàn bộ overlay di chuyển cùng nhau.

---

# 17. Snap System

Overlay có thể tự căn theo:

- Mép màn hình.
- Tâm màn hình.
- Overlay khác.
- Grid.

Ví dụ:

```text
Snap Distance = 10 px
```

---

# 18. Anchor

Cho phép đặt overlay theo vị trí tương đối.

```text
Top Left
Top Center
Top Right
Center
Bottom Left
Bottom Center
Bottom Right
```

Ví dụ:

```text
Anchor = Bottom Right
Offset X = -20
Offset Y = -20
```

Nếu thay đổi độ phân giải màn hình, overlay vẫn giữ đúng vị trí tương đối.

---

# 19. Animation Settings

GIF / APNG / WebP animation có thể cấu hình:

```text
Playback Speed
FPS Limit
Loop
Pause
Reverse
Start Delay
```

Ví dụ:

```text
Playback Speed = 0.5x
```

---

# 20. Performance Mode

Settings:

```text
Performance Mode
```

Các tùy chọn:

```text
High Quality
Balanced
Low Power
```

### High Quality

- Render full FPS.
- High quality scaling.

### Balanced

- FPS giới hạn 30.
- Pause overlay khi không cần thiết.

### Low Power

- FPS giới hạn 15–20.
- Dừng animation khi fullscreen app được phát hiện nếu người dùng bật tùy chọn này.

---

# 21. Smart Render

Để giảm tải phần cứng:

### PNG tĩnh

Không render lại nếu không thay đổi.

### GIF

Chỉ render khi tới frame tiếp theo.

### Overlay bị Hide

Dừng toàn bộ animation.

### Overlay opacity = 0

Dừng render.

### Màn hình đang sleep

Pause animation.

---

# 22. Overlay FPS Limit

Mỗi overlay có thể đặt:

```text
15 FPS
24 FPS
30 FPS
60 FPS
Unlimited
```

Mặc định đề xuất:

```text
30 FPS
```

Đối với GIF, nên ưu tiên thời gian frame gốc thay vì ép animation thành 60 FPS.

---

# 23. Memory Cache

Ảnh chỉ nên decode một lần.

```text
Media
 ↓
Decode
 ↓
Cache
 ↓
Overlay 1
Overlay 2
Overlay 3
```

Nếu ba overlay sử dụng cùng một ảnh thì không cần load file ba lần.

---

# 24. Video Overlay

Phiên bản sau có thể hỗ trợ video:

```text
MP4
WEBM
```

Các tùy chọn:

- Mute.
- Loop.
- Playback speed.
- Start time.
- End time.

Để đảm bảo ứng dụng nhẹ, video overlay nên là tính năng tùy chọn.

---

# 25. Text Overlay

Cho phép người dùng thêm text.

Ví dụ:

```text
Daily Tasks

□ Daily
□ Event
□ Farm
```

Tùy chỉnh:

- Font.
- Size.
- Font Weight.
- Color.
- Background.
- Shadow.
- Opacity.

---

# 26. Clock Widget

Widget mặc định:

```text
19:45
24/09/2026
```

Có thể tùy chỉnh format.

---

# 27. Timer Widget

Ví dụ:

```text
05:43
```

Có thể dùng cho:

- Pomodoro.
- Game boss timer.
- Farming timer.

---

# 28. Playlist Overlay

Cho phép một overlay luân phiên nhiều ảnh.

Ví dụ:

```text
Image 1
  ↓ 10s
Image 2
  ↓ 10s
Image 3
```

Transition:

- Fade.
- Slide.
- None.

---

# 29. Random Overlay

Có thể chọn một folder.

Ứng dụng định kỳ chọn ảnh ngẫu nhiên.

Ví dụ:

```text
Change every 5 minutes
```

---

# 30. Mouse Hover Mode

Có thể cho overlay:

```text
Opacity = 20%
```

Khi mouse hover:

```text
Opacity = 100%
```

Hoặc ngược lại:

```text
Mouse nearby → fade out
```

Tính năng này hữu ích cho các overlay nằm gần vùng thao tác.

---

# 31. Auto Hide

Overlay có thể tự ẩn nếu:

```text
Fullscreen app detected
Specific app is running
Specific app is focused
Idle timeout
```

Ví dụ:

```text
Hide when:
chrome.exe is fullscreen
```

---

# 32. Application Rules

Cho phép cấu hình:

```text
IF process = Photoshop.exe
THEN show Reference Overlay
```

Hoặc:

```text
IF process = Game.exe
THEN load Gaming Scene
```

---

# 33. Focus Rules

Ví dụ:

```text
Show overlay only when:
ZenlessZoneZero.exe is focused
```

Khi Alt+Tab:

overlay tự động ẩn.

---

# 34. Wallpaper Mode

Overlay có thể nằm phía trên wallpaper nhưng phía dưới icon desktop.

Phù hợp cho:

- Desktop mascot.
- Animated decoration.
- Clock.

Đây là tính năng nâng cao vì Windows desktop sử dụng nhiều window layer đặc biệt.

---

# 35. Presentation Mode

Một nút:

```text
Hide All Overlays
```

để nhanh chóng ẩn mọi overlay khi:

- Share Screen.
- Họp online.
- Trình chiếu.

Có thể gán hotkey.

---

# 36. Privacy Mode

Có thể bổ sung:

```text
Hide overlays during screen capture
```

Tính năng này phụ thuộc vào khả năng của Windows và loại phần mềm capture, do đó nên xem là tính năng nâng cao và không cam kết hoạt động với mọi phần mềm.

---

# 37. Edit Mode UX

Khi bật Edit Mode:

```text
┌────────────────────────────┐
│                            │
│       ┌─────────────┐      │
│       │   Overlay   │      │
│       │             │      │
│       └─────────────┘      │
│         Resize handles     │
│                            │
└────────────────────────────┘
```

Các thao tác:

```text
Drag      → Move
Corner    → Resize
Shift     → Maintain aspect ratio
Delete    → Remove overlay
Ctrl + D  → Duplicate
```

---

# 38. Context Menu

Right click overlay khi ở Edit Mode:

```text
Edit
Duplicate
Hide
Lock
Always on Top
Click Through
Opacity
Monitor
Add to Group
Delete
```

---

# 39. Undo / Redo

Nên có:

```text
Ctrl + Z
Ctrl + Y
```

Cho các thao tác:

- Move.
- Resize.
- Delete.
- Add overlay.

---

# 40. Autosave

Mọi thay đổi cấu hình nên được save tự động.

Ví dụ:

```text
config.json
```

Không cần nút Save riêng cho các thay đổi thông thường.

---

# 41. Backup

Ứng dụng nên tự backup cấu hình.

Ví dụ:

```text
Backups/

2026-09-24.json
2026-09-23.json
```

Nếu config bị hỏng có thể restore.

---

# 42. Export / Import Profile

Cho phép export:

```text
Gaming.overlayprofile
```

Bao gồm:

- Overlay configs.
- Layout.
- Media references.
- Hotkey local của scene.

Có thể có tùy chọn đóng gói cả media thành một file ZIP nội bộ.

---

# 43. Plugin System

Phiên bản nâng cao có thể hỗ trợ plugin.

Ví dụ:

```text
Spotify Widget
Weather Widget
Discord Status
CPU Monitor
Game Timer
```

Interface ví dụ:

```csharp
public interface IOverlayPlugin
{
    string Name { get; }

    void Initialize();

    IOverlayContent CreateOverlay();
}
```

Plugin system không nên xuất hiện trong MVP.

---

# 44. Cấu trúc dữ liệu

```text
AppData
│
├── config.json
│
├── media
│   ├── image01.png
│   ├── character.gif
│   └── icon.webp
│
├── thumbnails
│
├── profiles
│   ├── default.json
│   ├── gaming.json
│   └── work.json
│
├── scenes
│
├── cache
│
├── backups
│
└── logs
```

---

# 45. Entity / Model đề xuất

## OverlayItem

```text
OverlayItem

Id
Name
Type
MediaId
Position
Size
Opacity
Rotation
ZIndex
MonitorId
IsVisible
IsLocked
IsClickThrough
IsTopMost
AllowOverTaskbar
FpsLimit
Anchor
GroupId
```

## MediaItem

```text
MediaItem

Id
Name
FilePath
MediaType
Width
Height
FileSize
ThumbnailPath
Tags
IsFavorite
```

## Profile

```text
Profile

Id
Name
OverlayIds
CreatedAt
UpdatedAt
```

## OverlayGroup

```text
OverlayGroup

Id
Name
OverlayIds
Position
```

---

# 46. Services đề xuất

```text
IOverlayService
IMediaService
IProfileService
ISceneService
IHotkeyService
ISettingsService
IStartupService
IMonitorService
IRenderService
IProcessWatcherService
ICacheService
ITrayService
```

---

# 47. Overlay Manager

Overlay Manager là thành phần quan trọng nhất.

```text
OverlayManager

CreateOverlay()
RemoveOverlay()
ShowOverlay()
HideOverlay()
ShowAll()
HideAll()
LockAll()
SetTopMost()
MoveOverlay()
UpdateOverlay()
RestoreProfile()
```

Manager giữ danh sách:

```text
Dictionary<Guid, OverlayWindow>
```

---

# 48. Win32 API cần sử dụng

Các chức năng Windows native dự kiến cần dùng:

```text
SetWindowPos
GetWindowLong
SetWindowLong
RegisterHotKey
UnregisterHotKey
MonitorFromWindow
GetMonitorInfo
EnumDisplayMonitors
```

Các window style quan trọng:

```text
WS_EX_TOOLWINDOW
WS_EX_TOPMOST
WS_EX_TRANSPARENT
WS_EX_NOACTIVATE
```

Mục tiêu:

- Không xuất hiện mỗi overlay trong Alt+Tab.
- Không chiếm taskbar button.
- Không lấy focus khi người dùng click.
- Cho phép click-through.
- Điều khiển Z-order.

---

# 49. Cơ chế Always On Top

Windows cho phép điều khiển Z-order của một cửa sổ thông qua `SetWindowPos`.

Overlay có thể chuyển giữa:

```text
HWND_TOPMOST
HWND_NOTOPMOST
```

Do đó người dùng có thể bật/tắt Always on Top ngay khi chương trình đang chạy.

---

# 50. Taskbar Overlay – lưu ý kỹ thuật

Taskbar của Windows cũng là một cửa sổ hệ thống và cách quản lý Z-order có thể thay đổi theo phiên bản Windows.

Do đó nên thiết kế hai mức:

```text
Always on Top
```

và

```text
Force above taskbar
```

Chế độ thứ hai chỉ nên bật khi người dùng chủ động chọn.

Không nên liên tục spam `SetWindowPos` mỗi frame để ép overlay lên trên vì sẽ làm tăng CPU và có thể gây hiện tượng nhấp nháy.

---

# 51. Fullscreen Game

Cần phân biệt:

### Borderless Fullscreen

Overlay topmost thường có thể hiển thị.

### Exclusive Fullscreen

Overlay desktop thông thường có thể không hiển thị phía trên game.

Ứng dụng **không nên sử dụng DLL injection hoặc graphics hook trong MVP**.

Lý do:

- Tăng độ phức tạp.
- Có nguy cơ xung đột anti-cheat.
- Tăng khả năng crash.
- Khó bảo trì.

---

# 52. Anti-cheat Friendly Design

Nguyên tắc:

```text
No process injection
No game memory reading
No DirectX hook
No DLL injection
```

Overlay chỉ là một cửa sổ Windows thông thường.

Đây là cách tiếp cận an toàn và đơn giản hơn nhiều.

---

# 53. Giao diện chính đề xuất

```text
┌──────────────────────────────────────────────────────┐
│ OverlayCanvas                               _ □ X    │
├──────────────┬───────────────────────────────────────┤
│              │                                       │
│ Dashboard    │      Active Overlays                  │
│ Media        │                                       │
│ Profiles     │     ┌────────┐ ┌────────┐            │
│ Scenes       │     │ GIF    │ │ PNG    │            │
│              │     └────────┘ └────────┘            │
│ Hotkeys      │                                       │
│ Settings     │                                       │
│              │                                       │
└──────────────┴───────────────────────────────────────┘
```

---

# 54. Dashboard

Dashboard nên hiển thị:

```text
Active Overlays: 4
Current Profile: Gaming
CPU: 0.4%
Memory: 86 MB
```

Các nút nhanh:

```text
+ Add Overlay
Hide All
Edit Mode
Lock All
```

---

# 55. Settings

Các tab đề xuất:

```text
General
Overlay
Performance
Hotkeys
Startup
Appearance
Advanced
```

---

# 56. General Settings

```text
Launch on Windows startup
Start minimized
Minimize to tray
Close to tray
Restore previous profile
Check for updates
```

---

# 57. Overlay Settings

```text
Default click-through
Default always-on-top
Default FPS
Default opacity
Snap distance
Show overlay border in Edit Mode
```

---

# 58. Performance Settings

```text
Rendering Mode
FPS Limit
Pause hidden overlays
Pause animations when monitor sleeps
Image cache size
Hardware acceleration
```

---

# 59. Appearance

```text
Light
Dark
System
```

Có thể cho chọn Accent Color.

---

# 60. Logging

Ứng dụng cần log các lỗi quan trọng.

Ví dụ:

```text
logs/
2026-09-24.log
```

Không nên log liên tục từng frame.

Chỉ log:

- App start.
- App exit.
- Media load error.
- Config error.
- Renderer exception.
- Overlay restore failure.

---

# 61. Crash Recovery

Nếu chương trình crash:

Lần khởi động sau hiển thị:

```text
OverlayCanvas did not close correctly.

Restore previous session?
```

Người dùng chọn:

```text
Restore
Start Safe Mode
```

Safe Mode không tự load overlay.

---

# 62. Single Instance

Chỉ cho phép một instance của Control Center chạy.

Nếu người dùng mở chương trình lần hai:

```text
Existing instance → Bring to Front
```

Không khởi động thêm toàn bộ Overlay Engine.

---

# 63. Update System

Có thể thêm:

```text
Check for updates
```

Phiên bản đầu có thể dùng update thủ công.

Phiên bản sau có thể dùng auto update.

---

# 64. Security

Một số nguyên tắc:

- Không tự tải file từ Internet nếu người dùng chưa xác nhận.
- Không chạy script nằm trong profile.
- Validate file type.
- Không load executable từ plugin chưa tin cậy.
- Giới hạn kích thước media import nếu cần.

---

# 65. Các tính năng nên bổ sung để dự án hoàn thiện hơn

Ngoài yêu cầu ban đầu, các tính năng sau rất đáng thêm.

## Favorite

Đánh dấu media thường dùng.

## Tags

Ví dụ:

```text
ZZZ
Anime
Work
Timer
```

## Search

Tìm media nhanh.

## Duplicate Overlay

Tạo nhanh một overlay giống overlay hiện tại.

## Reset Position

Đưa overlay về giữa màn hình nếu bị nằm ngoài vùng hiển thị.

## Fit to Screen

Scale ảnh về kích thước phù hợp màn hình.

## Maintain Aspect Ratio

Không làm méo ảnh khi resize.

## Opacity Slider

Điều chỉnh nhanh từ 0–100%.

## Pin

Khóa overlay khỏi thao tác vô tình.

## Quick Toggle

Mỗi overlay có thể có hotkey riêng.

---

# 66. Ý tưởng nâng cao

## Interactive Overlay

Một overlay có thể chứa button hoặc checkbox.

Ví dụ:

```text
Daily Checklist

[x] Daily
[ ] Farm
[ ] Event
```

Khi bật interactive mode thì overlay không click-through.

---

## Browser Overlay

Phiên bản nâng cao có thể render nội dung HTML.

Ví dụ:

```text
local widget
clock
custom dashboard
```

Không nên đưa browser engine vào MVP vì tăng RAM đáng kể.

---

## Audio Reactive Overlay

Animation thay đổi dựa trên âm thanh đang phát.

Ví dụ mascot nhún theo nhạc.

---

## Screen Edge Dock

Overlay có thể dock vào mép màn hình.

Khi không dùng:

```text
[ > ]
```

Hover vào sẽ mở rộng.

---

## Overlay Preset Store

Về lâu dài có thể có thư viện preset cộng đồng.

Ví dụ:

```text
Minimal Clock
Anime Desk
Streamer Pack
Game Checklist
```

Cần kiểm soát nội dung và bảo mật trước khi hỗ trợ tải preset từ Internet.

---

# 67. MVP – phiên bản đầu tiên

Không nên làm tất cả ngay từ đầu.

MVP nên tập trung vào:

```text
1. Import PNG / JPG / GIF
2. Hiển thị overlay
3. Move
4. Resize
5. Opacity
6. Always On Top
7. Click Through
8. Multiple Overlay
9. Lock Overlay
10. System Tray
11. Global Hotkey
12. Save Configuration
13. Restore Configuration
14. Startup with Windows
15. Multi Monitor
```

Đây là bộ chức năng đủ để tạo một sản phẩm có thể sử dụng thực tế.

---

# 68. Version 0.2

Bổ sung:

```text
Media Library
Profiles
Snap
Anchor
WebP
APNG
Overlay Group
Performance Mode
```

---

# 69. Version 0.3

```text
Scenes
Auto Scene
Application Rules
Text Overlay
Clock
Timer
Playlist
```

---

# 70. Version 1.0

```text
Profile Export / Import
Advanced Rules
Auto Update
Plugin API
Crash Recovery
Advanced Renderer
```

---

# 71. Roadmap đề xuất

```text
Phase 1
Core Overlay Engine
        ↓
Phase 2
Control Center
        ↓
Phase 3
Media Library
        ↓
Phase 4
Profiles
        ↓
Phase 5
Rules / Automation
        ↓
Phase 6
Widgets / Plugins
```

---

# 72. Thứ tự triển khai kỹ thuật

## Bước 1

Tạo một cửa sổ WPF:

```text
Transparent
Borderless
TopMost
```

## Bước 2

Hiển thị PNG.

## Bước 3

Thêm drag + resize.

## Bước 4

Thêm click-through bằng Win32 window style.

## Bước 5

Overlay Manager cho phép nhiều window.

## Bước 6

Save / Restore vị trí.

## Bước 7

GIF rendering.

## Bước 8

System Tray.

## Bước 9

Global Hotkey.

## Bước 10

Profiles.

---

# 73. Luồng sử dụng cơ bản

```text
Open App
   ↓
Import Media
   ↓
Select Media
   ↓
Add Overlay
   ↓
Move / Resize
   ↓
Set Opacity
   ↓
Enable Click Through
   ↓
Lock
   ↓
Save Automatically
```

---

# 74. Luồng khởi động Windows

```text
Windows Login
      ↓
OverlayCanvas Start
      ↓
Load config.json
      ↓
Load Last Profile
      ↓
Create Overlay Windows
      ↓
Minimize to System Tray
```

Nếu người dùng bật Delay Startup:

```text
Windows Login
      ↓
Wait configured delay
      ↓
Start OverlayCanvas
```

---

# 75. Luồng Edit Mode

```text
Global Hotkey
      ↓
Edit Mode ON
      ↓
Disable Click Through
      ↓
Show Border / Resize Handles
      ↓
User Edit
      ↓
Exit Edit Mode
      ↓
Restore Click Through
```

---

# 76. Những vấn đề cần lưu ý

## Exclusive Fullscreen

Không đảm bảo overlay sẽ hiển thị phía trên ứng dụng chạy exclusive fullscreen.

## UAC / Secure Desktop

Overlay bình thường không được kỳ vọng xuất hiện phía trên màn hình bảo mật của Windows.

## Anti-cheat

Một số game có thể hạn chế overlay hoặc topmost window.

Do đó ứng dụng không nên quảng bá là tương thích với mọi game.

## Taskbar

Hành vi taskbar và Start Menu có thể khác giữa các bản Windows.

## DPI

Phải kiểm tra các mức:

```text
100%
125%
150%
175%
200%
```

---

# 77. Kiểm thử cần thực hiện

## Functional Test

- Import PNG.
- Import GIF.
- Add overlay.
- Delete overlay.
- Resize.
- Move.
- Lock.
- Click-through.
- Always on top.

## Multi Overlay Test

```text
1 overlay
5 overlays
10 overlays
20 overlays
```

## Performance Test

Kiểm tra:

```text
CPU
GPU
RAM
FPS
```

với:

```text
1 GIF
5 GIF
10 GIF
```

## Multi Monitor Test

```text
1 monitor
2 monitors
Different DPI
Different resolution
```

## Startup Test

```text
Startup enabled
Startup disabled
Restore profile
Start minimized
```

---

# 78. Tiêu chí hiệu năng đề xuất

Một phiên bản ổn định nên hướng tới:

```text
Idle CPU      < 1%
Static PNG    gần 0% CPU
Startup       nhanh
UI responsive
No continuous polling nếu không cần thiết
```

Không nên dùng timer 1–10 ms cho toàn bộ ứng dụng.

Các component nên hoạt động theo event khi có thể.

---

# 79. Điểm khác biệt có thể tạo cho sản phẩm

Các ứng dụng overlay thường chỉ tập trung vào một mục đích riêng.

OverlayCanvas có thể khác biệt nhờ:

```text
Media Overlay
+
Scene Manager
+
Application Rules
+
Widgets
+
Performance Optimization
```

Điểm mạnh nên tập trung là:

> Một công cụ overlay tổng quát, nhẹ, dễ sử dụng và có khả năng tự thay đổi theo ngữ cảnh người dùng.

---

# 80. Định hướng UI/UX

Phong cách đề xuất:

```text
Windows 11
Fluent style
Rounded corner
Dark Mode
Minimal
```

Nguyên tắc:

- Không đưa quá nhiều tùy chọn ra màn hình chính.
- Các chức năng nâng cao nằm trong Advanced.
- Thao tác phổ biến không quá 2–3 click.
- Có tooltip cho các chức năng như Click-through và Taskbar Overlay.

---

# 81. Kết luận

OverlayCanvas nên bắt đầu như một ứng dụng Windows nhỏ với nhiệm vụ cốt lõi:

> Hiển thị nhiều nội dung PNG/GIF/WebP trên màn hình dưới dạng lớp phủ nhẹ, trong suốt, có khả năng click-through và luôn nổi trên ứng dụng khác.

Kiến trúc được đề xuất:

```text
C#
.NET 10 LTS
WPF
MVVM
Win32 API
JSON Configuration
```

Sau MVP, dự án có thể phát triển dần thành một nền tảng overlay hoàn chỉnh với:

```text
Profiles
Scenes
Widgets
Application Rules
Overlay Groups
Auto Scene
Plugins
```

Nếu giữ Overlay Engine độc lập với UI ngay từ đầu, dự án sẽ dễ mở rộng và dễ tối ưu hơn rất nhiều.

---

# 82. Nguồn kỹ thuật tham khảo

Các tài liệu Microsoft nên tham khảo khi triển khai:

- Microsoft Learn — `SetWindowPos`: điều khiển vị trí và Z-order của cửa sổ.
  - https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowpos

- Microsoft Learn — Extended Window Styles: `WS_EX_TOOLWINDOW`, `WS_EX_TOPMOST`, `WS_EX_TRANSPARENT` và các window style liên quan.
  - https://learn.microsoft.com/windows/win32/winmsg/extended-window-styles

- Microsoft Learn — Startup Apps: các cơ chế ứng dụng khởi động cùng Windows và cách Windows đánh giá startup impact.
  - https://learn.microsoft.com/windows/win32/w8cookbook/startup-apps

- .NET Support Policy: .NET 10 là bản LTS đang được hỗ trợ tại thời điểm tài liệu này được soạn.
  - https://dotnet.microsoft.com/platform/support/policy

---

**Tài liệu định hướng dự án — OverlayCanvas**  
**Phiên bản tài liệu:** 1.0  
**Ngày:** 24/09/2026
