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

	// Внешние вершины звезды (кончики лучей)
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

	// Создаем линии для лучей звезды
	Line line1(outer1, inner1 - outer1); // От outer1 к inner1
	Line line2(inner1, outer2 - inner1); // От inner1 к outer2
	Line line3(outer2, inner2 - outer2); // От outer2 к inner2
	Line line4(inner2, outer3 - inner2); // От inner2 к outer3
	Line line5(outer3, inner3 - outer3); // От outer3 к inner3
	Line line6(inner3, outer4 - inner3); // От inner3 к outer4
	Line line7(outer4, inner4 - outer4); // От outer4 к inner4
	Line line8(inner4, outer5 - inner4); // От inner4 к outer5
	Line line9(outer5, inner5 - outer5); // От outer5 к inner5
	Line line10(inner5, outer6 - inner5); // От inner5 к outer6
	Line line11(outer6, inner6 - outer6); // От outer6 к inner6
	Line line12(inner6, outer7 - inner6); // От inner6 к outer7
	Line line13(outer7, inner7 - outer7); // От outer7 к inner7
	Line line14(inner7, outer8 - inner7); // От inner7 к outer8
	Line line15(outer8, inner8 - outer8); // От outer8 к inner8
	Line line16(inner8, outer1 - inner8); // От inner8 к outer1

										  // Создаем ребра на основе линий
	Edge edge1(&line1, 0, 1);
	Edge edge2(&line2, 0, 1);
	Edge edge3(&line3, 0, 1);
	Edge edge4(&line4, 0, 1);
	Edge edge5(&line5, 0, 1);
	Edge edge6(&line6, 0, 1);
	Edge edge7(&line7, 0, 1);
	Edge edge8(&line8, 0, 1);
	Edge edge9(&line9, 0, 1);
	Edge edge10(&line10, 0, 1);
	Edge edge11(&line11, 0, 1);
	Edge edge12(&line12, 0, 1);
	Edge edge13(&line13, 0, 1);
	Edge edge14(&line14, 0, 1);
	Edge edge15(&line15, 0, 1);
	Edge edge16(&line16, 0, 1);

	// Собираем все ребра в вектор
	std::vector<Edge> edges = {
		edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8,
		edge9, edge10, edge11, edge12, edge13, edge14, edge15, edge16
	};

	// Теперь у вас есть звезда, состоящая из 16 ребер
	std::cout << "Звезда создана!" << std::endl;


	/*Line line1 = Line(Point(0, 0, 0), Point(0, 1, 0));
	Line line2 = Line(Point(0, 0, 0), Point(1, 0, 0));
	Line line3 = Line(Point(0, 1, 0), Point(1, 0, 0));
	Line line4 = Line(Point(1, 1, 0), Point(0, -1, 0));

	Edge edge1(&line1, 0, 1);
	Edge edge2(&line2, 0, 1);
	Edge edge3(&line3, 0, 1);
	Edge edge4(&line4, 0, 1);

	std::vector<Edge> edges = {
		edge1, edge2, edge3, edge4 };*/

	Sphere sphere = Sphere(Point(0, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, -M_PI, M_PI, -M_PI / 2, M_PI / 2);
	Plane plane = Plane(Point(0, 0, 0), Point(1, 0, 0), Point(0, 1, 0), -3, 3, -3, 3);
	Face face = Face(&sphere, edges);

	double u = 0.5, v = 0.5;

	std::cout << face.IsInside(u, v) << std::endl;

	Cylinder cylinder1 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);
	Cylinder cylinder2 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);
	Cylinder cylinder3 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);


	return 0;

}