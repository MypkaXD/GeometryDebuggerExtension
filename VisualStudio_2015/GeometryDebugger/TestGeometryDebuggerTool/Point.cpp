#include "Point.h"

Point operator+(const Point& left, const Point& right) {
	return Point(left.getX() + right.getX(), left.getY() + right.getY(), left.getZ() + right.getZ());
}

Point operator-(const Point& left, const Point& right) {
	return Point(left.getX() - right.getX(), left.getY() - right.getY(), left.getZ() - right.getZ());
}

Point operator*(const Point& vector, const double& value) {
	return Point(vector.getX() * value, vector.getY() * value, vector.getZ() * value);
}

double operator*(const Point& left, const Point& right) {
	return (left.getX() * right.getX() + left.getY() * right.getY() + left.getZ() * right.getZ());
}

Point operator&(const Point& left, const Point& right) {

	double x = left.getY() * right.getZ() - left.getZ() * right.getY();
	double y = left.getZ() * right.getX() - left.getX() * right.getZ();
	double z = left.getX() * right.getY() - left.getY() * right.getX();

	return Point(x, y, z);
}