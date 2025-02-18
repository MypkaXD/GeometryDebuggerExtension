#include <iostream>
#include <vector>

#include "Circle.h"
#include "Vector.h"
#include "Line.h"
#include "Point.h"
#include "Coordinate_system.h"
#include "Edge.h"
#include "Net.h"

Point getPointForCustomCurve(double param) {
	return Point(std::sin(param), std::cos(param), param);
}

int main() {

	CoordinateSystem const cs = CoordinateSystem();

	return 0;

}