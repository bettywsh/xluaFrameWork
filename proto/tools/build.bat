

::lua
XCOPY /y ..\proto\*.proto .\
::XCOPY /y ..\proto\lua\*.proto .\

echo return {>> lua\ProtoFlies.lua.bytes

FOR /F "delims==" %%i IN ('dir /b *.proto') DO (
	@echo %%i
	"protoc.exe" -o lua\%%~ni.pc.bytes %%i
	echo "%%~ni",>> lua\ProtoFlies.lua.bytes
	
)
echo }>> lua\ProtoFlies.lua.bytes

del/F /S /Q *.proto

::cs
XCOPY /y ..\proto\*.proto .\
::XCOPY /y ..\proto\cs\*.proto .\
FOR /F "delims==" %%i IN ('dir /b *.proto') DO (
	@echo %%i
	".\protogen\protogen.exe" --csharp_out=.\cs %%i
)


::拷贝到项目
::set LuaPath=D:\MyFrameWork\xluaFrameWork\client\Assets\App\Lua\Protobuf
XCOPY /y .\lua\*.pc.bytes .\..\..\client\Assets\App\Protobuf
XCOPY /y .\lua\*.lua.bytes .\..\..\client\Assets\App\Lua\3rd\lua-protobuf\
XCOPY /y .\cs\*.cs .\..\..\client\Assets\Script\Net\Protobuf

del/F /S /Q *.proto
del/F /S /Q .\cs\*.cs
del/F /S /Q .\lua\*.pc.bytes
del/F /S /Q .\lua\*.lua.bytes
pause