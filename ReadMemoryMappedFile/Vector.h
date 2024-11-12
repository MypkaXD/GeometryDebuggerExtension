#pragma once

class Vector {
private:

	double m_x, m_y, m_z;

public:
	
	Vector() {
		m_x = 0;
		m_y = 0;
		m_z = 0;
	}

	Vector(double x, double y, double z) :
		m_x(x), m_y(y), m_z(z) {
	}

	double getX() const {
		return m_x;
	}
	double getY() const {
		return m_y;
	}
	double getZ() const {
		return m_z;
	}

	void setX(const double& x) {
		m_x = x;
	}
	void setY(const double& y) {
		m_y = y;
	}
	void setZ(const double& z) {
		m_z = z;
	}

	double lenght() const {
		return std::sqrt(m_x * m_x + m_y * m_y + m_z * m_z);
	}

	bool isNormalized(double epsilon = 1e-6) const {
		return (std::abs(lenght() - 1.0) < epsilon);
	}

	void normalize() {
		
		double lenghtOfVec = lenght();
		
		m_x /= lenghtOfVec;
		m_y /= lenghtOfVec;
		m_z /= lenghtOfVec;
	}
};

std::string serialize(Vector * value, std::string typeName, std::string variableName, int r, int g, int b) {

	std::string data = "";

	data += "points: " + typeName + "_" + variableName + '\n' + "(" + std::to_string(value->getX()) + "," + std::to_string(value->getY()) + "," + std::to_string(value->getZ()) + ")";
	
	return data;
}
