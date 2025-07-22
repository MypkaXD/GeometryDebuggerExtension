#include <memory>  
#include <tuple>
#include <string>
#include <iostream>
#include <algorithm>
#include <fstream>
#include <limits>
#include <chrono>

#define NOMINMAX
#include <windows.h>  

#define _USE_MATH_DEFINES
#include <math.h>

#include "Point.h"
#include "Vector.h"
#include "Curve.h"
#include "Line.h"
#include "Edge.h"
#include "Plane.h"
#include "Surface.h"
#include "CustomPlane.h"
#include "Sphere.h"
#include "Cylinder.h"
#include "Face.h"

#include <geom_view.h>
geom_view gv;
Tree* tree;

//std::vector<Point> points = {
//	Point(1, 5, 0), Point(5, 10, 0), Point(7, 7, 0), Point(6, 2, 0), Point(4,6, 0), Point(2, 3, 0)
//};

std::vector<Point> points = {
	Point(1, 5, 0), Point(5, 10, 0), Point(7, 7, 0)
};

void dump();

Point getPointForCustomCurve(double param) {
	return Point(std::sin(param), std::cos(param), param);
}

Point getPointForSphere(double u, double v) {
	return Point(std::cos(v) * std::cos(u), std::sin(u) * std::cos(v), std::sin(v));
}

Point getPointForSpiral(double u, double v) {
	return Point(std::cos(u) * (std::cos(v) + 3), std::sin(u) * (std::cos(v) + 3), std::sin(v) + u);
}

Point getPointForMebius(double u, double v) {
	return Point((1 + v / 2 * std::cos(u / 2)) * std::cos(u), (1 + v / 2 * std::cos(u / 2)) * std::sin(u), v / 2 * std::sin(u / 2));
}

Point getPointForDini(double u, double v) {
	return Point(std::cos(u) * std::sin(v), std::sin(u) * std::sin(v), std::cos(v) + std::log10(std::tan(v / 2)) + 0.2 * u - 4);
}

void moveControl(void* callback_data, std::vector<std::string>& sId, double x, double y, double z) {
	geom_view& gv = *static_cast<geom_view*>(callback_data);
	int id = atoi(sId.back().c_str());

	size_t pos = sId[0].find("_");
	int number = std::stoi(sId[0].substr(pos + 1, sId[0].size() - 1 - pos));
	points[number] = Point(x, y, 0);
	dump();
	gv.reload();
}


