#include "Edge.h"

Edge CreateArcEdge(Circle* circle, double t1, double t2) {
	return Edge(t1, t2, circle);
}

Edge CreateStraightEdge(Line* line, double t1, double t2) {
	return Edge(t1, t2, line);
}

Edge CreateCustomEdge(CustomCurve* line, double t1, double t2) {
	return Edge(t1, t2, line);
}