@echo off

rem Quick package creation script.

cd "%~dp0"
echo NuGet package creation script
echo =============================
echo This requires NuGet.exe in the .nuget directory.
echo If you haven't done so, enable package restore for the solution, and build for Release.
echo Press any key to create a NuPkg near this batch file.
pause >nul

rmdir /S /Q lib >nul
mkdir lib\net40
copy /B /Y ..\GraphX\bin\Release\GraphX.dll lib\net40\GraphX.dll

..\.nuget\NuGet.exe pack GraphX.temp.nuspec

rmdir /S /Q lib >nul

echo Press any key to exit.
pause >nul
