# Giai đoạn 5 — Ổn định sản phẩm (`v1.0.0`)

## Sprint 11: Chia sẻ, backup và crash recovery

Mục tiêu: dữ liệu người dùng có thể di chuyển và phục hồi an toàn.

### Backlog

- Export/import profile/scene bằng package có manifest, schema version và checksum.
- Validate path, file type, kích thước và chống path traversal; không chạy script trong package.
- Backup rotation và restore có preview; migration có rollback.
- Phát hiện shutdown bất thường; Restore previous session/Start Safe Mode.
- Undo/redo cho thao tác layout quan trọng trong phiên hiện tại.
- Test corrupt/partial/malicious package và disk full/read-only.

### Tiêu chí nghiệm thu

- Import package không thể ghi ra ngoài thư mục ứng dụng cho phép.
- Crash giả lập không làm mất bản cấu hình tốt cuối cùng; Safe Mode không tự load overlay.
- Package round-trip giữ đúng scene/profile trên máy sạch.

## Sprint 12: Update, plugin API tối thiểu và security review

Mục tiêu: tạo đường nâng cấp bền vững và extension contract có kiểm soát.

### Backlog

- Check-for-updates qua GitHub Releases; tải/cài chỉ sau xác nhận của người dùng.
- Kênh stable/pre-release, semantic versioning và kiểm tra checksum/signature.
- Thiết kế plugin API versioned tối thiểu; lifecycle, capability và lỗi cô lập ở mức khả thi.
- Plugin mặc định bị tắt; chỉ load từ vị trí tin cậy và hiển thị cảnh báo rõ ràng.
- Threat modeling cho import, updater, plugin loading, media decoder và AppData permissions.
- Dependency/license audit; sửa lỗi bảo mật và thêm regression test.

### Tiêu chí nghiệm thu

- Updater không tự thực thi artifact không hợp lệ và xử lý offline/timeout an toàn.
- Plugin lỗi không làm hỏng cấu hình cốt lõi; compatibility contract được document.
- Không còn finding Critical/High chưa xử lý hoặc chưa có quyết định chấp nhận rủi ro.

## Sprint 13: Release candidate và General Availability

Mục tiêu: đóng băng tính năng, xử lý lỗi và phát hành `v1.0.0` có thể hỗ trợ lâu dài.

### Backlog

- Feature freeze; full regression và exploratory testing theo persona.
- Soak/performance/leak test; startup, sleep/wake, Explorer restart, display hot-plug.
- Test clean install, upgrade lần lượt từ `v0.1`, `v0.2`, `v0.3`, uninstall và reinstall.
- Hoàn thiện onboarding, user guide, FAQ, troubleshooting, architecture và contributor docs.
- Accessibility pass, UI polish, error copy và telemetry/privacy review (nếu có telemetry).
- Triage issue; đóng hoặc gắn milestone sau 1.0, công bố known issues.
- Build RC, nhận phản hồi beta, sửa lỗi chặn, build lại từ tag sạch và phát hành GA.

### Tiêu chí phát hành `v1.0.0`

- Không lỗi P0/P1; P2 chỉ được giữ khi có workaround và maintainer chấp thuận bằng văn bản.
- CI, security scan, full regression, performance budget và migration matrix đều xanh.
- Installer/portable chạy trên Windows 10/11 x64 sạch; artifact có checksum và chữ ký nếu chứng thư sẵn có.
- Tag, source archive, release notes, upgrade/rollback guide và support policy xuất hiện trên GitHub.
- Tạo milestone bảo trì `v1.0.x` và roadmap sau 1.0; không đưa Browser Overlay/marketplace vào phút cuối.
