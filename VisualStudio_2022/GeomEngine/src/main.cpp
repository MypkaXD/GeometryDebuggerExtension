#include <memory>  
#include <tuple>
#include <string>
#include <iostream>
#include <algorithm>
#include <fstream>

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

Plate plate;
BoundingBox box;
Point start_point;
Point middle_point;
Point end_point;
std::vector<Edge> current_edge;

void dump();

Point getPointForCustomCurve(double param) {
	return Point(std::sin(param), std::cos(param), param);
}

Point getPointForSphere(double u, double v) {
	return Point(std::cos(v)*std::cos(u), std::sin(u)*std::cos(v), std::sin(v));
}

Point getPointForSpiral(double u, double v) {
	return Point(std::cos(u)*(std::cos(v) + 3), std::sin(u)*(std::cos(v) + 3), std::sin(v) + u);
}

Point getPointForMebius(double u, double v) {
	return Point((1 + v / 2 * std::cos(u / 2))*std::cos(u), (1 + v / 2 * std::cos(u / 2))*std::sin(u), v / 2 * std::sin(u / 2));
}

Point getPointForDini(double u, double v) {
	return Point(std::cos(u)*std::sin(v), std::sin(u)*std::sin(v), std::cos(v) + std::log10(std::tan(v / 2)) + 0.2*u - 4);
}

void moveControl(void* callback_data, std::vector<std::string>& sId, double x, double y, double z) {
	geom_view& gv = *static_cast<geom_view*>(callback_data);
	int id = atoi(sId.back().c_str());
	if (sId[0] == "end") {
		end_point = Point(x, y, 0);
	}
	else if(sId[0] == "start") {
		start_point = Point(x, y, 0);
	}
	else if (sId[0] == "middle") {
		middle_point = Point(x, y, 0);
	}
	dump();
	gv.reload();
}


