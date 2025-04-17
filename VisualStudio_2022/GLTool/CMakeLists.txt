cmake_minimum_required(VERSION 3.13)
project(GLTool VERSION 1.0 LANGUAGES CXX)

set(GEOM_VIEW_CORE_DIR "" CACHE PATH "Path for geom view")
set(GLFW_DIR "" CACHE PATH "Path for glf3 include")
set(OUTPUT_DIR "" CACHE PATH "Path for output GLTool.dll")

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "$<0:>${OUTPUT_DIR}")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "$<0:>${OUTPUT_DIR}")
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "$<0:>${OUTPUT_DIR}")

find_package(OpenGL REQUIRED)

add_library(GLTool SHARED src/GLtool.cpp src/Wrapper.cpp)

target_include_directories(GLTool PUBLIC include)
target_include_directories(GLTool PUBLIC ${GEOM_VIEW_CORE_DIR}/include)
target_include_directories(GLTool PUBLIC ${GLFW_DIR})

target_link_libraries(GLTool PUBLIC ${GEOM_VIEW_CORE_DIR}/lib/libgeom_view.lib)
target_link_libraries(GLTool PUBLIC OpenGL::GL)