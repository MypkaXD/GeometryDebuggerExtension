#include "Surface.h"

#ifndef CUSTOMPLANE
#define CUSTOMPLANE

class CustomPlane : public Surface
{
private:

	Point(*getPointPtr)(double, double);

	double uMin, uMax; // ����������� ��������� U
	double vMin, vMax; // ����������� ��������� V

public:

	CustomPlane(Point origin, Point ox, Point oy,
		Point(*getPointPtr)(double, double),
		double uMin, double uMax, double vMin, double vMax)
		: Surface(origin, ox, oy), getPointPtr(getPointPtr),
		uMin(uMin), uMax(uMax), vMin(vMin), vMax(vMax) {}

	Point getPoint(const double& u, const double& v) override {
		Point localPoint = getPointPtr(u, v);
		Point globalPoint = Point(localPoint.getX()*getOX().getX() + localPoint.getY()*getOY().getX() + localPoint.getZ()*getOZ().getX() + get_Origin().getX(),
			localPoint.getX()*getOX().getY() + localPoint.getY()*getOY().getY() + localPoint.getZ()*getOZ().getY() + get_Origin().getY(),
			localPoint.getX()*getOX().getZ() + localPoint.getY()*getOY().getZ() + localPoint.getZ()*getOZ().getZ() + get_Origin().getZ());
		return globalPoint;
	}

	// ������� ��� ������
	double getUMin() const { return uMin; }
	double getUMax() const { return uMax; }
	double getVMin() const { return vMin; }
	double getVMax() const { return vMax; }
};

#endif // !CUSTOMPLANE
