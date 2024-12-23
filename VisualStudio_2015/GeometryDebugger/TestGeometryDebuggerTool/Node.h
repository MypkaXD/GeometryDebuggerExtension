#pragma once
#include "Point.h"

class Node
{
public:
	int top_id;
	PointGraph* coord;

	Node(int top, PointGraph coord);
	Node(int top, double x, double y, double z);
	double get_distance(Node a2);
};