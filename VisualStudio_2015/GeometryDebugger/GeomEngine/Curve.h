#include "Point.h"

#ifndef CURVE_H
#define CURVE_H

class Curve {

private:

public:
	Point virtual getPoint(const double& param) = 0;
};

#endif // !CURVE_H
