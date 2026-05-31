@echo off
chcp 65001 >nul

if exist "build" (
    rd /s /q "build"
)

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner.exe" "Jscription.Runner.win-x64.exe"

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner.exe" "Jscription.Runner.win-x86.exe"

dotnet publish "Jscription.Runner\Jscription.Runner.csproj" -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -p:TrimMode=copyused -o "build"
ren "build\Jscription.Runner.exe" "Jscription.Runner.win-arm64.exe"

del "build\*.pdb"


pause