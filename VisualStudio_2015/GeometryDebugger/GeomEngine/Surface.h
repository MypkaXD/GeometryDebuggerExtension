#include "Point.h"

#ifndef SURFACE_H
#define SURFACE_H

class Surface {
private:

	Point m_ox;
	Point m_oy;
	Point m_oz;

	Point m_origin;

public:

	Surface(Point origin, Point ox, Point oy) :
		m_origin(origin), m_ox(ox), m_oy(oy)
	{
		m_oz = m_ox & m_oy;
	}

	Point virtual getPoint(const double& u_param, const double& v_param) = 0;
	virtual double getUMax() const = 0;
	virtual double getUMin() const = 0;
	virtual double getVMax() const = 0;
	virtual double getVMin() const = 0;

	Point getOX() {
		return m_ox;
	}
	Point getOY() {
		return m_oy;
	}
	Point getOZ() {
		return m_oz;
	}
	Point get_Origin() {
		return m_origin;
	}
};

#endif // !SURFACE_H
