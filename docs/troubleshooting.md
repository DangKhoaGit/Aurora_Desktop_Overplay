# Aurora Desktop Overlay — Troubleshooting

## Không thấy overlay

- Mở Control Center bằng `Ctrl+Shift+O`, chọn overlay và kiểm tra **Visible** cùng opacity.
- Chọn **Bring to front** và bật **Always on top**.
- Với game, dùng borderless fullscreen; exclusive fullscreen có thể không hiển thị cửa sổ overlay thông thường.
- Dùng **Reset position** nếu cấu hình màn hình vừa thay đổi.

## Không click/chỉnh được overlay

Bật Edit Mode bằng `Ctrl+Shift+E`. Chế độ này tạm tắt click-through. Nếu chỉ muốn cố định, bỏ click-through rồi bỏ Locked cho overlay đang chọn.

## Hotkey không hoạt động

Control Center sẽ báo tổ hợp bị ứng dụng khác chiếm. Thoát/chỉnh hotkey của ứng dụng kia và khởi động lại ADO. Bản MVP chưa có UI đổi tổ hợp.

## Session không khôi phục đầy đủ

ADO bỏ qua riêng media bị xóa/corrupt. Kiểm tra `%LocalAppData%\AuroraDesktopOverlay\media` và log. Khi `config.json` hỏng, ADO thử `backups\config.backup.json`.

## Thu thập log

Log nằm tại `%LocalAppData%\AuroraDesktopOverlay\logs`. Trước khi chia sẻ, kiểm tra và xóa đường dẫn/tên file riêng tư nếu cần. Không đăng media cá nhân kèm bug report.
