#include <string>

#include "Vector.h"
#include "Point.h"
#include "Circle.h"
#include "Line.h"
#include "Coordinate_system.h"
#include "Edge.h"
#include "Net.h"

#pragma once

std::string serialize(Point* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "points: " + variableName + '\n' + "(" + std::to_string(value->getX()) + "," + std::to_string(value->getY()) + "," + std::to_string(value->getZ()) + ")";

	return data + "\n";
}
std::string serialize(Circle* value, std::string variableName, float r, float g, float b) {

	std::string data = "";
	int countOfCircle = 50;

	std::vector<std::tuple<double, double, double>> coords;

	for (int i = 0; i < countOfCircle; ++i) {
		double phi = 2 * M_PI * i / countOfCircle;
		Point coord = value->getPoint(phi);
		coords.push_back(std::make_tuple(coord.getX(), coord.getY(), coord.getZ()));
	}

	// Формирование строк для записи сегментов круга
	data += "points: " + variableName + ".center\n";
	data += "(" + std::to_string(value->getCenter().getX()) + "," +
		std::to_string(value->getCenter().getY()) + "," +
		std::to_string(value->getCenter().getZ()) + ")\n";

	data += "lines: " + variableName + ".lines\n";

	for (int i = 0; i < countOfCircle - 1; ++i) {
		data += "(" + std::to_string(std::get<0>(coords[i])) + "," +
			std::to_string(std::get<1>(coords[i])) + "," +
			std::to_string(std::get<2>(coords[i])) + ")";
		data += "(" + std::to_string(std::get<0>(coords[i + 1])) + "," +
			std::to_string(std::get<1>(coords[i + 1])) + "," +
			std::to_string(std::get<2>(coords[i + 1])) + ")";
		data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
	}

	// Замыкаем круг, соединяя последнюю и первую точки
	data += "(" + std::to_string(std::get<0>(coords[countOfCircle - 1])) + "," +
		std::to_string(std::get<1>(coords[countOfCircle - 1])) + "," +
		std::to_string(std::get<2>(coords[countOfCircle - 1])) + ")";
	data += "(" + std::to_string(std::get<0>(coords[0])) + "," +
		std::to_string(std::get<1>(coords[0])) + "," +
		std::to_string(std::get<2>(coords[0])) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	return data + "\n";

}
std::string serialize(std::vector<Circle>* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	for (int i = 0; i < value->size(); ++i) {
		data += serialize(&(*value)[i], variableName + std::to_string(i), r, g, b);
	}

	return data + "\n";

}
std::string serialize(Line* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	std::vector<std::tuple<double, double, double>> coords;

	int param = 10;

	for (int i = 0; i < param; ++i) {
		Point coord = value->getPoint(i);
		data += serialize(&coord, variableName + std::to_string(i), r, g, b);
		coords.push_back(std::make_tuple(coord.getX(), coord.getY(), coord.getZ()));
	}

	data += "lines: " + variableName + ".lines\n";

	for (int i = 0; i < param - 1; ++i) {
		data += "(" + std::to_string(std::get<0>(coords[i])) + "," +
			std::to_string(std::get<1>(coords[i])) + "," +
			std::to_string(std::get<2>(coords[i])) + ")";
		data += "(" + std::to_string(std::get<0>(coords[i + 1])) + "," +
			std::to_string(std::get<1>(coords[i + 1])) + "," +
			std::to_string(std::get<2>(coords[i + 1])) + ")";
		data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
	}

	return data + "\n";
}

std::string serialize(Vector* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	Point startCoord = Point(0, 0, 0);

	data += serialize(&startCoord, variableName, r, g, b);

	data += "lines: " + variableName + ".lines\n";

	data += "(" + std::to_string(startCoord.getX()) + "," +
		std::to_string(startCoord.getY()) + "," +
		std::to_string(startCoord.getZ()) + ")";
	data += "(" + std::to_string(value->point.getX()) + "," +
		std::to_string(value->point.getY()) + "," +
		std::to_string(value->point.getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	return data + "\n";
}

std::string serialize(CoordinateSystem* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += serialize(&value->center, variableName, r, g, b);

	data += "lines: " + variableName + ".lines\n";

	data += serialize(&value->OrtX, variableName + "X", 255, 0, 0);
	data += serialize(&value->OrtY, variableName + "Y", 0, 255, 0);
	data += serialize(&value->OrtZ, variableName + "Z", 0, 0, 255);

	return data + "\n";
}
std::string serialize(Edge* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	double leftPoint = value->t1;
	double rightPoint = value->t2;

	int count = 50;
	double step = (rightPoint - leftPoint) / count;
	for (int i = 0; i < count; ++i) {
		Point point = value->getPoint(leftPoint + step * i);

		data += serialize(&point, variableName + std::to_string(i), r, g, b);
	}

	return data + "\n";
}


std::string serialize(NetPoint* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "points: " + variableName + '\n' + "(" + std::to_string(value->getX()) + "," + std::to_string(value->getY()) + "," + std::to_string(value->getZ()) + ")";

	return data + "\n";
}

std::string serialize(NetEdge* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += serialize(&(value->getA()), variableName + ".netPoint", r, g, b);
	data += serialize(&(value->getB()), variableName + ".netPoint", r, g, b);

	data += "lines: " + variableName + ".lines\n";

	//(0.000000,5.000000,0.000000)(-0.626666,4.960574,0.000000)(1.000000,0.000000,0.000000)

	data += "(" + std::to_string(value->getA().getX()) + "," + std::to_string(value->getA().getY()) + "," + std::to_string(value->getA().getZ()) + ")" +
		"(" + std::to_string(value->getB().getX()) + "," + std::to_string(value->getB().getY()) + "," + std::to_string(value->getB().getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	return data + "\n";
}

std::string serialize(Net* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	for (int i = 0; i < value->getEdges().size(); ++i) {
		data += serialize(&(value->getEdges()[i]), variableName + ".netEdge" + std::to_string(i), r, g, b);
	}

	return data + "\n";
}

