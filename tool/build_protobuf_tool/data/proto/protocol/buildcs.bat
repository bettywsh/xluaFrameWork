::É¾³ý¾ÉµÄ
del/F /S /Q ..\..\lua\*.lua
del/F /S /Q ..\..\lua\*.pc
del/F /S /Q ..\..\cpp\*.h
del/F /S /Q ..\..\cpp\*.cc
del/F /S /Q ..\..\cs\*.cs
del/F /S /Q *.proto

XCOPY /y ..\..\..\..\..\serverdata\Common\*.proto .\

FOR /F "delims==" %%i IN ('dir /b *.proto') DO (
@echo %%i
"protogen.exe" -i:%%i -o:..\..\cs\%%~ni.cs -ns:ProtoMsgs -p:detectMissing
rem  -ns:ProtoMsgs
)
XCOPY /y ..\..\cs\*.cs ..\..\..\..\..\..\ClientSrc\Assets\_Script\Game\MsgProcessor\ProtoDefine\

del/F /S /Q ..\..\lua\*.lua
del/F /S /Q ..\..\lua\*.pc
del/F /S /Q ..\..\cpp\*.h
del/F /S /Q ..\..\cpp\*.cc
del/F /S /Q ..\..\cs\*.cs
del/F /S /Q *.proto