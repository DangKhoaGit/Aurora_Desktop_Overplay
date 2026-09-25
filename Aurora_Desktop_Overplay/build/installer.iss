#ifndef MyAppVersion
  #define MyAppVersion "0.1.0"
#endif
#ifndef PublishDir
  #define PublishDir "..\artifacts\publish\win-x64"
#endif
#ifndef OutputDir
  #define OutputDir "..\artifacts\release"
#endif

[Setup]
AppId={{A79F606E-DBE6-45AD-9776-BF8D99C5159D}
AppName=Aurora Desktop Overlay
AppVersion={#MyAppVersion}
AppPublisher=Aurora Desktop Overlay contributors
DefaultDirName={localappdata}\Programs\Aurora Desktop Overlay
DefaultGroupName=Aurora Desktop Overlay
UninstallDisplayIcon={app}\AuroraDesktopOverlay.exe
OutputDir={#OutputDir}
OutputBaseFilename=AuroraDesktopOverlay-{#MyAppVersion}-win-x64-setup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern
CloseApplications=yes
RestartApplications=no
SetupLogging=yes

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Aurora Desktop Overlay"; Filename: "{app}\AuroraDesktopOverlay.exe"
Name: "{autodesktop}\Aurora Desktop Overlay"; Filename: "{app}\AuroraDesktopOverlay.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Run]
Filename: "{app}\AuroraDesktopOverlay.exe"; Description: "Launch Aurora Desktop Overlay"; Flags: nowait postinstall skipifsilent

; User media/config live under %LocalAppData%\AuroraDesktopOverlay and are intentionally
; not removed by uninstall. This prevents silent loss of imported media and layouts.
