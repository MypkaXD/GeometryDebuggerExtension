#include <iostream>

#define _USE_MATH_DEFINES
#include <math.h>

#include "Point.h"
#include "Curve.h"
#include "Line.h"
#include "Circle.h"
#include "CustomCurve.h"
#include "Edge.h"
#include "Plane.h"
#include "Surface.h"
#include "CustomPlane.h"

Point getPointForCustomCurve(double param) {
	return Point(std::sin(param), std::cos(param), param);
}

Point getPointForParaboloid(double u, double v) {
	return Point(u, v, u * u + v * v);
}

int main() {

	Line line = Line(Point(0,0,0), Point(1,0,0));
	CustomCurve customCurve = CustomCurve(Point(0, 0, 0), getPointForCustomCurve);
	Circle circle = Circle(0.1, Point(0, 0, 0), Point(1, 0, 0));
	Edge edge1 = Edge(&line, 0, 1);
	Edge edge2 = Edge(&customCurve, 0, 10);
	Edge edge3 = Edge(&circle, 0, 2 * M_PI);

	Point point = Point(0, 0, 0);

	CustomPlane customPlane = CustomPlane(Point(1, 0, 0), Point(1, 0, 0), Point(0, 1, 0), getPointForParaboloid, -5, 5, -5, 5);
	Plane plane = Plane(Point(0, 0, 0), Point(1, 1, 0), Point(0, 1, 0), -5, 5, -5, 5);

	return 0;
}