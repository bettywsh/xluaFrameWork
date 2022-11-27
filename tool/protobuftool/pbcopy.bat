@echo off

for /f "usebackq tokens=*" %%d in (`dir /s /b *.pb`) do (
 xcopy %%d ..\..\client\Main\Assets\Resources\Proto /e /y
)

xcopy protocols.lua ..\..\client\Main\Assets\Resources\Proto /e /y

echo "finished"

Pause