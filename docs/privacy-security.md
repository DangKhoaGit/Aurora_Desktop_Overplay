# Privacy và Security

- ADO MVP không có telemetry, tài khoản, cloud sync hoặc quảng cáo.
- Media, config, backup và log chỉ được lưu cục bộ trong `%LocalAppData%\AuroraDesktopOverlay`.
- ADO không process injection, không đọc memory game, không DLL/DirectX hook và không cố vượt anti-cheat.
- Ứng dụng chạy quyền người dùng thông thường (`asInvoker`), không yêu cầu Administrator cho luồng chuẩn.
- Media import được kiểm tra extension, dung lượng và giới hạn decoded pixels. ADO không chạy script từ profile/media.
- Log không ghi từng frame; log lỗi có thể chứa tên/đường dẫn file cục bộ. Người dùng nên kiểm tra trước khi chia sẻ.
- Chỉ tải installer/portable từ GitHub Release chính thức và kiểm tra file `.sha256`.
- Bản `v0.1.0` chưa ký số; cảnh báo SmartScreen không thay thế việc kiểm tra URL tải xuống và SHA-256.

Quy trình báo cáo lỗ hổng nằm trong `Aurora_Desktop_Overplay/SECURITY.md`.
