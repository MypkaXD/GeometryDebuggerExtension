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

	double getUMin() const { return uMin; }
	double getUMax() const { return uMax; }
	double getVMin() const { return vMin; }
	double getVMax() const { return vMax; }
};

#endif // PLANE_H
