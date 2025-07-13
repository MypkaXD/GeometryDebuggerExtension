#include "Curve.h"

#ifndef CUSTOMCURVE_H
#define CUSTOMCURVE_H

class CustomCurve : public Curve
{
private:

	Point m_origin;
	Point(*getPointPtr)(double);

public:
	CustomCurve(Point point, Point(*getPointPtr)(double)) :
		getPointPtr(getPointPtr), m_origin(point)
	{
	}

	Point getPoint(const double& param) override {
		return getPointPtr(param) + m_origin;
	}
};

#endif // !CUSTOMCURVE_H
