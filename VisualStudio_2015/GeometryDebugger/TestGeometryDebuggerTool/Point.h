#include <string>
#include <iostream>
#include <vector>
#include <tuple>

#define _USE_MATH_DEFINES
#include <math.h>

#pragma once

class Point {
private:

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

	double length() const {
		return std::sqrt(m_x * m_x + m_y * m_y + m_z * m_z);
	}

	Point& getNormalize() {

		double lenghtOfVec = length();

		if (lenghtOfVec > 1) {
			double x = m_x / lenghtOfVec;
			double y = m_y / lenghtOfVec;
			double z = m_z / lenghtOfVec;

			Point normalized = Point(x, y, z);

			return normalized;
		}
		else
			return *this;
	}

	void normalize() {

		double lenghtOfVec = length();

		if (lenghtOfVec > 1) {
			m_x /= lenghtOfVec;
			m_y /= lenghtOfVec;
			m_z /= lenghtOfVec;
		}
		else
			return;
	}

	void transformToOtherSK(const Point& xComp, const Point& yComp, const Point& zComp) {

		double x = xComp.getX() * m_x + yComp.getX() * m_y + zComp.getX() * m_z;
		double y = xComp.getY() * m_x + yComp.getY() * m_y + zComp.getY() * m_z;
		double z = xComp.getZ() * m_x + yComp.getZ() * m_y + zComp.getZ() * m_z;

		m_x = x;
		m_y = y;
		m_z = z;

	}
};

Point operator*(const Point& v, const double& scalar);
double operator*(const Point& v1, const Point& v2);
Point operator-(const Point& v1, const Point& v2);
Point operator+(const Point& v1, const Point& v2);
Point operator&(const Point& v1, const Point& v2);