std::string serialize_box(BoundingBox* value, std::string variableName, float r, float g, float b) {

	std::string data = "";

	data += "lines: \n";

	Point startPoint = Point(value->m_x_min, value->m_y_min, 0);
	Point endPoint = Point(value->m_x_min + value->m_width, value->m_y_min + value->m_height, 0);

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


	std::cout << "SIZE OF: " << value->get_points().size() << std::endl;

	for (int j = 0; j < value->get_points().size(); ++j) {

		std::cout << "size of " << j << value->get_points()[j].size() << std::endl;

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

void dump() {

	std::ofstream file;
	file.open("serialize.txt");

	Line current_line_first = Line(start_point, Point(middle_point.getX() - start_point.getX(), middle_point.getY() - start_point.getY(), 0));
	Line current_line_second = Line(middle_point, Point(end_point.getX() - middle_point.getX(), end_point.getY() - middle_point.getY(), 0));
	current_edge[0] = Edge(&current_line_first, 0, 1);
	current_edge[1] = Edge(&current_line_second, 0, 1);

	std::vector<std::vector<std::pair<float, float>>> cuts = get_cut_of_figure(box, std::vector<Edge>{current_edge}, file);

	std::vector<std::vector<Point>> points_cuts;
	for (int i = 0; i < cuts.size(); ++i) {
		std::vector<Point> points;
		for (int j = 0; j < cuts[i].size(); ++j) {
			points.push_back(Point(cuts[i][j].first, cuts[i][j].second, 0));
		}
		points_cuts.push_back(points);
	}
	plate = Plate(points_cuts);

	if (file.is_open()) {
		file << serialize_box(&box, "box", 1, 1, 1);
		file << serialize_plate(&plate, "plate", 1, 1, 1);
		file << serialize_edges(&current_edge, "edge", 1, 1, 1);

		file << serialize_point(&start_point, "start", 1, 0, 0);
		file << serialize_point(&middle_point, "middle", 0, 1, 0);
		file << serialize_point(&end_point, "end", 0, 0, 1);

	}


	file.close();
}

int main() {

	////setlocale(LC_ALL, "rus");

	Point origin = Point(0, 0, 0);
	Point x = Point(1, 0, 0);
	Point y = Point(0, 1, 0);
	Point z = Point(0, 0, 1);
	Vector ox = Vector(origin, x);
	Vector oy = Vector(origin, y);
	Vector oz = Vector(origin, z);

	current_edge.push_back(Edge());
	current_edge.push_back(Edge());

	box = BoundingBox(Point(0, 0, 0), Point(2, 2, 0));
	start_point = Point(-0.2, 1.7, 0);
	end_point = Point(2.5, 0.4, 0);
	middle_point = Point(1.5, 0.4, 0);
	Line current_line_first = Line(start_point, Point(middle_point.getX() - start_point.getX(), middle_point.getY() - start_point.getY(), 0));
	Line current_line_second = Line(middle_point, Point(end_point.getX() - middle_point.getX(), end_point.getY() - middle_point.getY(), 0));
	current_edge[0] = Edge(&current_line_first, 0, 1);
	current_edge[1] = Edge(&current_line_second, 0, 1);

	std::ofstream file;

	std::vector<std::vector<std::pair<float, float>>> cuts = get_cut_of_figure(box, std::vector<Edge>{current_edge}, file);

	std::vector<std::vector<Point>> points_cuts;
	for (int i = 0; i < cuts.size(); ++i) {
		std::vector<Point> points;
		for (int j = 0; j < cuts[i].size(); ++j) {
			points.push_back(Point(cuts[i][j].first, cuts[i][j].second, 0));
		}
		points_cuts.push_back(points);
	}
	plate = Plate(points_cuts);

	std::cout << "end" << std::endl;
	std::cout << "end" << std::endl;

	dump();

	geom_view gv;
	gv.init("serialize.txt");
	gv.setCallBack((void*)&gv, &moveControl);

	size_t apasd = 0 - 1;

	std::cout <<"asdasd: " << apasd << std::endl;

	std::string cmd;
	while (cmd != "exit") {
		std::cin >> cmd;
	}


	//Point outer1(0, 3, 0);
	//Point outer2(2, 2, 0);
	//Point outer3(3, 0, 0);
	//Point outer4(2, -2, 0);
	//Point outer5(0, -3, 0);
	//Point outer6(-2, -2, 0);
	//Point outer7(-3, 0, 0);
	//Point outer8(-2, 2, 0);

	//// Внутренние вершины звезды (углубления между лучами)
	//Point inner1(0, 1, 0);
	//Point inner2(1, 1, 0);
	//Point inner3(1, 0, 0);
	//Point inner4(1, -1, 0);
	//Point inner5(0, -1, 0);
	//Point inner6(-1, -1, 0);
	//Point inner7(-1, 0, 0);
	//Point inner8(-1, 1, 0);

	//// Создаем линии для лучей звезды (используем shared_ptr)
	//std::vector<Line> lines = {
	//	Line(outer1, Point(inner1.getX() - outer1.getX(), inner1.getY() - outer1.getY(), 0)),
	//	Line(inner1, Point(outer2.getX() - inner1.getX(), outer2.getY() - inner1.getY(), 0)),
	//	Line(outer2, Point(inner2.getX() - outer2.getX(), inner2.getY() - outer2.getY(), 0)),
	//	Line(inner2, Point(outer3.getX() - inner2.getX(), outer3.getY() - inner2.getY(), 0)),
	//	Line(outer3, Point(inner3.getX() - outer3.getX(), inner3.getY() - outer3.getY(), 0)),
	//	Line(inner3, Point(outer4.getX() - inner3.getX(), outer4.getY() - inner3.getY(), 0)),
	//	Line(outer4, Point(inner4.getX() - outer4.getX(), inner4.getY() - outer4.getY(), 0)),
	//	Line(inner4, Point(outer5.getX() - inner4.getX(), outer5.getY() - inner4.getY(), 0)),
	//	Line(outer5, Point(inner5.getX() - outer5.getX(), inner5.getY() - outer5.getY(), 0)),
	//	Line(inner5, Point(outer6.getX() - inner5.getX(), outer6.getY() - inner5.getY(), 0)),
	//	Line(outer6, Point(inner6.getX() - outer6.getX(), inner6.getY() - outer6.getY(), 0)),
	//	Line(inner6, Point(outer7.getX() - inner6.getX(), outer7.getY() - inner6.getY(), 0)),
	//	Line(outer7, Point(inner7.getX() - outer7.getX(), inner7.getY() - outer7.getY(), 0)),
	//	Line(inner7, Point(outer8.getX() - inner7.getX(), outer8.getY() - inner7.getY(), 0)),
	//	Line(outer8, Point(inner8.getX() - outer8.getX(), inner8.getY() - outer8.getY(), 0)),
	//	Line(inner8, Point(outer1.getX() - inner8.getX(), outer1.getY() - inner8.getY(), 0))
	//};

	////// Добавляем дополнительные линии, чтобы получить 50 рёбер
	////for (int i = 0; i < 34; i++) {
	////	// Создаем простые диагональные линии внутри звезды
	////	Point p1(i * 0.1, i * 0.1, 0);
	////	Point p2(i * 0.1 + 0.5, i * 0.1 - 0.5, 0);
	////	lines.push_back(Line(p1, Point(p2.getX() - p1.getX(), p2.getY() - p1.getY(), 0)));
	////}

	//// Создаем ребра на основе линий
	//std::vector<Edge> edges;
	//for (size_t i = 0; i < lines.size(); i++) {
	//	edges.push_back(Edge(&lines[i], (double)0, (double)1));
	//}

	//Sphere sphere = Sphere(Point(0, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, -M_PI, M_PI, -M_PI / 2, M_PI / 2);
	//Plane plane = Plane(Point(0, 0, 0), Point(1, 0, 0), Point(0, 1, 0), -3, 3, -3, 3);
	//Face face = Face(&plane, edges);

	//double u = 0.5, v = 0.5;

	//std::cout << face.IsInside(u, v) << std::endl;

	//Cylinder cylinder1 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);
	//Cylinder cylinder2 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);
	//Cylinder cylinder3 = Cylinder(Point(10, 0, 0), Point(1, 0, 0), Point(0, 1, 0), 1, 0, 2 * M_PI, -5, 5);

	//std::vector<int> vec;
	//vec[3] = 2;



	return 0;

}