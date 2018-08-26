color 0A && echo off

set "PROTOC_EXE=protoc.exe"

%PROTOC_EXE% --version

set "WORK_DIR=%cd%"

rem 找到输出路径
cd ../Socket/Protocol/
set "CS_OUT_PATH=%cd%"
cd %WORK_DIR%

echo ===================================
echo CompileProto
echo ===================================

for /f "delims=" %%i in ('dir /b "*.proto"') do (
	echo.generate %%i
	"%PROTOC_EXE%" --proto_path="%WORK_DIR%" --csharp_out="%CS_OUT_PATH%" %%i
)

echo ===================================
echo MakeProtoCode
echo ===================================

python MakeProtoCode.py

pause