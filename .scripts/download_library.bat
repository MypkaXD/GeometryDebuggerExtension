@echo off

setlocal

set count_of_steps=3

set _DOWNLOAD_URL=%1
set _ZIP_FILE=%2
set _INSTALL_DIR=%3

echo [1/%count_of_steps%] Downloading library from %_DOWNLOAD_URL%
powershell -Command "(New-Object Net.WebClient).DownloadFile('%_DOWNLOAD_URL%', '%_ZIP_FILE%')"

echo [2/%count_of_steps%] Extracting archive %_ZIP_FILE% in %_INSTALL_DIR%
if not exist "%_INSTALL_DIR%" mkdir "%_INSTALL_DIR%"
powershell -Command "Expand-Archive -Path '%_ZIP_FILE%' -DestinationPath '%_INSTALL_DIR%'"

echo [4/%count_of_steps%] Cleaning up...
del "%_ZIP_FILE%"
echo Successfully downloaded and extracted to %_INSTALL_DIR%