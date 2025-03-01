#ifndef PLANE_H
#define PLANE_H

#include "Surface.h"

class Plane : public Surface {
private:

	double uMin, uMax; // ����������� ��������� U
	double vMin, vMax; // ����������� ��������� V

public:

	Plane(Point origin, Point ox, Point oy, double uMin, double uMax, double vMin, double vMax)
		: Surface(origin, ox, oy), uMin(uMin), uMax(uMax), vMin(vMin), vMax(vMax) 
	{}

	Point getPoint(const double& u, const double& v) override {

		// ����������� ����� � ������� ��������� Plane
		return get_Origin() +
			getOX() * u +
			getOY() * v +
			getOZ() * 0;  // z-���������� ����� ����� 0 (��� ����� ������������ ������ ��������)
	}

	double getUMax() const override { return uMax; }
	double getUMin() const override { return uMin; }
	double getVMax() const override { return vMax; }
	double getVMin() const override { return vMin; }
};

#endif // PLANE_H
