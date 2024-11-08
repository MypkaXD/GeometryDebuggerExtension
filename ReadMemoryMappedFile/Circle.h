#include "Vector.h"

#pragma once

class Circle {
private:

	double m_r;
	Vector m_normal;
	Vector m_center;

public:

	Circle() {
		m_r = 1;
		m_normal = Vector(0, 0, 1);
		m_center = Vector(0, 0, 0);
	}
	Circle(double r) :
		m_r(r)
	{
		m_normal = Vector(0, 0, 1);
		m_center = Vector(0, 0, 0);
	}
	Circle(double r, Vector center, Vector normal) :
		m_r(r), m_center(center), m_normal(normal) {
	}

	double getRadius() const {
		return m_r;
	}
	Vector getNormal() const {
		return m_normal;
	}
	Vector getCenter() const {
		return m_center;
	}

    void setRadius(const double& radius) {
        m_r = radius;
    }
};

std::string serialize(Circle* value, std::string typeName, std::string variableName, int r, int g, int b) {
    std::string path = typeName + "_" + variableName + ".txt";
    std::string data = "";
    int countOfCircle = 50;

    std::vector<std::tuple<double, double, double>> coords;

    for (int i = 0; i < countOfCircle; ++i) {
        double phi = 2 * M_PI * i / countOfCircle;
        double x = value->getCenter().getX() + value->getRadius() * cos(phi);
        double y = value->getCenter().getY() + value->getRadius() * sin(phi);
        double z = value->getCenter().getZ();
        coords.push_back(std::make_tuple(x, y, z));
    }

    // Формирование строк для записи сегментов круга
    data += "points: " + typeName + "_" + variableName + ".center\n";
    data += "(" + std::to_string(value->getCenter().getX()) + "," +
        std::to_string(value->getCenter().getY()) + "," +
        std::to_string(value->getCenter().getZ()) + ")\n";

    data += "lines: " + typeName + "_" + variableName + ".lines\n";

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

    return data;
}