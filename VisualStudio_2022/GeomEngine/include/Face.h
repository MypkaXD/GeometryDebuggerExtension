
//#include <limits>
//#include <algorithm>

#include <tuple>
#include <vector>
#include <array>

#include "Point.h"
#include "Edge.h"

#ifndef FACE_H
#define FACE_H

extern double eps;

struct BoundingBox {

	Point m_start_point = Point(0,0,0);
	Point m_end_point = Point(0,0,0);

	float m_width = 0;
	float m_height = 0;

	BoundingBox() {}

	BoundingBox(Point start_point, float height, float width) :
		m_start_point(start_point), m_height(height), m_width(width)
	{
		m_end_point = start_point + Point(width, height, 0);
	}

	BoundingBox(Point start_point, Point end_point)
	{
		float x_min = (std::min)(start_point.getX(), end_point.getX());
		float y_min = (std::min)(start_point.getY(), end_point.getY());

		float x_max = (std::max)(start_point.getX(), end_point.getX());
		float y_max = (std::max)(start_point.getY(), end_point.getY());

		m_start_point = Point(x_min, y_min, 0);
		m_end_point = Point(x_max, y_max, 0);

		m_width = std::abs(end_point.getX() - start_point.getX());
		m_height = std::abs(end_point.getY() - start_point.getY());
	}

	bool is_point_inside(const Point& point) {

		if ((point.getX() >= m_start_point.getX() - eps && point.getX() <= m_end_point.getX() + eps) &&
			(point.getY() >= m_start_point.getY() - eps && point.getY() <= m_end_point.getY() + eps))
			return true;
		else
			return false;
	}
};


bool get_clockwise_of_points(Point start, Point end);

Point get_intersection_point(std::tuple<float, float, float> first, std::tuple<float, float, float> second);

bool is_line_cross(std::tuple<float, float, float> first, std::tuple<float, float, float> second);

std::tuple<float, float, float> get_equation_of_line(Point start, Point end);

bool is_point_on_line(std::tuple<float, float, float> line_coefs, Point point);

std::vector<std::vector<std::pair<float, float>>> get_cut_of_figure(BoundingBox box, std::vector<Edge*> edges);


class Plate {

	std::vector<std::vector<Point>> m_points;

public:

	Plate(std::vector<std::vector<Point>> points) :m_points(points) {}
	Plate() {}

	const std::vector<std::vector<Point>>& get_points() {
		return m_points;
	}
};


struct Node {

	std::array<Node*, 4> m_childrens;
	Node* m_parent;

	BoundingBox m_box;
	bool m_is_list = false;
	std::array<Edge*, 2> m_edges;

	Node() {

		m_parent = nullptr;

		for (int i = 0; i < m_childrens.size(); ++i)
			m_childrens[i] = nullptr;
		m_edges = {};
	}

	~Node() {

		for (int i = 0; i < m_childrens.size(); ++i)
			delete m_childrens[i];
	}

	const Node* get_list() const{

		if (this->m_is_list)
			return this;

		for (int i = 0; i < m_childrens.size(); ++i) {
			if (m_childrens[i] != nullptr) {
				if (m_childrens[i]->m_is_list)
					return m_childrens[i];
				else
					return m_childrens[i]->get_list();
			}
		}

		return nullptr;
	}
};

class Tree {
private:
	Node* m_root;
	int m_max_count_of_edge_in_box = 2;
public:


	Tree(BoundingBox& box, std::vector<Edge*> edges) {

		m_root = new Node();
		create_tree(box, edges, m_root);

	}

	Tree(){
		m_root = new Node();
	}

	~Tree() {
		delete m_root;
	}

	const Node* get_root() const {
		return m_root;
	}

