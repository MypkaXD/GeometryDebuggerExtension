cmake_minimum_required(VERSION 3.8)
project(GEOM_ENGINE)

file(GLOB SOURCES "${CMAKE_CURRENT_SOURCE_DIR}/src/*.cpp")

file(GLOB HEADERS "${CMAKE_CURRENT_SOURCE_DIR}/include/*.h")

add_executable(${PROJECT_NAME} 
    ${SOURCES}
    ${HEADERS}
)

set(GEOM_VIEW_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../../.scripts/geomView-main/geomView-main/myinstall_files/Debug/myinstall")

target_link_libraries(${PROJECT_NAME} PUBLIC 
    "${GEOM_VIEW_DIR}/lib/libgeom_view.lib"
)

target_include_directories(${PROJECT_NAME} PUBLIC
    "${CMAKE_CURRENT_SOURCE_DIR}/include"
    "${GEOM_VIEW_DIR}/include"
)

find_package(OpenGL REQUIRED)
target_link_libraries(${PROJECT_NAME} PUBLIC OpenGL::GL)
