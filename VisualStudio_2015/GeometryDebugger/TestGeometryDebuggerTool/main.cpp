#include <iostream>
#include <vector>

#include "Circle.h"
#include "Vector.h"
#include "Line.h"
#include "Point.h"
#include "Coordinate_system.h"
#include "Edge.h"

Point getPointForCustomCurve(double param) {
	return Point(std::sin(param), std::cos(param), param);
}

int main() {

	CoordinateSystem cs = CoordinateSystem();

	Line line = Line(Point(1, 1, 1), Vector(Point(0, 0, 1)));

	Point vector = Point(1, 0, 0);
	Point vector2 = Point(0, 0, 0);

	Edge edge1 = CreateStraightEdge(&line, 0, 1);
	Circle circle = Circle(1, Point(1, 1, 1), Point(0, 0, 1));
	Edge edge2 = CreateArcEdge(&circle, 0, 3 * M_PI / 2);

	CustomCurve customCurve = CustomCurve(
		Point(0, 0, 0),
		Vector(Point(0, 0, 1)),
		getPointForCustomCurve
	);

	Edge edge3 = CreateCustomEdge(&customCurve, 0, M_PI * 2);

	std::cout << std::endl;

	int a = 5;
	Point local = Point(5, 0, 0);

	Circle circle2 = Circle(15, Point(0, 0, 0), Point(0, 1, 1));
	Circle circle3 = Circle(5, Point(0, 0, 0), Point(0, 0, 1));
	Circle circle4 = Circle(5, Point(0, 0, 0), Point(0, 0, 1));

	std::vector<Circle> circles(3);
	for (int i = 0; i < circles.size(); ++i) {
		circles[i] = Circle((i + 1) * 5, Point(0, 0, 0), Point(0, 0, 1));
	}

	double step = 1;

	while (a < 10) {
		circle.setRadius(circle.getRadius() + step);
		circle2.setRadius(circle2.getRadius() + step);
		circle3.setRadius(circle3.getRadius() + step);
		step += 0.5;
		++a;
	}

	std::cout << "HELLO " << std::endl;

	return 0;

}