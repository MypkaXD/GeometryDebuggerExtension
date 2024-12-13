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

	double length() const {
		return std::sqrt(m_x * m_x + m_y * m_y + m_z * m_z);
	}

	Vector& getNormalize() {
		
		double lenghtOfVec = length();

		if (lenghtOfVec > 1) {
			double x = m_x / lenghtOfVec;
			double y = m_y / lenghtOfVec;
			double z = m_z / lenghtOfVec;

			Vector normalized = Vector(x, y, z);

			return normalized;
		}
		else
			return *this;
	}

	void normalize() {

		double lenghtOfVec = length();

		if (lenghtOfVec > 1) {
			m_x /= lenghtOfVec;
			m_y /= lenghtOfVec;
			m_z /= lenghtOfVec;
		}
		else
			return;
	}

	void transformToOtherSK(const Vector& xComp, const Vector& yComp, const Vector& zComp) {

		double x = xComp.getX() * m_x + yComp.getX() * m_y + zComp.getX() * m_z;
		double y = xComp.getY() * m_x + yComp.getY() * m_y + zComp.getY() * m_z;
		double z = xComp.getZ() * m_x + yComp.getZ() * m_y + zComp.getZ() * m_z;

		m_x = x;
		m_y = y;
		m_z = z;

	}
};

Vector operator+(const Vector& left, const Vector& right) {
	return Vector(left.getX() + right.getX(), left.getY() + right.getY(), left.getZ() + right.getZ());
}

Vector operator-(const Vector& left, const Vector& right) {
	return Vector(left.getX() - right.getX(), left.getY() - right.getY(), left.getZ() - right.getZ());
}

Vector operator*(const Vector& vector, const double& value) {
	return Vector(vector.getX() * value, vector.getY() * value, vector.getZ() * value);
}

double operator*(const Vector& left, const Vector& right) {
	return (left.getX() * right.getX() + left.getY() * right.getY() + left.getZ() * right.getZ());
}

Vector operator&(const Vector& left, const Vector& right) {

	double x = left.getY() * right.getZ() - left.getZ() * right.getY();
	double y = left.getZ() * right.getX() - left.getX() * right.getZ();
	double z = left.getX() * right.getY() - left.getY() * right.getX();

	return Vector(x, y, z);
}

std::string serialize(Vector * value, std::string typeName, std::string variableName, int r, int g, int b) {

	std::string data = "";

	data += "points: " + typeName + "_" + variableName + '\n' + "(" + std::to_string(value->getX()) + "," + std::to_string(value->getY()) + "," + std::to_string(value->getZ()) + ")";
	
	return data;
}
