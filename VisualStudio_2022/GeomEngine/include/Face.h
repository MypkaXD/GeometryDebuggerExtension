
//#include <limits>
//#include <algorithm>

#include <tuple>
#include <vector>
#include <array>

#include "Point.h"
#include "Edge.h"

#ifndef FACE_H
#define FACE_H

extern float eps;

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

std::vector<std::pair<float, float>> get_cut_of_plate(BoundingBox box, std::vector<Point> intersections_points);

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

	size_t m_counter = 0;;
	std::array<Node*, 4> m_childrens;
	Node* m_parent;
	bool m_is_list = false;

	BoundingBox m_box;
	std::array<Edge*, 2> m_edges;

	Node() {
		m_parent = nullptr;

		for (int i = 0; i < m_childrens.size(); ++i)
			m_childrens[i] = nullptr;
	}

	~Node() {
		for (int i = 0; i < m_childrens.size(); ++i)
			delete m_childrens[i];
	}

	Node* get_list() {

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


public:
	Node* m_root;

	int m_max_count_of_edge_in_box = 2;

	Tree(BoundingBox& box, std::vector<Edge*> edges) {

		m_root = new Node();
		create_tree(box, edges, m_root);

	}


	~Tree() {
		delete m_root;
	}

	void create_tree(BoundingBox& box, const std::vector<Edge*>& edges, Node* current_node) {

		if (current_node == nullptr || edges.empty()) {
			return;
		}

		std::vector<Edge*> edges_in_box;
		const int sample_points = 10;

		for (int i = 0; i < edges.size(); ++i) {
			if (edges[i] == nullptr) 
				continue;

			float t_start = edges[i]->getParams().first;
			float t_end = edges[i]->getParams().second;
			float step = (t_end - t_start) / sample_points;

			for (int j = 0; j <= sample_points; ++j) {
				
				Point current_point = edges[i]->getPoint(t_start + j * step);
				if (box.is_point_inside(current_point)) {
					edges_in_box.push_back(edges[i]);
					break;
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
				current_node->m_childrens[i] = new Node();
				current_node->m_childrens[i]->m_box = child_boxes[i];
				current_node->m_childrens[i]->m_parent = current_node;
				create_tree(child_boxes[i], edges_in_box, current_node->m_childrens[i]);
			}
		}
		else {

			current_node->m_box = box;
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

/*
class BoundingBox {
private:

Point m_startPoint;
Point m_endPoint;

double m_width;
double m_height;

public:

BoundingBox() {

}

BoundingBox(Point startPoint, Point endPoint)
{
m_startPoint = Point(startPoint.getX(), startPoint.getY(), 0);
m_endPoint = Point(endPoint.getX(), endPoint.getY(), 0);

m_width = m_endPoint.getX() - m_startPoint.getX();
m_height = m_endPoint.getY() - m_startPoint.getY();
}

BoundingBox(std::vector<Edge*> edges)
{
double minX = (std::numeric_limits<double>::max)();
double maxX = (std::numeric_limits<double>::min)();
double minY = (std::numeric_limits<double>::max)();
double maxY = (std::numeric_limits<double>::min)();

for (int i = 0; i < edges.size(); ++i) {
double currentX1 = edges[i]->getPoint(edges[i]->getParams().first).getX();
double currentY1 = edges[i]->getPoint(edges[i]->getParams().first).getY();

double currentX2 = edges[i]->getPoint(edges[i]->getParams().second).getX();
double currentY2 = edges[i]->getPoint(edges[i]->getParams().second).getY();

if ((std::min)(currentX1, currentX2) < minX)
minX = (std::min)(currentX1, currentX2);
if ((std::max)(currentX1, currentX2) > maxX)
maxX = (std::max)(currentX1, currentX2);

if ((std::min)(currentY1, currentY2) < minY)
minY = (std::min)(currentY1, currentY2);
if ((std::max)(currentY1, currentY2) > maxY)
maxY = (std::max)(currentY1, currentY2);
}

m_startPoint = Point(minX, minY, 0);
m_endPoint = Point(maxX, maxY, 0);

m_width = m_endPoint.getX() - m_startPoint.getX();
m_height = m_endPoint.getY() - m_startPoint.getY();
}

bool isEdgeInsideInBox(Edge* edge) {

double xStart = edge->getPoint(edge->getParams().first).getX();
double yStart = edge->getPoint(edge->getParams().first).getY();

double xEnd = edge->getPoint(edge->getParams().second).getX();
double yEnd = edge->getPoint(edge->getParams().second).getY();

if ((xStart >= m_startPoint.getX() && xStart <= m_endPoint.getX() &&
yStart >= m_startPoint.getY() && yStart <= m_endPoint.getY()) ||
(xEnd >= m_startPoint.getX() && xEnd <= m_endPoint.getX() &&
yEnd >= m_startPoint.getY() && yEnd <= m_endPoint.getY())) {
return true;
}
else
return false;
}

Point* getStartPoint() {
return &m_startPoint;
}
Point* getEndPoint() {
return &m_endPoint;
}

double getWidth() {
return m_width;
}
double getHeight() {
return m_height;
}

std::vector<Edge*> getEdgesInBoundigBox(std::vector<Edge*>& edges) {

std::vector<Edge*> result;

for (int i = 0; i < edges.size(); ++i) {
if (isEdgeInsideInBox((edges)[i]))
result.push_back((edges)[i]);
}

return result;
}

};


class Node {
private:
BoundingBox m_box;
std::vector<Edge*> m_edges;
std::vector<Node> m_childs;

public:
Node(std::vector<Edge*> edges) {
m_box = BoundingBox(edges);
m_childs.reserve(4);
}
Node(double uMin, double uMax, double vMin, double vMax, std::vector<Edge*> edges):
m_edges(edges)
{

m_childs.reserve(4);

double minXEdges = (std::numeric_limits<double>::max)();
double maxXEdges = (std::numeric_limits<double>::min)();
double minYEdges = (std::numeric_limits<double>::max)();
double maxYEdges = (std::numeric_limits<double>::min)();

for (int i = 0; i < edges.size(); ++i) {
double currentX1 = edges[i]->getPoint(edges[i]->getParams().first).getX();
double currentY1 = edges[i]->getPoint(edges[i]->getParams().first).getY();

double currentX2 = edges[i]->getPoint(edges[i]->getParams().second).getX();
double currentY2 = edges[i]->getPoint(edges[i]->getParams().second).getY();

if ((std::min)(currentX1, currentX2) < minXEdges)
minXEdges = (std::min)(currentX1, currentX2);
if ((std::max)(currentX1, currentX2) > maxXEdges)
maxXEdges = (std::max)(currentX1, currentX2);

if ((std::min)(currentY1, currentY2) < minYEdges)
minYEdges = (std::min)(currentY1, currentY2);
if ((std::max)(currentY1, currentY2) > maxYEdges)
maxYEdges = (std::max)(currentY1, currentY2);
}

double minXTotal = (std::max)(minXEdges, uMin);
double maxXTotal = (std::min)(maxXEdges, uMax);

double minYTotal = (std::max)(minYEdges, vMin);
double maxYTotal = (std::min)(maxYEdges, vMax);

m_box = BoundingBox(Point(minXTotal, minYTotal, 0), Point(maxXTotal, maxYTotal, 0));
}
Node() {

}

Node(BoundingBox box, std::vector<Edge*> edges) :
m_box(box), m_edges(edges) {

}

void removeEdges(std::vector<Edge*> edges) {
std::vector<Edge*> temp_edges;

for (int i = 0; i < m_edges.size(); ++i) {

bool isFind = false;

for (int j = 0; j < edges.size(); ++j) {
if (edges[j] == m_edges[i]) {
isFind = true;
break;
}
}

if (!isFind)
temp_edges.push_back(m_edges[i]);
}

m_edges.swap(temp_edges);
}

static bool isNormalNode(Node& node) {

if (node.m_edges.size() > 4)
return false;
else
return true;
}

BoundingBox* getBox() {
return &m_box;
}

std::vector<Edge*>& getEdges() {
return m_edges;
}

std::vector<Node>& getChildren() {
return m_childs;
}
};

class QuadTree {
private:
Node m_root;
public:
QuadTree(double u_min, double u_max, double v_min, double v_max, std::vector<Edge*>* edges)
{
m_root = Node(u_min, u_max, v_min, v_max, *edges);

if (Node::isNormalNode(m_root)) {
return;
}
else {
createCorrectTree(m_root);
}
}

QuadTree() {

}

Node& getRoot() {
return m_root;
}

void createCorrectTree(Node& node) {
if (Node::isNormalNode(node)) {
return;
}
else {
BoundingBox topLeft = BoundingBox(Point(node.getBox()->getStartPoint()->getX(),
node.getBox()->getStartPoint()->getY() + node.getBox()->getHeight() / 2, 0),
Point(node.getBox()->getStartPoint()->getX() + node.getBox()->getWidth() / 2,
node.getBox()->getStartPoint()->getY() + node.getBox()->getHeight(), 0));
BoundingBox topRight = BoundingBox(Point(node.getBox()->getStartPoint()->getX() +
node.getBox()->getWidth() / 2,
node.getBox()->getStartPoint()->getY() + node.getBox()->getHeight() / 2, 0),
Point(node.getBox()->getStartPoint()->getX() + node.getBox()->getWidth(),
node.getBox()->getStartPoint()->getY() + node.getBox()->getHeight(), 0));
BoundingBox downLeft = BoundingBox(Point(node.getBox()->getStartPoint()->getX(),
node.getBox()->getStartPoint()->getY(), 0),
Point(node.getBox()->getStartPoint()->getX() + node.getBox()->getWidth() / 2,
node.getBox()->getStartPoint()->getY() + node.getBox()->getHeight() / 2, 0));
BoundingBox downRight = BoundingBox(Point(node.getBox()->getStartPoint()->getX() +
node.getBox()->getWidth() / 2,
node.getBox()->getStartPoint()->getY(), 0),
Point(node.getBox()->getStartPoint()->getX() + node.getBox()->getWidth(),
node.getBox()->getStartPoint()->getY() + node.getBox()->getHeight() / 2, 0));

Node topLeftNode(topLeft, topLeft.getEdgesInBoundigBox(node.getEdges()));
Node topRightNode(topRight, topRight.getEdgesInBoundigBox(node.getEdges()));
Node downLeftNode(downLeft, downLeft.getEdgesInBoundigBox(node.getEdges()));
Node downRightNode(downRight, downRight.getEdgesInBoundigBox(node.getEdges()));

if (topLeftNode.getEdges().size() != 0)
node.removeEdges(topLeftNode.getEdges());
if (topRightNode.getEdges().size() != 0)
node.removeEdges(topRightNode.getEdges());
if (downLeftNode.getEdges().size() != 0)
node.removeEdges(downLeftNode.getEdges());
if (downRightNode.getEdges().size() != 0)
node.removeEdges(downRightNode.getEdges());

if (!Node::isNormalNode(topLeftNode))
createCorrectTree(topLeftNode);
if (node.getChildren().size() <= 4)
node.getChildren().push_back(topLeftNode);

if (!Node::isNormalNode(topRightNode))
createCorrectTree(topRightNode);
if (node.getChildren().size() <= 4)
node.getChildren().push_back(topRightNode);

if (!Node::isNormalNode(downLeftNode))
createCorrectTree(downLeftNode);
if (node.getChildren().size() <= 4)
node.getChildren().push_back(downLeftNode);

if (!Node::isNormalNode(downRightNode))
createCorrectTree(downRightNode);
if (node.getChildren().size() <= 4)
node.getChildren().push_back(downRightNode);


}
}
};

class Face {
private:

Surface* m_surface;
std::vector<Edge*> m_edges;
QuadTree tree;

public:

Face(Surface* surface, std::vector<Edge> edges) :
m_surface(surface)
{
for (int i = 0; i < edges.size(); ++i) {
m_edges.push_back(&edges[i]);
}

tree = QuadTree(m_surface->getUMin(), m_surface->getUMax(), m_surface->getVMin(), m_surface->getVMax(), &m_edges);

std::cout << "END" << std::endl;
}


Surface* getSurface() {
return m_surface;
}

double orientation(Point a, Point b, Point c) {

double x1 = a.getX() - b.getX();
double y1 = a.getY() - b.getY();

double x2 = c.getX() - b.getX();
double y2 = c.getY() - b.getY();

double val = x1*y2 - y1*x2;

if (val > 0)
return 1;
else if (val < 0)
return -1;

return 0;
}

bool onSegment(Point a, Point b, Point c) {
return ((myMin(a.getX(), b.getX()) <= c.getX()) && (c.getX() <= myMax(a.getX(), b.getX()))) &&
((myMin(a.getY(), b.getY()) <= c.getY()) && (c.getY() <= myMax(a.getY(), b.getY())));
}

double myMin(const double& left, const double& right) {
return (left > right ? right : left);
}
double myMax(const double& left, const double& right) {
return (left > right ? left : right);
}

bool IsInside(double u, double v) {

int count = 0;

for (int i = 0; i < m_edges.size(); ++i) {
std::pair<double, double> params = m_edges[i]->getParams();

Point A = m_edges[i]->getPoint(params.first);
Point B = m_edges[i]->getPoint(params.second);

Point C = Point(u, v, 0);
Point D = Point(m_surface->getUMax(), v, 0);

int O1 = orientation(A, B, C);
int O2 = orientation(A, B, D);
int O3 = orientation(C, D, A);
int O4 = orientation(C, D, B);

// Общий случай
if (O1 != O2 && O3 != O4)
++count;

}

return (count % 2 == 0 ? false : true);
}
};
*/


#endif // !FACE_H
