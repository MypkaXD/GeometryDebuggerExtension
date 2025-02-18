#include "Curve.h"

#ifndef CIRCLE_H
#define CIRCLE_H

class Circle : public Curve {
private:

	double m_r;
	Point m_center;

public:

	Circle() {
		m_r = 1;
		m_center = Point(0, 0, 0);
	}
	Circle(double r) :
		m_r(r)
	{
		m_center = Point(0, 0, 0);
	}
	Circle(double r, Point center, Point normal) :
		m_r(r), m_center(center) {
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

		Point currentPoint = Point(m_r * cos(phi), m_r * sin(phi), 0);

		return currentPoint + getCenter();
	}
};

#endif // !CIRCLE_H
