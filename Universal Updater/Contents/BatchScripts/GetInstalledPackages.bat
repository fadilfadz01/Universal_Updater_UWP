set CacheFolder=%~1
set InstalledLocation=%~2

C:\Windows\Servicing\UpdateApp.exe GetInstalledPackages >"%CacheFolder%\InstalledPackages.txt"
for /f "tokens=1 delims=," %%a in ('findstr /i "Microsoft" "%CacheFolder%\InstalledPackages.txt"') do echo>>"%CacheFolder%\Output2.txt" %%a
for /f "tokens=*" %%a in ('findstr /i "Microsoft" "%CacheFolder%\Output2.txt"') do echo>>"%CacheFolder%\FilteredPackages.txt" %%a

echo.>"%CacheFolder%\End2.txt"
