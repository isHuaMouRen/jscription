@echo off
chcp 65001 >nul

if exist "build" (
    rd /s /q "build"
)

:: Windows

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner.exe" "Jscription.Runner.win-x64.exe"

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner.exe" "Jscription.Runner.win-x86.exe"

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner.exe" "Jscription.Runner.win-arm64.exe"

:: Linux

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner" "Jscription.Runner.linux-x64"

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner" "Jscription.Runner.linux-arm64"

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r linux-arm --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner" "Jscription.Runner.linux-arm"

:: MacOS

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner" "Jscription.Runner.macos-x64"

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner" "Jscription.Runner.macos-arm64"



del "build\*.pdb"

pause