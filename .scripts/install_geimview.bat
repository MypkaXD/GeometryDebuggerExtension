@echo off
:: Download and build GeomView

echo [1/7] Build dependency libraries...
call install_glew.bat
call install_glfw.bat
call install_zlib.bat
call install_libpng.bat

set GEOMVIEW_URL=https://github.com/dafadey/geomView/archive/refs/heads/main.zip
set GEOMVIEW_ZIP=geomView-main.zip
set INSTALL_DIR=geomView-main

echo [2/7] Downloading GEOMVIEW...
powershell -Command "(New-Object Net.WebClient).DownloadFile('%GEOMVIEW_URL%', '%GEOMVIEW_ZIP%')"

echo [3/7] Extracting archive...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
powershell -Command "Expand-Archive -Path '%GEOMVIEW_ZIP%' -DestinationPath '%INSTALL_DIR%'"

echo [4/7] Cleaning up...
del "%GEOMVIEW_ZIP%"
echo GEOMVIEW successfully downloaded and extracted to %INSTALL_DIR%

echo [5/7] Creating folders...
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/install" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/install"
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/build" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/build"

echo [6/7] Configuration CMake
cmake -S "%INSTALL_DIR%/%INSTALL_DIR%" -B "%INSTALL_DIR%/%INSTALL_DIR%/build" -G "Visual Studio 14 2015" -A Win32 -DCMAKE_CONFIGURATION_TYPES="Release" -DCMAKE_INSTALL_PREFIX="%cd%/%INSTALL_DIR%/%INSTALL_DIR%/install" -DGLEW_DIR="%cd%/glew-2.1.0/glew-2.1.0/install/lib/cmake/glew" -DPNG_INCLUDE_DIR="%cd%/lpng1647/lpng1647/myinstall/include" -DPNG_LIBRARY="%cd%/lpng1647/lpng1647/myinstall/lib/libpng16_static.lib" -DZLIB_LIBRARY="%cd%/zlib-1.3.1/zlib-1.3.1/install/lib/zlibstatic.lib" -Dglfw3_DIR="%cd%/glfw-master/glfw-master/install/lib/cmake/glfw3" -DCMAKE_BUILD_TYPE=Release

echo [7/7] Building
cmake --build "%cd%/%INSTALL_DIR%/%INSTALL_DIR%/build" --config Release --target install

echo Ready! Library in %cd%/%INSTALL_DIR%/%INSTALL_DIR%/install