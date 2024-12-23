#include <string>

#include "Point.h"
#include "Graph.h"
#include "Node.h"
#include "Path.h"
#include "Edge.h"

#pragma once

std::string serialize(Point* value, std::string variableName, float r, float g, float b)
{
	std::string data = "";

	data += "points: " + variableName + '\n' + "(" + std::to_string(value->x) + "," + std::to_string(value->y) + "," + std::to_string(value->z) + ")";

	return data + "\n";
}

std::string serialize(Edge* value, std::string variableName, float r, float g, float b)
{
	std::string data = "";

	data += serialize(&(*value->prev_point), variableName + ".prevPoint", r, g, b);
	data += serialize(&(*value->next_point), variableName + ".nextPoint", r, g, b);

	data += "lines: " + variableName + "\n";

	data += "(" + std::to_string((value->prev_point)->x) + "," + std::to_string((value->prev_point)->y) + "," + std::to_string((value->prev_point)->z) + ")"
		"(" + std::to_string((value->next_point)->x) + "," + std::to_string((value->next_point)->y) + "," + std::to_string((value->next_point)->z) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	return data + "\n";
}

std::string serialize(Graph* value, std::string variableName, float r, float g, float b)
{
	std::string data = "";

	for (int i = 0; i < value->edges.size(); ++i) {
		data += serialize(&(value->edges[i]), variableName + std::to_string(i), r, g, b);
	}

	return data + "\n";
}

std::string serialize(Path* value, std::string variableName, float r, float g, float b)
{
	std::string data = "";

	for (int i = 0; i < value->path.size(); ++i) {
		data += serialize((value->path[i].coord), variableName + ".point" + std::to_string(i), r, g, b);
	}

	data += "lines: " + variableName + "\n";

	for (int i = 0; i < value->path.size() - 1; ++i) {
		data += "(" + std::to_string((value->path[i].coord)->x) + "," + std::to_string((value->path[i].coord)->y) + "," + std::to_string((value->path[i].coord)->z) + ")"
			"(" + std::to_string((value->path[i+1].coord)->x) + "," + std::to_string((value->path[i+1].coord)->y) + "," + std::to_string((value->path[i+1].coord)->z) + ")";
		data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";
	}

	return data + "\n";
}
