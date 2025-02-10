#include <iostream>
#include <vector>

#include "Circle.h"
#include "Vector.h"
#include "Line.h"
#include "Point.h"
#include "Coordinate_system.h"
#include "Edge.h"
#include "Net.h"

Point getPointForCustomCurve(double param) {
	return Point(std::sin(param), std::cos(param), param);
}

Net getFirstNet() {
	NetPoint a1 = NetPoint(0, 0, 0);
	NetPoint a2 = NetPoint(1, 0, 0);
	NetPoint a3 = NetPoint(1.2, 0, 0);
	NetPoint a4 = NetPoint(3, 0, 0);
	NetPoint b1 = NetPoint(0, 1, 0);
	NetPoint b2 = NetPoint(1, 1, 0);
	NetPoint b3 = NetPoint(2, 1, 0);
	NetPoint b4 = NetPoint(3, 1, 0);

	NetEdge a1a2 = NetEdge(a1, a2);
	NetEdge a2a3 = NetEdge(a2, a3);
	NetEdge a3a4 = NetEdge(a3, a4);

	NetEdge a1b1 = NetEdge(a1, b1);
	NetEdge a2b2 = NetEdge(a2, b2);
	NetEdge a3b3 = NetEdge(a3, b3);
	NetEdge a4b4 = NetEdge(a4, b4);

	NetEdge b1b2 = NetEdge(b1, b2);
	NetEdge b2b3 = NetEdge(b2, b3);
	NetEdge b3b4 = NetEdge(b3, b4);

	Net net = Net();

	net.addEdge(a1a2);
	net.addEdge(a2a3);
	net.addEdge(a3a4);

	net.addEdge(a1b1);
	net.addEdge(a2b2);
	net.addEdge(a3b3);
	net.addEdge(a4b4);

	net.addEdge(b1b2);
	net.addEdge(b2b3);
	net.addEdge(b3b4);

	return net;
}

Net fixNet() {
	NetPoint a1 = NetPoint(0, 0, 0);
	NetPoint a2 = NetPoint(1, 0, 0);
	NetPoint a3 = NetPoint(2, 0, 0);
	NetPoint a4 = NetPoint(3, 0, 0);
	NetPoint b1 = NetPoint(0, 1, 0);
	NetPoint b2 = NetPoint(1, 1, 0);
	NetPoint b3 = NetPoint(2, 1, 0);
	NetPoint b4 = NetPoint(3, 1, 0);

	NetEdge a1a2 = NetEdge(a1, a2);
	NetEdge a2a3 = NetEdge(a2, a3);
	NetEdge a3a4 = NetEdge(a3, a4);

	NetEdge a1b1 = NetEdge(a1, b1);
	NetEdge a2b2 = NetEdge(a2, b2);
	NetEdge a3b3 = NetEdge(a3, b3);
	NetEdge a4b4 = NetEdge(a4, b4);

	NetEdge b1b2 = NetEdge(b1, b2);
	NetEdge b2b3 = NetEdge(b2, b3);
	NetEdge b3b4 = NetEdge(b3, b4);

	Net net = Net();

	net.addEdge(a1a2);
	net.addEdge(a2a3);
	net.addEdge(a3a4);

	net.addEdge(a1b1);
	net.addEdge(a2b2);
	net.addEdge(a3b3);
	net.addEdge(a4b4);

	net.addEdge(b1b2);
	net.addEdge(b2b3);
	net.addEdge(b3b4);

	return net;
}

int main() {

	CoordinateSystem const cs = CoordinateSystem();
	
	const CoordinateSystem& cs2 = cs;

	NetPoint point = NetPoint(0, 2.5, 4);

	Net net = getFirstNet();
	Net net2 = fixNet();

	std::cout << "End" << std::endl;

	return 0;

}