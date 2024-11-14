#include "Vector.h"
#include "Curve.h"

#define _USE_MATH_DEFINES

#include <math.h>

#pragma once

class Circle : public Curve {
private:

	double m_r;
	Vector m_center;

    Vector getPointInLocalSK(const double& phi) const {
        return Vector(m_r * cos(phi), m_r * sin(phi), 0);
    }

public:

	Circle() {
		m_r = 1;
		m_center = Vector(0, 0, 0);
        Curve::setNormal(Vector(0,0,1));
        Curve::setBasis();
    }
	Circle(double r) :
		m_r(r)
	{
		m_center = Vector(0, 0, 0);
        
        Curve::setNormal(Vector(0, 0, 1));

        Curve::setBasis();
	}
	Circle(double r, Vector center, Vector normal) :
		m_r(r), m_center(center) {
        Curve::setNormal(normal);
        Curve::setBasis();
    }

	double getRadius() override {
		return m_r;
	}

	Vector getCenter() override {
		return m_center;
	}

    void setRadius(const double& radius) {
        m_r = radius;
    }

    Vector getPoint(const double& phi) override {

        Vector currentPoint = getPointInLocalSK(phi);

        currentPoint.transformToOtherSK(Curve::getXComp(), Curve::getYComp(), Curve::getNormal());
        
        return currentPoint + getCenter();
    }
};

std::string serialize(Circle* value, std::string typeName, std::string variableName, int r, int g, int b) {
    
    std::string data = "";
    int countOfCircle = 50;

    std::vector<std::tuple<double, double, double>> coords;

    for (int i = 0; i < countOfCircle; ++i) {
        double phi = 2 * M_PI * i / countOfCircle;
        Vector coord = value->getPoint(phi);
        coords.push_back(std::make_tuple(coord.getX(), coord.getY(), coord.getZ()));
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

std::string serialize(std::vector<Circle>* value, std::string typeName, std::string variableName, int r, int g, int b) {

    std::string data = "";
    
    for (int i = 0; i < value->size(); ++i) {
        data += serialize(&(*value)[i], typeName, variableName + std::to_string(i), r, g, b);
    }

    return data;

}