#include <string>

#include "Point.h"
#include "Vector.h"
#include "Curve.h"
#include "Line.h"
#include "Edge.h"
#include "Plane.h"
#include "Surface.h"
#include "CustomPlane.h"
#include "Sphere.h"
#include "Cylinder.h"

#ifndef SERIALIZEDMESSAGES_H
#define SERIALIZEDMESSAGES_H


std::string serialize(Point* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "points: " + variableName + '\n' + "(" + std::to_string(value->getX()) + "," + std::to_string(value->getY()) + "," + std::to_string(value->getZ()) + ")";

	return data + "\n";
}
std::string serialize(Vector* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "lines: " + variableName + ".lines\n";

	data += "(" + std::to_string(value->getVector().first.getX()) + "," +
		std::to_string(value->getVector().first.getY()) + "," +
		std::to_string(value->getVector().first.getZ()) + ")";
	data += "(" + std::to_string(value->getVector().second.getX()) + "," +
		std::to_string(value->getVector().second.getY()) + "," +
		std::to_string(value->getVector().second.getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	return data + "\n";
}

std::string serialize(Edge* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	std::pair<double, double> params = value->getParams();
	std::vector<Point> points;

	int count = 50;
	double step = (params.second - params.first) / count;

	for (int i = 0; i < count; ++i) {

		Point point = value->getPoint(params.first + step * i);
		points.push_back(point);

		data += serialize(&point, variableName + std::to_string(i), r, g, b);
	}

	for (int i = 0; i < points.size() - 1; ++i) {
		Vector vector = Vector(points[i], points[i + 1]);
		data += serialize(&vector, "vector." + std::to_string(i), r, g, b);
	}

	return data + "\n";
}

std::string serialize(CustomPlane* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	int countU = 20;
	int countV = 20;

	std::vector<std::vector<Point>> points(countU, std::vector<Point>(countV));

	double stepU = (value->getUMax() - value->getUMin()) / (countU - 1);
	double stepV = (value->getVMax() - value->getVMin()) / (countV - 1);

	//data += "points: " + variableName + '\n';

	for (int i = 0; i < countU; ++i) {
		for (int j = 0; j < countV; ++j) {
			Point currentPoint = value->getPoint(value->getUMin() + i * stepU, value->getVMin() + j * stepV);
			points[i][j] = currentPoint;
			//data += "(" + std::to_string(currentPoint.getX()) + "," + std::to_string(currentPoint.getY()) + "," + std::to_string(currentPoint.getZ()) + ")\n";
		}
	}

	//data += "lines: " + variableName + ".lines\n";
	data += "triangles: " + variableName + "\n";

	/*for (int i = 0; i < points.size(); ++i) {
		for (int j = 0; j < points[i].size(); ++j) {

			if (i + 1 <= points.size() - 1) {
				Vector vector1 = Vector(points[i][j], points[i + 1][j]);
				data += "(" + std::to_string(vector1.getVector().first.getX()) + "," +
					std::to_string(vector1.getVector().first.getY()) + "," +
					std::to_string(vector1.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector1.getVector().second.getX()) + "," +
					std::to_string(vector1.getVector().second.getY()) + "," +
					std::to_string(vector1.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j + 1 <= points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i][j + 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j != 0 && i != points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i + 1][j - 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}
		}
	}*/

	for (int i = 0; i < points.size() - 1; ++i) {
		for (int j = 0; j < points[i].size() - 1; ++j) {

			Point leftDown = points[i][j];
			Point leftUp = points[i + 1][j];
			Point rightDown = points[i][j + 1];
			Point rightUp = points[i + 1][j + 1];

			data += "(" + std::to_string(leftDown.getX()) + "," + 
				std::to_string(leftDown.getY()) + "," + std::to_string(leftDown.getZ()) + ")";
			data += "(" + std::to_string(leftUp.getX()) + "," +
				std::to_string(leftUp.getY()) + "," + std::to_string(leftUp.getZ()) + ")";
			data += "(" + std::to_string(rightDown.getX()) + "," +
				std::to_string(rightDown.getY()) + "," + std::to_string(rightDown.getZ()) + ")";
			data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

			data += "(" + std::to_string(leftUp.getX()) + "," +
				std::to_string(leftUp.getY()) + "," + std::to_string(leftUp.getZ()) + ")";
			data += "(" + std::to_string(rightUp.getX()) + "," +
				std::to_string(rightUp.getY()) + "," + std::to_string(rightUp.getZ()) + ")";
			data += "(" + std::to_string(rightDown.getX()) + "," +
				std::to_string(rightDown.getY()) + "," + std::to_string(rightDown.getZ()) + ")";
			data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
		}
	}

	return data + "\n";
}

std::string serialize(Plane* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	int countU = 10;
	int countV = 10;

	std::vector<std::vector<Point>> points(countU, std::vector<Point>(countV));

	double stepU = (value->getUMax() - value->getUMin()) / (countU - 1);
	double stepV = (value->getVMax() - value->getVMin()) / (countV - 1);

	std::cout << "SDASD" << std::endl;

	data += "points: " + variableName + '\n';

	for (int i = 0; i < countU; ++i) {
		for (int j = 0; j < countV; ++j) {
			Point currentPoint = value->getPoint(value->getUMin() + i * stepU, value->getVMin() + j * stepV);
			points[i][j] = currentPoint;
			data += "(" + std::to_string(currentPoint.getX()) + "," + std::to_string(currentPoint.getY()) + "," + std::to_string(currentPoint.getZ()) + ")\n";
			std::cout << "SDASD" << std::endl;
		}
	}

	data += "lines: " + variableName + ".lines\n";

	std::cout << "SDASD" << std::endl;

	for (int i = 0; i < points.size(); ++i) {
		for (int j = 0; j < points[i].size(); ++j) {

			if (i + 1 <= points.size() - 1) {
				Vector vector1 = Vector(points[i][j], points[i + 1][j]);
				data += "(" + std::to_string(vector1.getVector().first.getX()) + "," +
					std::to_string(vector1.getVector().first.getY()) + "," +
					std::to_string(vector1.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector1.getVector().second.getX()) + "," +
					std::to_string(vector1.getVector().second.getY()) + "," +
					std::to_string(vector1.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j + 1 <= points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i][j + 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j != 0 && i != points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i + 1][j - 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}
		}
	}

	return data + "\n";
}

std::string serialize(Sphere* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	int countU = 20;
	int countV = 20;

	std::vector<std::vector<Point>> points(countU, std::vector<Point>(countV));

	double stepU = (value->getUMax() - value->getUMin()) / (countU - 1);
	double stepV = (value->getVMax() - value->getVMin()) / (countV - 1);

	std::cout << "SDASD" << std::endl;

	data += "points: " + variableName + '\n';

	for (int i = 0; i < countU; ++i) {
		for (int j = 0; j < countV; ++j) {
			Point currentPoint = value->getPoint(value->getUMin() + i * stepU, value->getVMin() + j * stepV);
			points[i][j] = currentPoint;
			data += "(" + std::to_string(currentPoint.getX()) + "," + std::to_string(currentPoint.getY()) + "," + std::to_string(currentPoint.getZ()) + ")\n";
			std::cout << "SDASD" << std::endl;
		}
	}

	data += "lines: " + variableName + ".lines\n";

	std::cout << "SDASD" << std::endl;

	for (int i = 0; i < points.size(); ++i) {
		for (int j = 0; j < points[i].size(); ++j) {

			if (i + 1 <= points.size() - 1) {
				Vector vector1 = Vector(points[i][j], points[i + 1][j]);
				data += "(" + std::to_string(vector1.getVector().first.getX()) + "," +
					std::to_string(vector1.getVector().first.getY()) + "," +
					std::to_string(vector1.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector1.getVector().second.getX()) + "," +
					std::to_string(vector1.getVector().second.getY()) + "," +
					std::to_string(vector1.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j + 1 <= points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i][j + 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j != 0 && i != points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i + 1][j - 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}
		}
	}

	return data + "\n";
}

std::string serialize(Cylinder* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	int countU = 20;
	int countV = 20;

	std::vector<std::vector<Point>> points(countU, std::vector<Point>(countV));

	double stepU = (value->getUMax() - value->getUMin()) / (countU - 1);
	double stepV = (value->getVMax() - value->getVMin()) / (countV - 1);

	std::cout << "SDASD" << std::endl;

	data += "points: " + variableName + '\n';

	for (int i = 0; i < countU; ++i) {
		for (int j = 0; j < countV; ++j) {
			Point currentPoint = value->getPoint(value->getUMin() + i * stepU, value->getVMin() + j * stepV);
			points[i][j] = currentPoint;
			data += "(" + std::to_string(currentPoint.getX()) + "," + std::to_string(currentPoint.getY()) + "," + std::to_string(currentPoint.getZ()) + ")\n";
			std::cout << "SDASD" << std::endl;
		}
	}

	data += "lines: " + variableName + ".lines\n";

	std::cout << "SDASD" << std::endl;

	for (int i = 0; i < points.size(); ++i) {
		for (int j = 0; j < points[i].size(); ++j) {

			if (i + 1 <= points.size() - 1) {
				Vector vector1 = Vector(points[i][j], points[i + 1][j]);
				data += "(" + std::to_string(vector1.getVector().first.getX()) + "," +
					std::to_string(vector1.getVector().first.getY()) + "," +
					std::to_string(vector1.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector1.getVector().second.getX()) + "," +
					std::to_string(vector1.getVector().second.getY()) + "," +
					std::to_string(vector1.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j + 1 <= points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i][j + 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}

			if (j != 0 && i != points[i].size() - 1) {
				Vector vector2 = Vector(points[i][j], points[i + 1][j - 1]);
				data += "(" + std::to_string(vector2.getVector().first.getX()) + "," +
					std::to_string(vector2.getVector().first.getY()) + "," +
					std::to_string(vector2.getVector().first.getZ()) + ")";
				data += "(" + std::to_string(vector2.getVector().second.getX()) + "," +
					std::to_string(vector2.getVector().second.getY()) + "," +
					std::to_string(vector2.getVector().second.getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
			}
		}
	}

	return data + "\n";
}

#endif // !SERIALIZEDMESSAGES_H

