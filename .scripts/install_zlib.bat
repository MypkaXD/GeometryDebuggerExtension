@echo off
:: Download and build ZLIB

set ZLIB_URL=https://github.com/madler/zlib/releases/download/v1.3.1/zlib131.zip
set ZLIB_ZIP=zlib131.zip
set INSTALL_DIR=zlib-1.3.1

echo [1/6] Downloading ZLIB...
powershell -Command "(New-Object Net.WebClient).DownloadFile('%ZLIB_URL%', '%ZLIB_ZIP%')"

echo [2/6] Extracting archive...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
powershell -Command "Expand-Archive -Path '%ZLIB_ZIP%' -DestinationPath '%INSTALL_DIR%'"

echo [3/6] Cleaning up...
del "%ZLIB_ZIP%"
echo ZLIB successfully downloaded and extracted to %INSTALL_DIR%

echo [4/6] Creating folders...
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/install" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/install"
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/build" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/build"

echo [5/6] Configuration CMake
cmake -S "%INSTALL_DIR%/%INSTALL_DIR%" -B "%INSTALL_DIR%/%INSTALL_DIR%/build" -G "Visual Studio 17 2022" -A x64 -DCMAKE_CONFIGURATION_TYPES="Release" -DCMAKE_INSTALL_PREFIX="%INSTALL_DIR%/%INSTALL_DIR%/install" -DINSTALL_BIN_DIR="%INSTALL_DIR%/%INSTALL_DIR%/install/bin" -DINSTALL_INC_DIR="%INSTALL_DIR%/%INSTALL_DIR%/install/include" -DINSTALL_LIB_DIR="%INSTALL_DIR%/%INSTALL_DIR%/install/lib" -DINSTALL_MAN_DIR="%INSTALL_DIR%/%INSTALL_DIR%/install/share/man" -DINSTALL_PKGCONFIG_DIR="%INSTALL_DIR%/%INSTALL_DIR%/install/share/pkgconfig" -DZLIB_BUILD_EXAMPLES=OFF -DCMAKE_BUILD_TYPE=Release 

echo [6/6] Building
cmake --build "%INSTALL_DIR%/%INSTALL_DIR%/build" --config Release --target install

echo Ready! Library in %INSTALL_DIR%/%INSTALL_DIR%/install