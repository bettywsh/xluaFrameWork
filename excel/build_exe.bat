PyInstaller -F -c  -i  Dragon.ico ExcelTools.py
move dist\ExcelTools.exe  .\ExcelTools.exe
rd /s/q dist
rd /s/q __pycache__
rd /s/q build
del ExcelTools.spec