	void create_tree(BoundingBox& box, const std::vector<Edge*>& edges, Node* current_node) {

		if (edges.empty()) {
			return;
		}

		std::vector<Edge*> edges_in_box;

		// get equations of bounding box edges
		std::tuple<float, float, float> equation_of_left = get_equation_of_line(box.m_start_point, box.m_start_point + Point(0, box.m_height, 0));
		std::tuple<float, float, float> equation_of_right = get_equation_of_line(box.m_end_point, box.m_start_point + Point(box.m_width, 0, 0));
		std::tuple<float, float, float> equation_of_up = get_equation_of_line(box.m_start_point + Point(0, box.m_height, 0), box.m_end_point);
		std::tuple<float, float, float> equation_of_bottom = get_equation_of_line(box.m_start_point + Point(box.m_width, 0, 0), box.m_start_point);
		std::vector<std::tuple<float, float, float>> equations_of_box = { equation_of_left , equation_of_up, equation_of_right, equation_of_bottom };
		////

		for (int i = 0; i < edges.size(); ++i) {
			
			if (edges[i] == nullptr) 
				continue;
			
			// Получаем точки начала и конца current_edge
			Point p_start = edges[i]->getPoint(edges[i]->getParams().first);
			Point p_end = edges[i]->getPoint(edges[i]->getParams().second);

			std::tuple<float, float, float>equations_of_edge = get_equation_of_line(p_start, p_end);

			BoundingBox current_box_of_edge = BoundingBox(p_start, p_end);

			for (int j = 0; j < equations_of_box.size(); ++j) {
				if (is_line_cross(equations_of_edge, equations_of_box[j])) {
					Point current_intersection_point = get_intersection_point(equations_of_edge, equations_of_box[j]);
					if (box.is_point_inside(current_intersection_point) && (box.is_point_inside(p_start) || box.is_point_inside(p_end) || current_box_of_edge.is_point_inside(current_intersection_point))) // edge пересекает bounding box, если точка пересечения принадлежит исходному BoundingBox И (либо один из концов edge лежит внутри BoundingBox'a или точка пересечения принадлежит BoundingBox'y edge'a)
					{
						edges_in_box.emplace_back(edges[i]);
						break;
					}
				}
			}
			
		}

		if (edges_in_box.size() > m_max_count_of_edge_in_box) {

			const Point half_size(box.m_width / 2, box.m_height / 2, 0);

			BoundingBox child_boxes[4] = {
				{box.m_start_point, box.m_start_point + half_size},
				{box.m_start_point + Point(0, half_size.getY(), 0), box.m_start_point + Point(half_size.getX(), box.m_height, 0)},
				{box.m_start_point + half_size, box.m_end_point},
				{box.m_start_point + Point(half_size.getX(), 0, 0), box.m_start_point + Point(box.m_width, half_size.getY(), 0)}
			};

			for (int i = 0; i < 4; ++i) {
				Node* child = new Node();
				child->m_box = child_boxes[i];
				child->m_parent = current_node;
				current_node->m_childrens[i] = child;
				create_tree(child->m_box, edges_in_box, child);
			}
		}
		else {

			current_node->m_is_list = true;

			if (edges_in_box.size() == 2 && edges_in_box[0] && edges_in_box[1]) {
				Point dir1 = edges_in_box[0]->getPoint(edges_in_box[0]->getParams().second) - edges_in_box[0]->getPoint(edges_in_box[0]->getParams().first);
				Point dir2 = edges_in_box[1]->getPoint(edges_in_box[1]->getParams().second) - edges_in_box[1]->getPoint(edges_in_box[1]->getParams().first);
				Point cross = dir1 & dir2;

				if (cross.getZ() > 0) {
					std::swap(edges_in_box[0], edges_in_box[1]);
				}
			}

			current_node->m_edges = {};

			for (size_t i = 0; i < edges_in_box.size() && i < current_node->m_edges.size(); ++i) {
				current_node->m_edges[i] = edges_in_box[i];
			}
		}
	}

	bool is_find_list(std::array<Node*, 4>& childres) {
		
		for (int i = 0; i < childres.size(); ++i) {
			if (childres[i] != nullptr)
				if (childres[i]->m_is_list)
					return true;
		}

		return false;
	}
};

bool is_bounding_box_inside(const Node* list, std::tuple<float, float, float>& equation_of_horizontal_line);

struct IntersectionPoint {
	Point m_point = Point(0,0,0);
	int m_parent_index_of_box_line = -1;
	int m_type_of_intersection_point = -1;
	bool m_is_visited = false;

	IntersectionPoint() {}

	IntersectionPoint(Point point, int parent_index_of_box_line, int type_of_intersection_point) :
		m_point(point), m_parent_index_of_box_line(parent_index_of_box_line), m_type_of_intersection_point(type_of_intersection_point)
	{

	}
};

#endif // !FACE_H