# Kế hoạch sprint — OverlayCanvas

Tài liệu này chuyển [đặc tả sản phẩm](../Desktop_Overlay_Project_Spec.md) thành kế hoạch triển khai từ repository trống đến bản `v1.0.0` được phát hành trên GitHub.

## 1. Giả định lập kế hoạch

- Nhóm chuẩn: 2 lập trình viên, 1 QA/thiết kế bán thời gian. Nếu chỉ có 1 người, giữ nguyên thứ tự nhưng tăng thời lượng khoảng 1,5–2 lần.
- Mỗi sprint dài 2 tuần; Sprint 0 dài 1 tuần. Tổng dự kiến: 27 tuần, chưa tính thời gian chờ chứng thư ký mã.
- Nền tảng: Windows 10/11 x64, C#, .NET 10 LTS, WPF, MVVM, Win32 API, lưu trữ JSON.
- `main` luôn ở trạng thái có thể phát hành; nhánh công việc tồn tại ngắn và đi qua pull request.
- MVP không inject process, không đọc memory game, không hook DirectX và không cam kết hỗ trợ exclusive fullscreen.

## 2. Mốc sản phẩm

| Giai đoạn | Sprint | Đầu ra chính | Mốc phát hành |
|---|---:|---|---|
| Khởi tạo | 0 | Kiến trúc, CI, quy ước, spike kỹ thuật | — |
| Overlay Engine | 1–3 | Cửa sổ overlay, Win32, PNG/JPG/GIF, nhiều overlay | `v0.1.0-alpha.1` |
| MVP dùng được | 4–6 | Control Center, lưu/khôi phục, tray, hotkey, startup, đa màn hình | `v0.1.0` |
| Quản lý nội dung | 7–8 | Media Library, profile, snap/anchor, WebP/APNG, hiệu năng | `v0.2.0` |
| Tự động hóa & widget | 9–10 | Scene, rules, text/clock/timer, playlist | `v0.3.0` |
| Ổn định 1.0 | 11–13 | Import/export, recovery, update, plugin API, hardening | `v1.0.0` |

Chi tiết:

- [Giai đoạn 0 — Khởi tạo](00-khoi-tao.md)
- [Giai đoạn 1 — Overlay Engine](01-overlay-engine.md)
- [Giai đoạn 2 — MVP hoàn chỉnh](02-mvp.md)
- [Giai đoạn 3 — Phiên bản 0.2](03-version-0.2.md)
- [Giai đoạn 4 — Phiên bản 0.3](04-version-0.3.md)
- [Giai đoạn 5 — Phiên bản 1.0](05-version-1.0.md)
- [Quy trình GitHub và phát hành](06-github-release.md)

## 3. Nhịp làm việc mỗi sprint

1. Planning: chốt mục tiêu, capacity, dependency và acceptance criteria.
2. Thực hiện: issue → nhánh → code/test → pull request → review.
3. Kiểm thử giữa sprint trên Windows 10 và 11; không dồn toàn bộ QA về cuối.
4. Review/demo bằng kịch bản người dùng có thể chạy lại.
5. Retrospective và cập nhật rủi ro, technical debt, số liệu hiệu năng.

## 4. Definition of Ready

Một issue chỉ được đưa vào sprint khi có:

- Mô tả giá trị người dùng hoặc kết quả kỹ thuật rõ ràng.
- Acceptance criteria kiểm chứng được và phạm vi ngoài yêu cầu.
- Phụ thuộc, thiết kế UI/API và dữ liệu mẫu cần thiết.
- Ước lượng; issue lớn hơn 3 ngày phải tách nhỏ.

## 5. Definition of Done

Một hạng mục chỉ hoàn thành khi:

- Code build sạch ở cấu hình Release; không có cảnh báo mới chưa được giải thích.
- Unit test và integration test liên quan đã qua; luồng UI quan trọng có checklist kiểm thử thủ công.
- Đã review qua pull request, cập nhật tài liệu và changelog khi hành vi người dùng thay đổi.
- Không log dữ liệu nhạy cảm; input/file được validate và lỗi có thông báo phù hợp.
- Không làm xấu đi ngân sách hiệu năng đã chốt; có số đo trước/sau cho thay đổi renderer.
- Acceptance criteria đã được Product Owner/maintainer xác nhận.

## 6. Cổng chất lượng chung

| Cổng | Yêu cầu tối thiểu |
|---|---|
| Build | `dotnet restore`, `dotnet build -c Release` thành công trên CI |
| Test | Unit/integration test xanh; không bỏ qua test quan trọng không có issue theo dõi |
| Hiệu năng | Idle CPU < 1%; PNG tĩnh gần 0% CPU; RAM app chính mục tiêu 50–150 MB |
| Tương thích | Windows 10/11 x64; DPI 100/125/150/175/200%; ít nhất cấu hình 1 và 2 màn hình |
| An toàn | Không injection/hook; validate loại và kích thước media; dependency/security scan xanh |
| Release | Smoke test gói cài đặt và portable trên máy sạch; checksum và release notes đầy đủ |

## 7. Rủi ro cần theo dõi từ đầu

| Rủi ro | Cách giảm thiểu | Sprint xác minh |
|---|---|---:|
| WPF transparent window/GIF dùng CPU cao | Spike renderer, benchmark cố định, pause khi ẩn | 0–3 |
| Click-through, Z-order khác giữa bản Windows | Bọc Win32 sau interface; test matrix | 1–2 |
| Sai vị trí khi đổi DPI/màn hình | Dùng tọa độ theo monitor/anchor; mô phỏng hot-plug | 3, 6, 8 |
| File media lỗi hoặc quá lớn | Decode an toàn, giới hạn kích thước, fallback thumbnail | 3, 7 |
| Mất/corrupt cấu hình | Atomic write, schema version, backup/migration | 4, 11 |
| False expectation với game fullscreen | Ghi rõ borderless/exclusive và anti-cheat policy | Mọi release |
| Chứng thư ký mã chưa có | Chuẩn bị unsigned build rõ cảnh báo; xin certificate sớm | 0, 13 |

## 8. Quy tắc thay đổi phạm vi

- Tính năng ngoài sprint không chen vào nếu không phải lỗi chặn release hoặc vấn đề bảo mật.
- Browser overlay, audio reactive, preset store và plugin marketplace nằm ngoài `v1.0.0`.
- Desktop Layer/force-above-taskbar chỉ phát hành khi test đủ trên Windows 10/11; nếu không đạt sẽ giữ sau feature flag hoặc hoãn.
- Mỗi mốc có thể lùi ngày, nhưng không bỏ cổng chất lượng để giữ ngày phát hành.
