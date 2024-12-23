#pragma once

class NetPoint {
private:
	double m_x, m_y, m_z;
public:
	NetPoint(double x, double y, double z) :
		m_x(x), m_y(y), m_z(z) {}
	double getX() {
		return m_x;
	}
	double getY() {
		return m_y;
	}
	double getZ() {
		return m_z;
	}
};

class NetEdge {
private:
	NetPoint m_a;
	NetPoint m_b;
public:
	NetEdge(NetPoint a, NetPoint b) :
		m_a(a), m_b(b) {}
	NetPoint getA() {
		return m_a;
	}
	NetPoint getB() {
		return m_b;
	}
};

class Net {
private:
	std::vector<NetEdge> edges;
public:
	Net() {
		edges.clear();
	}
	void addEdge(NetEdge& edge) {
		edges.push_back(edge);
	}
	std::vector<NetEdge> &getEdges() {
		return edges;
	}
};

