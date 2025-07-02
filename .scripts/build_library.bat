@echo off

set LIBRARY_PATH=%~1
set CMAKELISTS_PATH=%~2
set ARCH=%~3
set MVS_VERSION=%~4
set ADDITIONAL_PARAMS=%~5
set CONFIGURATION=%~6

setlocal

set count_of_steps=3

echo [1/%count_of_steps%] Creating folders...
if not exist "%LIBRARY_PATH%/mybuild" mkdir "%LIBRARY_PATH%/mybuild"
if not exist "%LIBRARY_PATH%/myinstall" mkdir "%LIBRARY_PATH%/myinstall"
if not exist "%LIBRARY_PATH%/myinstall_files" mkdir "%LIBRARY_PATH%/myinstall_files"

echo %CMAKELISTS_PATH%
echo %LIBRARY_PATH%
echo %MVS_VERSION%
echo %ARCH%
echo %CONFIGURATION%
echo %ADDITIONAL_PARAMS%

echo [2/%count_of_steps%] Configuration CMake...
cmake -S "%CMAKELISTS_PATH%" -B "%LIBRARY_PATH%/mybuild" -G "%MVS_VERSION%" -A %ARCH% ^
-DCMAKE_INSTALL_PREFIX="%LIBRARY_PATH%/myinstall" -DCMAKE_CONFIGURATION_TYPES="%CONFIGURATION%" ^
%ADDITIONAL_PARAMS%

setlocal
set "config_list=%CONFIGURATION:;= %"

echo [3/%count_of_steps%] Building each configuration...

for %%C in (%config_list%) do (
    echo Building configuration: %%C
    
    cmake --build "%LIBRARY_PATH%\mybuild" --config %%C --target INSTALL
	
	if not exist "%LIBRARY_PATH%\myinstall_files\%%C" mkdir "%LIBRARY_PATH%\myinstall_files\%%C"

    move /Y "%LIBRARY_PATH%/myinstall" "%LIBRARY_PATH%/myinstall_files/%%C/"
)
