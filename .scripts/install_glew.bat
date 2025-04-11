@echo off
:: Download and install GLEW 2.1.0

set GLEW_URL=https://github.com/nigels-com/glew/releases/download/glew-2.1.0/glew-2.1.0.zip
set GLEW_ZIP=glew-2.1.0.zip
set INSTALL_DIR=glew-2.1.0

echo [1/6] Downloading GLEW 2.1.0...
powershell -Command "(New-Object Net.WebClient).DownloadFile('%GLEW_URL%', '%GLEW_ZIP%')"

echo [2/6] Unzip...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

powershell -Command "Expand-Archive -Path '%GLEW_ZIP%' -DestinationPath '%INSTALL_DIR%'"

echo [3/6] Clear...
if exist "%GLEW_ZIP%" del "%GLEW_ZIP%"

echo [4/6] Creating folders
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/install" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/install"
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/mybuild" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/mybuild"

echo [5/6] Configuration CMake
cmake -S "%INSTALL_DIR%/%INSTALL_DIR%/build/cmake" -B "%INSTALL_DIR%/%INSTALL_DIR%/mybuild" -G "Visual Studio 14 2015" -A Win32 -DBUILD_UTILS=OFF -DCMAKE_BUILD_TYPE=Release -DCMAKE_INSTALL_PREFIX="%INSTALL_DIR%/%INSTALL_DIR%/install" -DGLEW_OSMESA=OFF -DGLEW_REGAL=OFF

echo [6/6] Building
cmake --build "%INSTALL_DIR%/%INSTALL_DIR%/mybuild" --config Release --target install

echo Ready! Library in %INSTALL_DIR%/%INSTALL_DIR%/install