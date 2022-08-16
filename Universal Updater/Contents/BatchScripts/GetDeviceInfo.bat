setlocal enabledelayedexpansion
set CacheFolder=%~1
set ScriptLocation=%~2

copy %ScriptLocation%\Contents\UpdateApp.exe C:\Windows\Servicing\UpdateApp.exe

for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v CurrentMajorVersionNumber /t REG_DWORD') do set /a "CurrentMajorVersionNumber=%%a"&goto 1
:1
for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v CurrentMinorVersionNumber /t REG_DWORD') do set /a "CurrentMinorVersionNumber=%%a"&goto 2
:2
for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v CurrentBuildNumber /t REG_SZ') do set "CurrentBuildNumber=%%a"&goto 3
:3
for /f "skip=2 tokens=3" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v UBR /t REG_DWORD') do set /a "UBR=%%a"&goto 4
:4

for /f "skip=2 tokens=1,3,*" %%a in ('reg query HKLM\System\Platform\DeviceTargetingInfo /t REG_SZ') do echo>>"%CacheFolder%\Output1.txt" %%a: %%b %%c

for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneFriendlyName" "%CacheFolder%\Output1.txt"') do (set FriendlyName=%%a
set FriendlyName=!FriendlyName:"=!
echo>"%CacheFolder%\DeviceInfo.txt" FriendlyName: !FriendlyName!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneHardwareVariant" "%CacheFolder%\Output1.txt"') do (set HardwareVariant=%%a
set HardwareVariant=!HardwareVariant:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" HardwareVariant: !HardwareVariant!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneManufacturer" "%CacheFolder%\Output1.txt"') do (set Manufacturer=%%a
set Manufacturer=!Manufacturer:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" Manufacturer: !Manufacturer!
echo.>>"%CacheFolder%\DeviceInfo.txt"
goto 5)
:5
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneManufacturerDisplayName" "%CacheFolder%\Output1.txt"') do (set ManufacturerDisplayName=%%a
set ManufacturerDisplayName=!ManufacturerDisplayName:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" ManufacturerDisplayName: !ManufacturerDisplayName!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneManufacturerModelName" "%CacheFolder%\Output1.txt"') do (set ManufacturerModelName=%%a
set ManufacturerModelName=!ManufacturerModelName:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" ManufacturerModelName: !ManufacturerModelName!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneMobileOperatorName" "%CacheFolder%\Output1.txt"') do (set MobileOperatorName=%%a
set MobileOperatorName=!MobileOperatorName:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" MobileOperatorName: !MobileOperatorName!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneModelName" "%CacheFolder%\Output1.txt"') do (set ModelName=%%a
set ModelName=!ModelName:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" ModelName: !ModelName!
echo.>>"%CacheFolder%\DeviceInfo.txt")
echo>>"%CacheFolder%\DeviceInfo.txt" OS Version: %CurrentMajorVersionNumber%.%CurrentMinorVersionNumber%.%CurrentBuildNumber%.%UBR%
echo.>>"%CacheFolder%\DeviceInfo.txt"
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneSOCVersion" "%CacheFolder%\Output1.txt"') do (set SOCVersion=%%a
set SOCVersion=!SOCVersion:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" SOC: !SOCVersion!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneFirmwareRevision" "%CacheFolder%\Output1.txt"') do (set FirmwareRevision=%%a
set FirmwareRevision=!FirmwareRevision:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" ROM FirmwareRevision: !FirmwareRevision!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneHardwareRevision" "%CacheFolder%\Output1.txt"') do (set HardwareRevision=%%a
set HardwareRevision=!HardwareRevision:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" HardwareRevision: !HardwareRevision!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneRadioSoftwareRevision" "%CacheFolder%\Output1.txt"') do (set RadioSoftwareRevision=%%a
set RadioSoftwareRevision=!RadioSoftwareRevision:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" RadioSoftwareRevision: !RadioSoftwareRevision!
echo.>>"%CacheFolder%\DeviceInfo.txt")
for /f "tokens=2 delims=:" %%a in ('findstr /r "PhoneROMLanguage" "%CacheFolder%\Output1.txt"') do (set ROMLanguage=%%a
set ROMLanguage=!ROMLanguage:"=!
echo>>"%CacheFolder%\DeviceInfo.txt" ROM Language: !ROMLanguage!)

echo.>"%CacheFolder%\End1.txt"
