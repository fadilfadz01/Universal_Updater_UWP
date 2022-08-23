set LocalFolder=%~1

C:\Windows\Servicing\UpdateApp.exe GetInstalledPackages >"%LocalFolder%\InstalledPackages.txt"
for /f "tokens=1 delims=," %%a in ('findstr /i "Microsoft" "%LocalFolder%\InstalledPackages.txt"') do echo>>"%LocalFolder%\Output2.txt" %%a
for /f "tokens=*" %%a in ('findstr /i "Microsoft" "%LocalFolder%\Output2.txt"') do echo>>"%LocalFolder%\FilteredPackages.txt" %%a

echo.>"%LocalFolder%\End2.txt"
