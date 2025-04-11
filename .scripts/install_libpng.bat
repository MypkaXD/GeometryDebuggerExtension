@echo off
:: Download and build LIBPNG

set LIBPNG_URL=https://sourceforge.net/projects/libpng/files/libpng16/1.6.47/lpng1647.zip/download
set LIBPNG_ZIP=lpng1647.zip
set INSTALL_DIR=lpng1647

echo [1/6] Downloading LIBPNG...
powershell -Command "(New-Object Net.WebClient).DownloadFile('%LIBPNG_URL%', '%LIBPNG_ZIP%')"

echo [2/6] Extracting archive...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
powershell -Command "Expand-Archive -Path '%LIBPNG_ZIP%' -DestinationPath '%INSTALL_DIR%'"

echo [3/6] Cleaning up...
del "%LIBPNG_ZIP%"
echo LIBPNG successfully downloaded and extracted to %INSTALL_DIR%

echo [4/6] Creating folders...
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/myinstall" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/myinstall"
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/build" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/build"

echo [5/6] Configuration CMake

cmake -S "lpng1647/lpng1647" -B "lpng1647/lpng1647/build" -G "Visual Studio 14 2015" -A Win32 -DCMAKE_CONFIGURATION_TYPES="Release" -DCMAKE_INSTALL_PREFIX="lpng1647/lpng1647/myinstall" -DPNG_BUILD_ZLIB=OFF -DPNG_DEBUG=OFF -DPNG_EXECUTABLES=ON -DPNG_HARDWARE_OPTIMIZATIONS=OFF -DPNG_SHARED=OFF -DPNG_STATIC=ON -DPNG_TESTS=OFF -DZLIB_ROOT="zlib-1.3.1/zlib-1.3.1/install" -DPNG_TOOLS=ON -DCMAKE_BUILD_TYPE=Release

echo [6/6] Building
cmake --build "%INSTALL_DIR%/%INSTALL_DIR%/build" --config Release --target install

echo Ready! Library in %INSTALL_DIR%/%INSTALL_DIR%/myinstall