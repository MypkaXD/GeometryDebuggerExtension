#include <iostream>
#include "Graph.h"
#include <string>


int main()
{
	Graph graph;
	//graph.fillRandom(100);


	graph.fillGrid(7, 7);

	//graph.fillRandom(50);


	Path p_s = graph.algDeikstra(0, 38);


	return 0;
}
