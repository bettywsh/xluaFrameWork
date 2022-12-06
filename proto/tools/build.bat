::lua
XCOPY /y ..\proto\*.proto .\
XCOPY /y ..\proto\lua\*.proto .\

echo return = { >> lua\proto.lua.bytes

FOR /F "delims==" %%i IN ('dir /b *.proto') DO (
	@echo %%i
	"protoc.exe" -o lua\%%~ni.pc %%i
	echo     %%~ni, >> lua\proto.lua.bytes
	
)
echo } >> lua\proto.lua.bytes

del/F /S /Q *.proto

::cs
XCOPY /y ..\proto\*.proto .\
XCOPY /y ..\proto\cs\*.proto .\
FOR /F "delims==" %%i IN ('dir /b *.proto') DO (
	@echo %%i
	"protoc.exe" --csharp_out=.\cs %%i
)

del/F /S /Q *.proto
del/F /S /Q .\cs\*.cs
del/F /S /Q .\lua\*.pc
del/F /S /Q .\lua\*.lua.bytes
pause