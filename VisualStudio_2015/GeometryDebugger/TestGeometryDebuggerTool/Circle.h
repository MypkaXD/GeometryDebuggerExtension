#pragma once

#include "Curve.h"

class Circle : public Curve {
private:

	double m_r;
	Point m_center;

	Point getPointInLocalSK(const double& phi) const {
		return Point(m_r * cos(phi), m_r * sin(phi), 0);
	}

public:

	Circle() {
		m_r = 1;
		m_center = Point(0, 0, 0);
		Curve::setNormal(Point(0, 0, 1));
		Curve::setBasis();
	}
	Circle(double r) :
		m_r(r)
	{
		m_center = Point(0, 0, 0);

		Curve::setNormal(Point(0, 0, 1));

		Curve::setBasis();
	}
	Circle(double r, Point center, Point normal) :
		m_r(r), m_center(center) {
		Curve::setNormal(normal);
		Curve::setBasis();
	}

	double getRadius() {
		return m_r;
	}

	Point getCenter() {
		return m_center;
	}

	void setRadius(const double& radius) {
		m_r = radius;
	}

	Point getPoint(const double& phi) override {

		Point currentPoint = getPointInLocalSK(phi);

		currentPoint.transformToOtherSK(Curve::getXComp(), Curve::getYComp(), Curve::getNormal());

		return currentPoint + getCenter();
	}
};