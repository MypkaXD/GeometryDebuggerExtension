#include "Curve.h"
#include "Circle.h"

#ifndef EDGE_H
#define EDGE_H


class Edge {
private:
	Curve* curve;
	double t1, t2;
public:
	Edge(Curve* curve, double t1, double t2) :
		curve(curve), t1(t1), t2(t2)
	{
	}
	Point getPoint(const double& param) {
		return curve->getPoint(param);
	}

	const std::pair<double, double> getParams() {
		return std::make_pair(t1, t2);
	}

};

Edge CreateArcEdge(Circle* circle, double t1, double t2);

#endif // !EDGE_H
