#include "Vector.h"
#include "Point.h"

#pragma once

struct CoordinateSystem {

	Point center = Point(0, 0, 0);
	Vector OrtZ = Vector(Point(0, 0, 1));
	Vector OrtX = Vector(Point(1, 0, 0));
	Vector OrtY = Vector(Point(0, 1, 0));

};