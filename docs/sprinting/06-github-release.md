# Quy trình GitHub và phát hành

## 1. Khởi tạo repository

Repository hiện tại cần được khởi tạo Git trước khi có thể đẩy lên GitHub. Các bước dự kiến sau khi chủ dự án chọn tên repository, tài khoản/organization, visibility và license:

```powershell
git init -b main
git add .
git commit -m "chore: initialize OverlayCanvas project"
gh repo create <owner>/<repo> --source . --remote origin --push
```

Không commit secret, certificate, token, file người dùng trong AppData, binary build hoặc media có bản quyền. Dùng GitHub CLI/browser authentication; không ghi token vào remote URL hay file repository.

## 2. Cấu trúc quản trị đề xuất

- Branch chính: `main`; bảo vệ branch, yêu cầu pull request, CI xanh và ít nhất 1 approval nếu có nhiều maintainer.
- Nhánh ngắn: `feat/<issue>-<slug>`, `fix/<issue>-<slug>`, `docs/<issue>-<slug>`.
- Commit: Conventional Commits để tạo changelog rõ ràng.
- Labels: `type:*`, `priority:P0..P3`, `area:*`, `status:*`, `good first issue`.
- Milestones: `v0.1.0`, `v0.2.0`, `v0.3.0`, `v1.0.0`.
- GitHub Projects: cột Backlog, Ready, In progress, In review, Verify, Done.

## 3. CI cho pull request

Pipeline Windows phải thực hiện tối thiểu:

1. Checkout với SDK .NET cố định bằng `global.json`.
2. Restore bằng lock file và cache NuGet phù hợp.
3. Format/analyzer check.
4. Build Release.
5. Unit/integration test và upload test results.
6. Kiểm tra dependency/license/security; CodeQL theo lịch và trên thay đổi phù hợp.
7. Build package thử nghiệm để phát hiện lỗi packaging sớm.

UI/Win32 test khó chạy headless được tách thành workflow trên máy Windows test/self-hosted hoặc checklist release có bằng chứng; không giả vờ coi unit test là đủ.

## 4. Version và nhánh phát hành

- Dùng Semantic Versioning: `MAJOR.MINOR.PATCH`.
- Pre-release: `v0.1.0-alpha.1`, `v1.0.0-rc.1`.
- Không duy trì nhánh release dài trước 1.0; tạo `release/1.0` chỉ khi cần ổn định RC song song.
- Mỗi release tag là annotated tag, trỏ đúng commit đã qua CI; artifact phải được build lại từ tag/workflow, không upload binary build thủ công không truy vết được.

## 5. Checklist phát hành

### Trước khi tag

- Chốt milestone; không còn issue chặn và mọi exception chất lượng đã được ghi nhận.
- Cập nhật version, `CHANGELOG`, release notes, known issues và migration notes.
- Chạy full matrix Windows/DPI/monitor, smoke test installer/portable và security scan.
- Test upgrade từ phiên bản public gần nhất và rollback/backup dữ liệu.
- Xác nhận license của dependency và asset; tạo SBOM nếu pipeline hỗ trợ.

### Tạo release

1. Merge pull request phát hành vào `main`.
2. CI build từ commit sạch và tạo tag `vX.Y.Z` theo workflow được bảo vệ.
3. Đính kèm installer, portable archive, checksum SHA-256, SBOM và signature nếu có.
4. GitHub Release mô tả highlights, breaking changes, cách nâng cấp, giới hạn và link issue tracker.
5. Đánh dấu pre-release cho alpha/beta/RC; chỉ đánh dấu Latest khi GA đã qua smoke test.

### Sau phát hành

- Tải artifact công khai về máy sạch và smoke test lần cuối.
- Theo dõi crash/issue 48–72 giờ; lỗi P0 có thể kích hoạt hotfix và gỡ recommendation của release lỗi.
- Đóng milestone, chuyển issue còn lại sang mốc kế tiếp và đăng post-release notes nếu cần.
- Retrospective: lead time, escaped defects, hiệu năng, download/update failure và hành động cải tiến.

## 6. Mẫu nội dung GitHub Release

```markdown
## Highlights
- ...

## Added / Changed / Fixed
- ...

## Known limitations
- Exclusive fullscreen có thể không hiển thị overlay.
- Ứng dụng không inject hoặc hook game; khả năng tương thích tùy game/anti-cheat.

## Install / Upgrade
- ...

## Verification
- SHA-256: ...

Full changelog: vPREVIOUS...vCURRENT
```

## 7. Khi nào coi dự án “đã up lên GitHub” hoàn chỉnh

- Repository có commit history, license, README, contributing/security policy và remote `origin` đúng.
- Branch protection và CI hoạt động trên một pull request thử nghiệm.
- Issues/milestones/project phản ánh backlog sprint, không chỉ có source code.
- Release tương ứng có tag, source, binary, checksum, release notes và hướng dẫn cài đặt.
- Một người khác có thể clone, build, test và tạo package chỉ bằng tài liệu trong repository.
