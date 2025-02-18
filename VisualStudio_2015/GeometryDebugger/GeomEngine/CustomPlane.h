#include "Surface.h"

#ifndef CUSTOMPLANE
#define CUSTOMPLANE

class CustomPlane : public Surface
{
private:

	Point(*getPointPtr)(double, double);

	double uMin, uMax; // Ограничения параметра U
	double vMin, vMax; // Ограничения параметра V

public:

	CustomPlane(Point origin, Point ox, Point oy,
		Point(*getPointPtr)(double, double),
		double uMin, double uMax, double vMin, double vMax)
		: Surface(origin, ox, oy), getPointPtr(getPointPtr),
		uMin(uMin), uMax(uMax), vMin(vMin), vMax(vMax) {}

	Point getPoint(const double& u, const double& v) override {
		Point localPoint = getPointPtr(u, v); // Локальная точка

		return get_Origin() +
			getOX() * localPoint.getX() +
			getOY() * localPoint.getY() +
			getOZ() * localPoint.getZ();
	}

	// Геттеры для границ
	double getUMin() const { return uMin; }
	double getUMax() const { return uMax; }
	double getVMin() const { return vMin; }
	double getVMax() const { return vMax; }
};

#endif // !CUSTOMPLANE
