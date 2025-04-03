#include <limits>
#include <algorithm>

#ifndef FACE_H
#define FACE_H

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

#endif // !FACE_H
