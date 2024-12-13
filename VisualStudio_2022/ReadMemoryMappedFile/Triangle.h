#include "Curve.h"
#include "Edge.h"
#include "Vector.h"

#pragma once

class Triangle {

private:

	Edge firstEdge;
	Edge secondEdge;
	Edge thirdEdge;

public:

	Triangle(Vector point1, Vector point2, Vector point3) {

		// первый Edge:

		Vector firstDirection = point2 - point1;
		firstDirection.normalize();


		
	}

};
