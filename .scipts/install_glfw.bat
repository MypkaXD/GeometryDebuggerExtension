@echo off
:: Download and install GLFW

set GLFW_URL=https://github.com/glfw/glfw/archive/refs/heads/master.zip
set GLFW_ZIP=glfw-master.zip
set INSTALL_DIR=glfw-master

echo [1/6] Downloading GLFW 3.3.8...
powershell -Command "(New-Object Net.WebClient).DownloadFile('%GLFW_URL%', '%GLFW_ZIP%')"

echo [2/6] Extracting archive...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
powershell -Command "Expand-Archive -Path '%GLFW_ZIP%' -DestinationPath '%INSTALL_DIR%'"

echo [3/6] Cleaning up...
del "%GLFW_ZIP%"

echo [4/6] Creating folders
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/install" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/install"
if not exist "%INSTALL_DIR%/%INSTALL_DIR%/build" mkdir "%INSTALL_DIR%/%INSTALL_DIR%/build"

echo [5/6] Configuration CMake

cmake -S "%INSTALL_DIR%/%INSTALL_DIR%" -B "%INSTALL_DIR%/%INSTALL_DIR%/build" -G "Visual Studio 14 2015" -A Win32 -DBUILD_SHARED_LIBS=OFF -DCMAKE_INSTALL_PREFIX="%INSTALL_DIR%/%INSTALL_DIR%/install" -DGLFW_BUILD_DOCS=OFF -DGLFW_BUILD_EXAMPLES=OFF -DGLFW_BUILD_TESTS=OFF -DGLFW_BUILD_WIN32=ON -DGLFW_INSTALL=ON -DGLFW_USE_HYBRID_HPG=OFF -USE_MSVC_RUNTIME_LIBRARY_DLL=OFF -DCMAKE_BUILD_TYPE=Release 

echo [6/6] Building
cmake --build "%INSTALL_DIR%/%INSTALL_DIR%/build" --config Release --target install

echo Ready! Library in %INSTALL_DIR%/%INSTALL_DIR%/install