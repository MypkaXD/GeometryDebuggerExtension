#include "Curve.h"
#include "Vector.h"
#include "Point.h"

#pragma once

class CustomCurve : public Curve {

private:

	Point startPoint;
	Point m_direction;
	Point(*getPointPtr)(double);

	Point getPointInLocalSK(const double& param) const {
		return getPointPtr(param);
	}

public:

	CustomCurve(Point point, Vector direction, Point(*getPointPtr)(double)) :
		getPointPtr(getPointPtr)
	{
		m_direction = direction.point;
		m_direction.normalize();

		Point ort = m_direction & Point(1, 0, 0);
		if (ort.length() < 0.3) {
			ort = m_direction & Point(0, 1, 0);
		}

		ort.normalize();
		Curve::setNormal(ort);
		Curve::setBasis();
		startPoint = point;
	}

	CustomCurve(Point startPoint, Point endPoint, Point(*getPointPtr)) :
		m_direction(endPoint - startPoint)
	{
		m_direction.normalize();

		Point ort = m_direction & Point(1, 0, 0);
		if (ort.length() < 0.3) {
			ort = m_direction & Point(0, 1, 0);
		}

		ort.normalize();
		Curve::setNormal(ort);
		Curve::setBasis();
		startPoint = startPoint;
	}

	Point getPoint(const double& param) override {

		Point currentPoint = getPointInLocalSK(param);
		return currentPoint + startPoint;
	}

};

