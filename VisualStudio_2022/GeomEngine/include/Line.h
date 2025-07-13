#include "Point.h"
#include "Curve.h"

#ifndef LINE_H
#define LINE_H

class Line : public Curve
{
private:
	Point m_origin;
	Point m_direction;
public:
	Line(const Point& origin, const Point& direction)
		: m_origin(origin), m_direction(direction) {}
	Point getPoint(const double& t) override {
		return Point(
			m_origin.getX() + t * m_direction.getX(),
			m_origin.getY() + t * m_direction.getY(),
			m_origin.getZ() + t * m_direction.getZ()
		);
	}
};

#endif // !LINE_H
