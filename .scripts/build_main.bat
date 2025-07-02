@echo off

set GLEW_URL=https://github.com/nigels-com/glew/releases/download/glew-2.1.0/glew-2.1.0.zip
set GLEW_ZIP=glew-2.1.0.zip
set GLEW_INSTALL_DIR=glew-2.1.0

set GLFW_URL=https://github.com/glfw/glfw/archive/refs/heads/master.zip
set GLFW_ZIP=glfw-master.zip
set GLFW_INSTALL_DIR=glfw-master

set ZLIB_URL=https://github.com/madler/zlib/releases/download/v1.3.1/zlib131.zip
set ZLIB_ZIP=zlib131.zip
set ZLIB_INSTALL_DIR=zlib-1.3.1

set LIBPNG_URL=https://github.com/pnggroup/libpng/archive/refs/tags/v1.6.47.zip
set LIBPNG_ZIP=libpng-1.6.47.zip
set LIBPNG_INSTALL_DIR=libpng-1.6.47

setlocal

set count_of_steps=3

echo [1/%count_of_steps%] Download dependency libraries...

call download_library.bat %GLEW_URL% %GLEW_ZIP% %GLEW_INSTALL_DIR%
call download_library.bat %GLFW_URL% %GLFW_ZIP% %GLFW_INSTALL_DIR%
call download_library.bat %ZLIB_URL% %ZLIB_ZIP% %ZLIB_INSTALL_DIR%
call download_library.bat %LIBPNG_URL% %LIBPNG_ZIP% %LIBPNG_INSTALL_DIR%

echo [2/%count_of_steps%] Build dependency libraries...

call build_library.bat "%GLEW_INSTALL_DIR%/%GLEW_INSTALL_DIR%" "%GLEW_INSTALL_DIR%/%GLEW_INSTALL_DIR%/build/cmake" "x64" "Visual Studio 17 2022" "-DBUILD_UTILS=OFF -DGLEW_OSMESA=OFF -DGLEW_REGAL=OFF" "Release"

call build_library.bat "%GLFW_INSTALL_DIR%/%GLFW_INSTALL_DIR%" "%GLFW_INSTALL_DIR%/%GLFW_INSTALL_DIR%" "x64" "Visual Studio 17 2022" "-DBUILD_SHARED_LIBS=OFF -DGLFW_BUILD_DOCS=OFF -DGLFW_BUILD_EXAMPLES=OFF -DGLFW_BUILD_TESTS=OFF -DGLFW_BUILD_WIN32=ON -DGLFW_INSTALL=ON -DGLFW_USE_HYBRID_HPG=OFF -USE_MSVC_RUNTIME_LIBRARY_DLL=OFF" "Release"

call build_library.bat "%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%" "%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%" "x64" "Visual Studio 17 2022" "-DINSTALL_BIN_DIR=%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%/myinstall_files/Release/myinstall/bin -DINSTALL_INC_DIR=%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%/myinstall_files/Release/myinstall/include -DINSTALL_LIB_DIR=%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%/myinstall_files/Release/myinstall/lib -DINSTALL_MAN_DIR=%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%/myinstall_files/Release/myinstall/share/man -DINSTALL_PKGCONFIG_DIR=%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%/myinstall_files/Release/myinstall/share/pkgconfig -DZLIB_BUILD_EXAMPLES=OFF" "Release"

call build_library.bat "%LIBPNG_INSTALL_DIR%/%LIBPNG_INSTALL_DIR%" "%LIBPNG_INSTALL_DIR%/%LIBPNG_INSTALL_DIR%" "x64" "Visual Studio 17 2022" "-DPNG_BUILD_ZLIB=OFF -DPNG_DEBUG=OFF -DPNG_EXECUTABLES=ON -DPNG_HARDWARE_OPTIMIZATIONS=OFF -DPNG_SHARED=OFF -DPNG_STATIC=ON -DPNG_TESTS=OFF -DZLIB_ROOT="%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%/myinstall_files/Release/myinstall" -DPNG_TOOLS=ON" "Release"

