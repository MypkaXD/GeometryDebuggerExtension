#include <iostream>
#include <memory>  

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
#include "Face.h"

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
	return Point((1 + v / 2 * std::cos(u / 2))*std::cos(u), (1 + v / 2 * std::cos(u / 2))*std::sin(u), v / 2 * std::sin(u / 2));
}

Point getPointForDini(double u, double v) {
	return Point(std::cos(u)*std::sin(v), std::sin(u)*std::sin(v), std::cos(v) + std::log10(std::tan(v / 2)) + 0.2*u - 4);
}


int main() {

	////setlocale(LC_ALL, "rus");

	Point origin = Point(0, 0, 0);
	Point x = Point(1, 0, 0);
	Point y = Point(0, 1, 0);
	Point z = Point(0, 0, 1);
	Vector ox = Vector(origin, x);
	Vector oy = Vector(origin, y);
	Vector oz = Vector(origin, z);

	Point outer1(0, 3, 0);
	Point outer2(2, 2, 0);
	Point outer3(3, 0, 0);
	Point outer4(2, -2, 0);
	Point outer5(0, -3, 0);
	Point outer6(-2, -2, 0);
	Point outer7(-3, 0, 0);
	Point outer8(-2, 2, 0);

	// Внутренние вершины звезды (углубления между лучами)
	Point inner1(0, 1, 0);
	Point inner2(1, 1, 0);
	Point inner3(1, 0, 0);
	Point inner4(1, -1, 0);
	Point inner5(0, -1, 0);
	Point inner6(-1, -1, 0);
	Point inner7(-1, 0, 0);
	Point inner8(-1, 1, 0);

	// Создаем линии для лучей звезды (используем shared_ptr)
	std::vector<Line> lines = {
		Line(outer1, Point(inner1.getX() - outer1.getX(), inner1.getY() - outer1.getY(), 0)),
		Line(inner1, Point(outer2.getX() - inner1.getX(), outer2.getY() - inner1.getY(), 0)),
		Line(outer2, Point(inner2.getX() - outer2.getX(), inner2.getY() - outer2.getY(), 0)),
		Line(inner2, Point(outer3.getX() - inner2.getX(), outer3.getY() - inner2.getY(), 0)),
		Line(outer3, Point(inner3.getX() - outer3.getX(), inner3.getY() - outer3.getY(), 0)),
		Line(inner3, Point(outer4.getX() - inner3.getX(), outer4.getY() - inner3.getY(), 0)),
		Line(outer4, Point(inner4.getX() - outer4.getX(), inner4.getY() - outer4.getY(), 0)),
		Line(inner4, Point(outer5.getX() - inner4.getX(), outer5.getY() - inner4.getY(), 0)),
		Line(outer5, Point(inner5.getX() - outer5.getX(), inner5.getY() - outer5.getY(), 0)),
		Line(inner5, Point(outer6.getX() - inner5.getX(), outer6.getY() - inner5.getY(), 0)),
		Line(outer6, Point(inner6.getX() - outer6.getX(), inner6.getY() - outer6.getY(), 0)),
		Line(inner6, Point(outer7.getX() - inner6.getX(), outer7.getY() - inner6.getY(), 0)),
		Line(outer7, Point(inner7.getX() - outer7.getX(), inner7.getY() - outer7.getY(), 0)),
		Line(inner7, Point(outer8.getX() - inner7.getX(), outer8.getY() - inner7.getY(), 0)),
		Line(outer8, Point(inner8.getX() - outer8.getX(), inner8.getY() - outer8.getY(), 0)),
		Line(inner8, Point(outer1.getX() - inner8.getX(), outer1.getY() - inner8.getY(), 0))
	};

	//// Добавляем дополнительные линии, чтобы получить 50 рёбер
	//for (int i = 0; i < 34; i++) {
	//	// Создаем простые диагональные линии внутри звезды
	//	Point p1(i * 0.1, i * 0.1, 0);
	//	Point p2(i * 0.1 + 0.5, i * 0.1 - 0.5, 0);
	//	lines.push_back(Line(p1, Point(p2.getX() - p1.getX(), p2.getY() - p1.getY(), 0)));
	//}

	// Создаем ребра на основе линий
	std::vector<Edge> edges;
	for (size_t i = 0; i < lines.size(); i++) {
		edges.push_back(Edge(&lines[i], (double)0, (double)1));
	}

	Sphere sphere = Sphere(Point(0, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, -M_PI, M_PI, -M_PI / 2, M_PI / 2);
	Plane plane = Plane(Point(0, 0, 0), Point(1, 0, 0), Point(0, 1, 0), -3, 3, -3, 3);
	Face face = Face(&plane, edges);

	double u = 0.5, v = 0.5;

	std::cout << face.IsInside(u, v) << std::endl;

	Cylinder cylinder1 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);
	Cylinder cylinder2 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);
	Cylinder cylinder3 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);

	std::vector<int> vec;
	vec[3] = 2;

	return 0;

}