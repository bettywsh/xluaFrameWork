del/F /S /Q ..\..\lua\*.lua
del/F /S /Q ..\..\lua\*.pc
del/F /S /Q ..\..\cpp\*.h
del/F /S /Q ..\..\cpp\*.cc
del/F /S /Q *.proto

::ecport Server+Client
XCOPY /y ..\..\..\..\..\proto\*.proto .\
FOR /F "delims==" %%i IN ('dir /b *.proto') DO (
@echo %%i
::"protoc.exe" --plugin=protoc-gen-lua="..\\..\\..\\protoc-gen-lua.bat" --lua_out=..\..\lua %%i
"protoc.exe" -o ..\..\lua\%%~ni.pc %%i
"protoc.exe" -I.  --cpp_out=..\..\cpp %%i
)
XCOPY /y ..\..\..\..\..\proto\msgdef.proto ..\..\..\..\..\client\Assets\App\Protobuf\
::XCOPY /y ..\..\lua\*.lua ..\..\..\..\..\..\ClientSrc\Assets\Lua\Game\JywGame\ProtoDefine\
XCOPY /y ..\..\lua\*.pc ..\..\..\..\..\client\Assets\App\Protobuf\

::ecport ServerOnly
XCOPY /y ..\..\..\..\..\proto\ServerOnly\*.proto .\
FOR /F "delims==" %%i IN ('dir /b *.proto') DO (
@echo %%i
"protoc.exe" -I.  --cpp_out=..\..\cpp %%i
)
XCOPY /y ..\..\cpp\*.h  ..\..\..\..\..\server\server\Message\
XCOPY /y ..\..\cpp\*.cc ..\..\..\..\..\server\server\Message\

del/F /S /Q *.proto
del/F /S /Q ..\..\lua\*.lua
del/F /S /Q ..\..\lua\*.pc
del/F /S /Q ..\..\cpp\*.h
del/F /S /Q ..\..\cpp\*.cc
