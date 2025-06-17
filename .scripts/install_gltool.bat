@echo off
:: install gltool

set INSTALL_DIR=../VisualStudio_2022/GLTool

echo [1/4] Creating folders
if not exist "%INSTALL_DIR%/install" mkdir "%INSTALL_DIR%/install"
if not exist "%INSTALL_DIR%/build" mkdir "%INSTALL_DIR%/build"

echo [2/4] Configuration CMake

cmake -S "%INSTALL_DIR%" -B "%INSTALL_DIR%/build" -G "Visual Studio 17 2022" -A x64 -DCMAKE_INSTALL_PREFIX="%INSTALL_DIR%/install" -DOUTPUT_DIR="%INSTALL_DIR%/install" -DCMAKE_BUILD_TYPE=Debug -DCMAKE_CONFIGURATION_TYPES=Debug -DGEOM_VIEW_CORE_DIR="%cd%/geomView-main/geomView-main/install" -DGLFW_DIR="%cd%/glfw-master/glfw-master/install/include"

echo [3/4] Building
cmake --build "%INSTALL_DIR%/build" --config Debug

echo [4/4] Move GLTool in Extension Folder
move "%INSTALL_DIR%\install\GLTool.dll" "..\VisualStudio_2022\GeometryDebugger\GeometryDebugger\GLTool.dll"
move "%INSTALL_DIR%\install\GLTool.pdb" "..\VisualStudio_2022\GeometryDebugger\GeometryDebugger\GLTool.pdb"

echo Ready!

pause