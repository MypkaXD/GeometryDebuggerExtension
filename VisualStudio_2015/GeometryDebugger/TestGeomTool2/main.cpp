#include <iostream>
#include "Graph.h"
#include <string>


int main()
{
	Graph graph;
	//graph.fillRandom(100);


	graph.fillGrid(30, 30);

	//graph.fillRandom(50);


	Path p_s = graph.algDeikstra(69, 893);


	return 0;
}
