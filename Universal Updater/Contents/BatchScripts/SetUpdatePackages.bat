set LocalFolder=%~1
set Packages=%~2
set FromSD=%~3

if %FromSD% == 1 (C:\Windows\Servicing\UpdateApp.exe installfromsd "%Packages%" >>"%LocalFolder%\Result.txt") else (C:\Windows\Servicing\UpdateApp.exe install "%Packages%" >>"%LocalFolder%\Result.txt")

echo.>"%LocalFolder%\End3.txt"