std::string serialize_box(BoundingBox* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "lines: \n";

	Point startPoint = value->m_start_point;
	Point endPoint = value->m_end_point;

	data += "(" + std::to_string(startPoint.getX()) + "," + std::to_string(startPoint.getY()) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(startPoint.getX()) + "," + std::to_string(startPoint.getY() + value->m_height) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	data += "(" + std::to_string(startPoint.getX()) + "," + std::to_string(startPoint.getY() + value->m_height) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(startPoint.getX() + value->m_width) + "," + std::to_string(startPoint.getY() + value->m_height) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	data += "(" + std::to_string(startPoint.getX() + value->m_width) + "," + std::to_string(startPoint.getY() + value->m_height) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(startPoint.getX() + value->m_width) + "," + std::to_string(startPoint.getY()) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	data += "(" + std::to_string(startPoint.getX() + value->m_width) + "," + std::to_string(startPoint.getY()) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(startPoint.getX()) + "," + std::to_string(startPoint.getY()) + "," + std::to_string(startPoint.getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";


	return data += "\n";
}
std::string serialize_plate(Plate* value, std::string variableName, float r, float g, float b) {

	std::string data = "";


	//std::cout << "SIZE OF: " << value->get_points().size() << std::endl;

	for (int j = 0; j < value->get_points().size(); ++j) {

		//std::cout << "size of " << j << value->get_points()[j].size() << std::endl;

		if (value->get_points()[j].size() >= 2) {

			data += "triangles: " + variableName + std::to_string(j) + "\n";

			for (int i = 0; i < value->get_points()[j].size() - 2; i += 1) {

				data += "(" + std::to_string(value->get_points()[j][0].getX()) + "," + std::to_string(value->get_points()[j][0].getY()) + "," + std::to_string(value->get_points()[j][0].getZ()) + ")";
				data += "(" + std::to_string(value->get_points()[j][i + 1].getX()) + "," + std::to_string(value->get_points()[j][i + 1].getY()) + "," + std::to_string(value->get_points()[j][i + 1].getZ()) + ")";
				data += "(" + std::to_string(value->get_points()[j][i + 2].getX()) + "," + std::to_string(value->get_points()[j][i + 2].getY()) + "," + std::to_string(value->get_points()[j][i + 2].getZ()) + ")";
				data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

			}
		}
	}



	return data += "\n";
}
std::string serialize_point(Point* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "control_points: " + variableName + '\n' + "(" + std::to_string(value->getX()) + "," + std::to_string(value->getY()) + "," + std::to_string(value->getZ()) + ")";
	data += "3";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")";
	//data += "(1,0,0)";

	return data + "\n";
}
std::string serialize_vector(Vector* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "vectors: " + variableName + ".lines\n";

	data += "(" + std::to_string(value->getVector().first.getX()) + "," +
		std::to_string(value->getVector().first.getY()) + "," +
		std::to_string(value->getVector().first.getZ()) + ")";
	data += "(" + std::to_string(value->getVector().second.getX()) + "," +
		std::to_string(value->getVector().second.getY()) + "," +
		std::to_string(value->getVector().second.getZ()) + ")";
	data += "(" + std::to_string(r) + "," + std::to_string(g) + "," + std::to_string(b) + ")\n";

	return data + "\n";
}
std::string serialize_edge(Edge* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	std::pair<double, double> params = value->getParams();
	Point start_point = value->getPoint(params.first);
	Point end_point = value->getPoint(params.second);

	Vector line = Vector(start_point, end_point);

	data += serialize_vector(&line, variableName + "line", r, g, b);

	return data + "\n";
}
std::string serialize_edges(std::vector<Edge>* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	for (int i = 0; i < value->size(); ++i) {
		data += serialize_edge(&(*value)[i], variableName + std::to_string(i), r, g, b);
	}

	return data + "\n";
}
std::string serialize_node(Node* value, std::string variableName, float r, float g, float b) {
	std::string data = "";

	if (value == nullptr)
		return "";
	if (value->m_is_list) {
		data += serialize_box(&value->m_box, variableName + "_box", r, g, b);
		for (int i = 0; i < value->m_edges.size(); ++i) {
			if (value->m_edges[i] != nullptr)
				data += serialize_edge(value->m_edges[i], variableName + "_edges_" + std::to_string(i), r, g, b);
		}
	}
	else {
		for (int i = 0; i < value->m_childrens.size(); ++i) {
			if (value->m_childrens[i] != nullptr)
				data += serialize_node(value->m_childrens[i], variableName + "_" + std::to_string(i), r, g, b);
		}
	}

	return data + "\n";
}
std::string serialize_tree(Tree* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	for (int i = 0; i < value->get_root()->m_childrens.size(); ++i)
		data += serialize_node(value->get_root()->m_childrens[i], variableName + "_" + std::to_string(i), r, g, b);

	return data + "\n";
}

void draw_current_node(const Node* current_node, std::vector<Plate>& plates) {

	if (current_node != nullptr) {
		if (current_node->m_is_list) {

			std::vector<Edge*> edges;

			for (int i = 0; i < current_node->m_edges.size(); ++i) {
				if (current_node->m_edges[i] != nullptr)
					edges.emplace_back(current_node->m_edges[i]);
			}

			std::vector<std::vector<std::pair<float, float>>> cuts = get_cut_of_figure(current_node->m_box, edges);

			std::vector<std::vector<Point>> points_cuts;
			for (int i = 0; i < cuts.size(); ++i) {
				std::vector<Point> points;
				for (int j = 0; j < cuts[i].size(); ++j) {
					points.push_back(Point(cuts[i][j].first, cuts[i][j].second, 0));
				}
				points_cuts.push_back(points);
			}

			if (points_cuts.size() == 0) {
				if (is_bounding_box_inside(current_node, get_equation_of_line(current_node->m_box.m_start_point + Point(current_node->m_box.m_width / 2, current_node->m_box.m_height / 2, 0), current_node->m_box.m_start_point + Point(current_node->m_box.m_width, current_node->m_box.m_height / 2, 0)))) {
					plates.emplace_back(Plate(
						{
							{ current_node->m_box.m_start_point, current_node->m_box.m_start_point + Point(0,current_node->m_box.m_height,0), current_node->m_box.m_end_point,current_node->m_box.m_start_point + Point(current_node->m_box.m_width,0,0)
							}
						}
					));
				}
			}
			else
				plates.emplace_back(points_cuts);
		}
	}
}

void draw_subtree(Node* current_node, std::vector<Plate>& plates) {
	if (current_node == nullptr)
		return;

	draw_current_node(current_node, plates);

	for (Node* child : current_node->m_childrens)
		draw_subtree(child, plates);
}

void draw(const Node* current_node, std::vector<Plate>& plates) {

	if (current_node == nullptr)
		return;

	draw_current_node(current_node, plates);

	Node* parrent = current_node->m_parent;

	if (parrent != nullptr) {

		int child_index = -1;

		for (int i = 0; i < parrent->m_childrens.size(); ++i) {
			if (parrent->m_childrens[i] == current_node) {
				child_index = i;
				break;
			}
		}

		if (child_index != -1) {
			for (int i = 0; i < parrent->m_childrens.size(); ++i) {
				if (i != child_index && parrent->m_childrens[i] != nullptr) {
					draw_subtree(parrent->m_childrens[i], plates);
				}
			}
		}
		draw(parrent, plates);
	}
	else
		return;
}

void save_points_in_binary_file() {

	std::string file_name = "cases_for_cut_bounding_box\\" + std::to_string(std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count()) + ".txt";

	std::ofstream file(file_name, std::ios::binary);
	if (!file.is_open())
		return;
	for (int i = 0; i < points.size(); ++i) {

		double x = points[i].getX();
		double y = points[i].getY();
		double z = points[i].getZ();

		file.write(reinterpret_cast<const char*>(&x), sizeof(double));
		file.write(reinterpret_cast<const char*>(&y), sizeof(double));
		file.write(reinterpret_cast<const char*>(&z), sizeof(double));
	}
	file.close();
}

void dump() {

	save_points_in_binary_file();

	BoundingBox box;
	std::vector<Edge> edges;
	std::vector<Line> lines;

	float x_min_box = std::numeric_limits<float>::max();
	float x_max_box = std::numeric_limits<float>::lowest();
	float y_min_box = std::numeric_limits<float>::max();
	float y_max_box = std::numeric_limits<float>::lowest();

	for (int i = 0; i < points.size(); ++i) {

		if (points[i].getX() > x_max_box)
			x_max_box = points[i].getX();
		if (points[i].getX() < x_min_box)
			x_min_box = points[i].getX();

		if (points[i].getY() > y_max_box)
			y_max_box = points[i].getY();
		if (points[i].getY() < y_min_box)
			y_min_box = points[i].getY();

	}

	box = BoundingBox(Point(x_min_box, y_min_box, 0), Point(x_max_box, y_max_box, 0));

	//box = BoundingBox(box.m_start_point - Point(10, 10, 0), box.m_end_point + Point(10, 10, 0));

	for (int i = 0; i < points.size() - 1; ++i)
		lines.emplace_back(points[i], points[i + 1] - points[i]);
	lines.emplace_back(points[points.size() - 1], points[0] - points[points.size() - 1]);

	for (int i = 0; i < lines.size(); ++i)
		edges.emplace_back(&lines[i], 0, 1);

	std::vector<Edge*> edges_in_tree;
	for (int i = 0; i < edges.size(); ++i)
		edges_in_tree.emplace_back(&edges[i]);

	std::vector<Plate> plates;

	auto start_time = std::chrono::steady_clock::now();

	tree = new Tree(box, edges_in_tree);

	const Node* list = tree->get_root()->get_list();

	if (list != nullptr) {
		draw(list, plates);
	}
	
	auto duration = std::chrono::steady_clock::now() - start_time;
	std::cout << "Time for creating tree: "
		<< std::chrono::duration_cast<std::chrono::milliseconds>(duration).count()
		<< " ms" << std::endl;

	//std::cout << "end" << std::endl;
	//std::cout << "end" << std::endl;


	std::ofstream file;
	file.open("serialize.txt");

	if (file.is_open()) {
		file << serialize_box(&box, "box", 1, 1, 1);
		file << serialize_edges(&edges, "edges", 1, 1, 1);
		file << serialize_tree(tree, "tree", 1, 1, 1);
		for (int i = 0; i < plates.size(); ++i)
			file << serialize_plate(&plates[i], "plates_" + std::to_string(i), 1, 1, 1);
		for (int i = 0; i < points.size(); ++i)
			file << serialize_point(&points[i], "points_" + std::to_string(i), 1, 1, 1);
	}

	file.close();
}

void read_binary_data_from_file() {

	std::string file_path = "cases_for_cut_bounding_box\\1753198883114.txt";

	std::ifstream file(file_path, std::ios::binary);
	if (!file.is_open())
		return;

	file.seekg(0, std::ios::end);
	std::streamsize size = file.tellg();
	file.seekg(0, std::ios::beg);

	points.clear();

	std::streamsize current_pos = 0;

	while (current_pos < size) {

		double x = 0;
		double y = 0;
		double z = 0;

		file.read((char*)(&x), sizeof(double));
		file.read((char*)(&y), sizeof(double));
		file.read((char*)(&z), sizeof(double));

		current_pos = file.tellg();

		points.emplace_back(x, y, z);
		
	}

	dump();
	gv.reload();
}

void create_random_triangle() {

	float x_cener = rand() % 10;
	float y_cener = rand() % 10;

	float radius = rand() % 100 + 1;

	int count_of_points = 10;
	points.resize(count_of_points);

	std::vector<float> angles(count_of_points);

	for (int i = 0; i < count_of_points; ++i) {
		angles[i] = rand() % 360;
		std::cout << angles[i] << std::endl;
	}

	std::sort(angles.begin(), angles.end(), std::greater<float>());

	for (int i = 0; i < count_of_points; ++i) {

		//float radius = rand() % 10 + 1;

		float current_angle = angles[i] * M_PI / 180;

		points[i] = Point(x_cener + radius * std::cos(current_angle), y_cener + radius * std::sin(current_angle), 0);
	}

	dump();
	gv.reload();

}

int main() {

	srand(time(0));

	////setlocale(LC_ALL, "rus");

	Point origin = Point(0, 0, 0);
	Point x = Point(1, 0, 0);
	Point y = Point(0, 1, 0);
	Point z = Point(0, 0, 1);
	Vector ox = Vector(origin, x);
	Vector oy = Vector(origin, y);
	Vector oz = Vector(origin, z);

	//dump();

	

	gv.init("serialize.txt");
	gv.setCallBack((void*)&gv, &moveControl);

	std::shared_ptr<geom_view_control_panel> panel;
	std::shared_ptr<geom_view_control_button> button;
	std::shared_ptr<geom_view_control_button> button_for_read_from_file;
	std::shared_ptr<geom_view_control_button> button_random_triangle;
	panel = geom_view_control_panel::makeCustomPanel("panel");
	gv.addCustomControl(std::static_pointer_cast<geom_view_control>(panel));
	button = geom_view_control_button::makeCustomButton("recreate_tree");
	button_for_read_from_file = geom_view_control_button::makeCustomButton("read_binary_file_with_points");
	button_random_triangle = geom_view_control_button::makeCustomButton("random_triangle");
	panel->add(std::static_pointer_cast<geom_view_control>(button));
	panel->add(std::static_pointer_cast<geom_view_control>(button_for_read_from_file));
	panel->add(std::static_pointer_cast<geom_view_control>(button_random_triangle));
	button->callback = [](void* data) {
		dump();
	};
	button_for_read_from_file->callback = [](void* data) {
		read_binary_data_from_file();
	};
	button_random_triangle->callback = [](void* data) {
		create_random_triangle();
	};

	std::string cmd;
	while (cmd != "exit") {
		std::cin >> cmd;
	}

	delete tree;

	return 0;

}