#include "Point.h"

#ifndef VECTOR_H
#define VECTOR_H

class Vector {
private:
	std::pair<Point, Point> m_vector;
public:
	Vector(Point first, Point second) :
		m_vector(first, second)
	{
	}
	Vector(std::pair<Point, Point> vector) :
		m_vector(vector)
	{
	}
	std::pair<Point, Point> getVector() {
		return m_vector;
	}
};

#endif // !VECTOR_H
