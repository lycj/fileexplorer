@echo off
echo "Building %1"

:NewRevision
choice /C YN /T 5 /D N /M "Increment Revision Number?" 
IF %ERRORLEVEL% EQU 1 goto Nant_Revision

:MajorVersion
choice /C YN /T 5 /D N /M "Increment Major Version?" 
IF %ERRORLEVEL% EQU 1 goto Nant_Patch

:GenerateDoc
choice /C YN /T 5 /D N /M "Generate Documentation?" 
IF %ERRORLEVEL% EQU 1 goto Doxygen

:MajorVersion
choice /C YN /T 5 /D N /M "Abort Build?" 
IF %ERRORLEVEL% EQU 2 goto Nant

echo "Aborting..."
pause
goto end

:Nant_Revision
..\tools\nant\nant.exe init import build-revision copy nuget pack -buildfile:%1 -logfile:log\%1.log
goto end

:Nant_Patch
..\tools\nant\nant.exe init import build-patch copy nuget pack -buildfile:%1 -logfile:log\%1.log
goto end

:Nant
..\tools\nant\nant.exe init import build copy nuget pack clean -buildfile:%1 -logfile:log\%1.log 
goto end

:Doxygen
..\tools\nant\nant.exe init doxygen -buildfile:%1 -logfile:log\%1.log 

:end
