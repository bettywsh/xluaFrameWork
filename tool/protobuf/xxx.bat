@echo off
 
cd /d %~dp0
 
set pbdest=..\..\proto
for /r %%i in (*.proto) do (
	
	protoc.exe -I %~dp0 -o%pbdest%\%%~ni.pb %%i	
 
)
pause