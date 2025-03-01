#ifndef FACE_H
#define FACE_H

class Face {
private:

	Surface* m_surface;
	std::vector<Edge> m_edges;

public:

	Face(Surface* surface, std::vector<Edge> edges) :
		m_surface(surface), m_edges(edges)
	{}

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
			std::pair<double, double> params = m_edges[i].getParams();

			Point A = m_edges[i].getPoint(params.first);
			Point B = m_edges[i].getPoint(params.second);
			
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
