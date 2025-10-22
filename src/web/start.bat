@echo off
cd /d "%~dp0"
echo local: %cd%
echo adicionando node ao PATH...

set "NODE_PATH=%~dp0..\..\..\nodejs"
set "PATH=%NODE_PATH%;%PATH%"

echo runing npm...
call "%NODE_PATH%\npm.cmd" run dev

echo -------------------------
echo end of the script
pause >nul
