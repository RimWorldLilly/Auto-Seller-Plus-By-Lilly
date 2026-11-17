@echo off
setlocal

set "MOD_DIR=%~dp0"
set "PROJECT=%~dp0Source\AutoSellerPlus.csproj"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo ERROR: .NET SDK was not found. Install the .NET SDK and try again.
    @REM  pause
    exit /b 1
)

dotnet build "%PROJECT%" --configuration Release
set "BUILD_EXIT_CODE=%ERRORLEVEL%"

if not "%BUILD_EXIT_CODE%"=="0" (
    echo.
    echo Build failed.
) else (
    echo.
    echo Build succeeded. DLL: "%MOD_DIR%1.7\Assemblies\AutoSellerPlusByLilly.dll"
)

@REM  pause
exit /b %BUILD_EXIT_CODE%
