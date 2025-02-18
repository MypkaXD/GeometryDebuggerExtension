#ifndef PLANE_H
#define PLANE_H

#include "Surface.h"

class Plane : public Surface {
private:

	double uMin, uMax; // ќграничени€ параметра U
	double vMin, vMax; // ќграничени€ параметра V

public:

	Plane(Point origin, Point ox, Point oy, double uMin, double uMax, double vMin, double vMax)
		: Surface(origin, ox, oy), uMin(uMin), uMax(uMax), vMin(vMin), vMax(vMax) 
	{}

	Point getPoint(const double& u, const double& v) override {

		// ѕреобразуем точку в систему координат Plane
		return get_Origin() +
			getOX() * u +
			getOY() * v +
			getOZ() * 0;  // z-координата будет равна 0 (или можно использовать другое значение)
	}

	double getUMin() const { return uMin; }
	double getUMax() const { return uMax; }
	double getVMin() const { return vMin; }
	double getVMax() const { return vMax; }
};

#endif // PLANE_H
