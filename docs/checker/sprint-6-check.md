# Sprint 6 — Nhật ký kiểm tra

Ngày thực hiện: 25/09/2026

## Đã triển khai

- Chốt product/file metadata và version MVP `0.1.0`; executable là `AuroraDesktopOverlay.exe`.
- Thêm chế độ `--smoke-test` khởi tạo host, monitor và config repository rồi thoát không mở UI/engine.
- Accessibility cơ bản: access key, keyboard tab cycle, automation name/live region, tooltip cho click-through/Edit Mode/Z-order, focus mặc định và panel có scroll.
- Control Center có min-size để tránh mất controls ở cửa sổ nhỏ.
- User guide, troubleshooting, privacy/security, known limitations và RC checklist.
- Script `build/Publish.ps1` tạo self-contained single-file portable ZIP và SHA-256.
- Inno Setup recipe tạo installer per-user, không yêu cầu admin và không xóa AppData khi uninstall.
- GitHub Release workflow: restore/build/test, package portable+installer, smoke test, checksum, upload artifact và tạo release từ tag.
- CI bổ sung publish check để lỗi đóng gói được phát hiện trên pull request.

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode`.
- [x] `dotnet format --verify-no-changes --no-restore`.
- [x] `dotnet build -c Release --no-restore` — 0 warning, 0 error.
- [x] Toàn bộ unit/integration tests — 24/24 test đạt.
- [x] `build/Publish.ps1 -Version 0.1.0` tạo portable ZIP/checksum; SHA-256 khớp.
- [x] Published executable chạy `--smoke-test` và exit code 0.
- [ ] Installer compile bằng Inno Setup 6.

Máy xác minh hiện tại chưa cài Inno Setup 6, vì vậy installer vẫn là release gate chưa hoàn tất.

## Ma trận kiểm thử thủ công bắt buộc trước `v0.1.0`

| Nhóm | Ma trận | Trạng thái |
|---|---|---|
| OS | Windows 10 22H2 x64; Windows 11 x64 | Chưa chạy |
| DPI | 100%, 125%, 150%, 175%, 200% | Chưa chạy |
| Monitor | 1 màn hình; 2 màn hình cùng/khác DPI; hot-plug | Chưa chạy |
| Overlay count | 1, 5, 10, 20 PNG | Chưa chạy |
| GIF count | 1, 5, 10; visible/hidden/opacity 0 | Chưa chạy |
| Lifecycle | sleep/wake; Explorer restart; sign out; shutdown | Chưa chạy |
| Packaging | clean install; upgrade; uninstall; reinstall; portable | Chưa chạy |

## Performance/soak gate

| Kịch bản | Mục tiêu | Kết quả |
|---|---:|---:|
| Idle, không overlay | CPU < 1%; RAM 50–150 MB | Chưa đo |
| 1 PNG tĩnh | CPU gần 0% | Chưa đo |
| 1/5/10 GIF | Ghi CPU/GPU/RAM/FPS | Chưa đo |
| Soak 8 giờ | Không crash/leak tăng liên tục | Chưa chạy |

## Release blockers còn mở

- Chưa chọn/thêm `LICENSE`; không nên phát hành public trước khi quyết định quyền phân phối.
- Chưa có code-signing certificate; installer/portable hiện dự kiến unsigned và có thể bị SmartScreen cảnh báo.
- Installer UI và uninstall-preserves-AppData cần máy có Inno Setup và kiểm thử thủ công.
- Chưa hoàn thành Windows 10/11, DPI, multi-monitor và performance matrix.

## Chính sách go/no-go

- Không tag `v0.1.0` nếu còn P0/P1, test tự động đỏ hoặc chưa chứng minh config/media sống sót qua upgrade/uninstall.
- Có thể tag `v0.1.0-rc.1` để chạy beta sau khi portable/installer và smoke test đạt.
- P2 chỉ được giữ khi có workaround, issue, owner và ghi trong known limitations.
