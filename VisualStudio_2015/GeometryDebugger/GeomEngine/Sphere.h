#include "Surface.h"

#ifndef SPHERE_H
#define SPHERE_H

class Sphere : public Surface
{
private:

	double m_r;

	double uMin, uMax; // Ограничения параметра U
	double vMin, vMax; // Ограничения параметра V

	Point getPointInLocal(const double& u, const double& v) {
		return Point(m_r * std::cos(v)*std::cos(u), m_r *std::sin(u)*std::cos(v), m_r *std::sin(v));
	}

public:
	Sphere(Point origin, Point ox, Point oy, double r, double uMin, double uMax, double vMin, double vMax)
		: Surface(origin, ox, oy), uMin(uMin), uMax(uMax), vMin(vMin), vMax(vMax), m_r(r)
	{}

	Point getPoint(const double& u, const double& v) override {

		Point localPoint = getPointInLocal(u, v);
		Point globalPoint = Point(localPoint.getX()*getOX().getX() + localPoint.getY()*getOY().getX() + localPoint.getZ()*getOZ().getX() + get_Origin().getX(),
			localPoint.getX()*getOX().getY() + localPoint.getY()*getOY().getY() + localPoint.getZ()*getOZ().getY() + get_Origin().getY(),
			localPoint.getX()*getOX().getZ() + localPoint.getY()*getOY().getZ() + localPoint.getZ()*getOZ().getZ() + get_Origin().getZ());
		return globalPoint;
	}

	double getUMax() const override { return uMax; }
	double getUMin() const override { return uMin; }
	double getVMax() const override { return vMax; }
	double getVMin() const override { return vMin; }
};

#endif // !SPHERE_H
