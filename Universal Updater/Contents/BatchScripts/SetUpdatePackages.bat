set CacheFolder=%~1
set ScriptLocation=%~2
set Packages=%~3

C:\Windows\Servicing\UpdateApp.exe install "%Packages%" >>"%CacheFolder%\Result.txt"

echo.>"%CacheFolder%\End3.txt"
