#include <iostream>
#include <vector>

#ifndef POINT_H
#define POINT_H

class Point {

private :

	double m_x, m_y, m_z;

public:

	Point() {
		m_x = 0;
		m_y = 0;
		m_z = 0;
	}

	Point(double x, double y, double z) :
		m_x(x), m_y(y), m_z(z) {
	}

	double getX() const {
		return m_x;
	}
	double getY() const {
		return m_y;
	}
	double getZ() const {
		return m_z;
	}

	void setX(const double& x) {
		m_x = x;
	}
	void setY(const double& y) {
		m_y = y;
	}
	void setZ(const double& z) {
		m_z = z;
	}

};

Point operator-(const Point& v1, const Point& v2);
Point operator+(const Point& v1, const Point& v2);
Point operator/(const Point& point, const double& value);
Point operator&(const Point& v1, const Point& v2);
Point operator*(const Point& v, const double& scalar);

#endif // !POINT_H
