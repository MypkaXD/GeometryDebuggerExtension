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
#include "Vector.h"
#include "Sphere.h"
#include "Cylinder.h"

Point getPointForCustomCurve(double param) {
	return Point(std::sin(param), std::cos(param), param);
}

Point getPointForSphere(double u, double v) {
	return Point(std::cos(v)*std::cos(u), std::sin(u)*std::cos(v), std::sin(v));
}

Point getPointForSpiral(double u, double v) {
	return Point(std::cos(u)*(std::cos(v) + 3), std::sin(u)*(std::cos(v) + 3), std::sin(v) + u);
}

Point getPointForMebius(double u, double v) {
	return Point((1 + v / 2 * std::cos(u / 2))*std::cos(u), (1 + v / 2 * std::cos(u / 2))*std::sin(u) , v/2*std::sin(u/2));
}

Point getPointForDini(double u, double v) {
	return Point(std::cos(u)*std::sin(v), std::sin(u)*std::sin(v), std::cos(v)+std::log10(std::tan(v/2)) + 0.2*u-4);
}

int main() {

	Line line = Line(Point(0,0,0), Point(1,0,0));
	CustomCurve customCurve = CustomCurve(Point(0, 0, 0), getPointForCustomCurve);
	Circle circle = Circle(0.1, Point(0, 0, 0), Point(1, 0, 0));

	Edge edge1 = Edge(&line, 0, 1);
	Edge edge2 = Edge(&customCurve, 0, 10);
	Edge edge3 = Edge(&circle, 0, 2 * M_PI);

	Point origin = Point(0, 0, 0);
	Point x = Point(1, 0, 0);
	Point y = Point(0, 1, 0);
	Point z = Point(0, 0, 1);
	Vector ox = Vector(origin, x);
	Vector oy = Vector(origin, y);
	Vector oz = Vector(origin, z);

	CustomPlane customPlane = CustomPlane(Point(-10, 0, 0), Point(1, 0, 0), Point(0, 0, 1), getPointForMebius, 0, 2 * M_PI, -1, 1);
	CustomPlane customPlane1 = CustomPlane(Point(-10, 0, 0), Point(1, 0, 0), Point(0, 0, 1), getPointForSpiral, -2 * M_PI, 2 * M_PI, -M_PI, M_PI);
	CustomPlane customPlane2 = CustomPlane(Point(-10, 0, 0), Point(1, 0, 0), Point(0, 0, 1), getPointForDini, 0, 4 * M_PI, 0.001, 2);

	Plane plane = Plane(Point(0, 0, 0), Point(1, 0, 0), Point(0, 1, 0), -2 * M_PI, 2 * M_PI, -M_PI, M_PI);
	Sphere sphere = Sphere(Point(0, 10, 0), Point(1, 0, 0), Point(0, 1, 0), 1, -M_PI, M_PI, -M_PI / 2, M_PI / 2);
	Cylinder cylinder = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);

	Cylinder &cylinder2 = cylinder;

	return 0;
}