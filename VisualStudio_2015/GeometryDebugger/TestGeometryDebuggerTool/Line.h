#include "Curve.h"
#include "Vector.h"
#include "Point.h"

#pragma once

class Line : public Curve {
private:

	Point startPoint;
	Point m_direction;

	Point getPointInLocalSK(const double& param) const {
		return m_direction * param + startPoint;
	}

public:

	Line(Point point, Vector direction)
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

	Line(Point startPoint, Point endPoint) :
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
		return currentPoint;
	}
};