set GEOMVIEW_URL=https://github.com/dafadey/geomView/archive/refs/heads/main.zip
set GEOMVIEW_ZIP=geomView-main.zip
set GEOMVIEW_INSTALL_DIR=geomView-main

call download_library.bat %GEOMVIEW_URL% %GEOMVIEW_ZIP% %GEOMVIEW_INSTALL_DIR%

call build_library.bat "%GEOMVIEW_INSTALL_DIR%/%GEOMVIEW_INSTALL_DIR%" "%GEOMVIEW_INSTALL_DIR%/%GEOMVIEW_INSTALL_DIR%" "x64" "Visual Studio 17 2022" "-DGLEW_DIR=%cd%/%GLEW_INSTALL_DIR%/%GLEW_INSTALL_DIR%/myinstall_files/Release/myinstall/lib/cmake/glew -DPNG_INCLUDE_DIR=%cd%/%LIBPNG_INSTALL_DIR%/%LIBPNG_INSTALL_DIR%/myinstall_files/Release/myinstall/include -DPNG_LIBRARY=%cd%/%LIBPNG_INSTALL_DIR%/%LIBPNG_INSTALL_DIR%/myinstall_files/Release/myinstall/lib/libpng16_static.lib -DZLIB_LIBRARY=%cd%/%ZLIB_INSTALL_DIR%/%ZLIB_INSTALL_DIR%/myinstall_files/Release/myinstall/lib/zlibstatic.lib -Dglfw3_DIR=%cd%/%GLFW_INSTALL_DIR%/%GLFW_INSTALL_DIR%/myinstall_files/Release/myinstall/lib/cmake/glfw3 -DCMAKE_CXX_FLAGS=/D_DISABLE_CONSTEXPR_MUTEX_CONSTRUCTOR" "Release;RelWithDebInfo;Debug"

set GLTOOL_INSTALL_DIR=../VisualStudio_2022/GLTool

call install_gltool.bat "%GLTOOL_INSTALL_DIR%" "%GLTOOL_INSTALL_DIR%" "x64" "Visual Studio 17 2022" "-DGEOM_VIEW_CORE_DIR=%cd%/%GEOMVIEW_INSTALL_DIR%/%GEOMVIEW_INSTALL_DIR%/myinstall_files/Debug/myinstall -DGLFW_DIR=%cd%/%GLFW_INSTALL_DIR%/%GLFW_INSTALL_DIR%/myinstall_files/Release/myinstall/include" "Debug"

del %GLTOOL_INSTALL_DIR%/mybuild

call install_gltool.bat "%GLTOOL_INSTALL_DIR%" "%GLTOOL_INSTALL_DIR%" "x64" "Visual Studio 17 2022" "-DOUTPUT_DIR=%GLTOOL_INSTALL_DIR%/myinstall -DGEOM_VIEW_CORE_DIR=%cd%/%GEOMVIEW_INSTALL_DIR%/%GEOMVIEW_INSTALL_DIR%/myinstall_files/RelWithDebInfo/myinstall -DGLFW_DIR=%cd%/%GLFW_INSTALL_DIR%/%GLFW_INSTALL_DIR%/myinstall_files/Release/myinstall/include" "RelWithDebInfo"

del %GLTOOL_INSTALL_DIR%/mybuild

call install_gltool.bat "%GLTOOL_INSTALL_DIR%" "%GLTOOL_INSTALL_DIR%" "x64" "Visual Studio 17 2022" "-DGEOM_VIEW_CORE_DIR=%cd%/%GEOMVIEW_INSTALL_DIR%/%GEOMVIEW_INSTALL_DIR%/myinstall_files/Release/myinstall -DGLFW_DIR=%cd%/%GLFW_INSTALL_DIR%/%GLFW_INSTALL_DIR%/myinstall_files/Release/myinstall/include" "Release"

pause