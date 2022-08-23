setlocal enabledelayedexpansion
set LocalFolder=%~1
set InstalledLocation=%~2

copy %InstalledLocation%\Contents\UpdateApp.exe C:\Windows\Servicing\UpdateApp.exe

for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v CurrentMajorVersionNumber /t REG_DWORD') do set /a "CurrentMajorVersionNumber=%%a"&goto 1
:1
for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v CurrentMinorVersionNumber /t REG_DWORD') do set /a "CurrentMinorVersionNumber=%%a"&goto 2
:2
for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v CurrentBuildNumber /t REG_SZ') do set "CurrentBuildNumber=%%a"&goto 3
:3
for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v UBR /t REG_DWORD') do set /a "UBR=%%a"&goto 4
:4

for /f "skip=2 tokens=1,3,*" %%a in ('reg query HKLM\System\Platform\DeviceTargetingInfo /t REG_SZ') do echo>>"%LocalFolder%\Output1.txt" %%a: %%b %%c

for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneFriendlyName" "%LocalFolder%\Output1.txt"') do (set FriendlyName=%%a
set FriendlyName=!FriendlyName:"=!
echo>"%LocalFolder%\DeviceInfo.txt" FriendlyName: !FriendlyName!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneHardwareVariant" "%LocalFolder%\Output1.txt"') do (set HardwareVariant=%%a
set HardwareVariant=!HardwareVariant:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" HardwareVariant: !HardwareVariant!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneManufacturer" "%LocalFolder%\Output1.txt"') do (set Manufacturer=%%a
set Manufacturer=!Manufacturer:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" Manufacturer: !Manufacturer!
echo.>>"%LocalFolder%\DeviceInfo.txt"
goto 5)
:5
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneManufacturerDisplayName" "%LocalFolder%\Output1.txt"') do (set ManufacturerDisplayName=%%a
set ManufacturerDisplayName=!ManufacturerDisplayName:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" ManufacturerDisplayName: !ManufacturerDisplayName!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneManufacturerModelName" "%LocalFolder%\Output1.txt"') do (set ManufacturerModelName=%%a
set ManufacturerModelName=!ManufacturerModelName:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" ManufacturerModelName: !ManufacturerModelName!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneMobileOperatorName" "%LocalFolder%\Output1.txt"') do (set MobileOperatorName=%%a
set MobileOperatorName=!MobileOperatorName:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" MobileOperatorName: !MobileOperatorName!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneModelName" "%LocalFolder%\Output1.txt"') do (set ModelName=%%a
set ModelName=!ModelName:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" ModelName: !ModelName!
echo.>>"%LocalFolder%\DeviceInfo.txt")
echo>>"%LocalFolder%\DeviceInfo.txt" OS Version: %CurrentMajorVersionNumber%.%CurrentMinorVersionNumber%.%CurrentBuildNumber%.%UBR%
echo.>>"%LocalFolder%\DeviceInfo.txt"
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneSOCVersion" "%LocalFolder%\Output1.txt"') do (set SOCVersion=%%a
set SOCVersion=!SOCVersion:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" SOC: !SOCVersion!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneFirmwareRevision" "%LocalFolder%\Output1.txt"') do (set FirmwareRevision=%%a
set FirmwareRevision=!FirmwareRevision:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" ROM FirmwareRevision: !FirmwareRevision!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneHardwareRevision" "%LocalFolder%\Output1.txt"') do (set HardwareRevision=%%a
set HardwareRevision=!HardwareRevision:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" HardwareRevision: !HardwareRevision!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneRadioSoftwareRevision" "%LocalFolder%\Output1.txt"') do (set RadioSoftwareRevision=%%a
set RadioSoftwareRevision=!RadioSoftwareRevision:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" RadioSoftwareRevision: !RadioSoftwareRevision!
echo.>>"%LocalFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneROMLanguage" "%LocalFolder%\Output1.txt"') do (set ROMLanguage=%%a
set ROMLanguage=!ROMLanguage:"=!
echo>>"%LocalFolder%\DeviceInfo.txt" ROM Language: !ROMLanguage!)

echo.>"%LocalFolder%\End1.txt"
