#include "Vector.h"

#pragma once



class Curve {
private:

	Vector m_normal; // нормаль к поверхности в которой лежит кривая
	Vector m_xComp;
	Vector m_yComp;

public:

	Vector virtual getPoint(const double& param) = 0; // получить точку кривой в UV-пространстве

	void setBasis() {
		
		m_normal.normalize();

		m_xComp = m_normal & Vector(1, 0, 0);

		if (m_xComp.length() < 0.3) {
			m_xComp = m_normal & Vector(0, 1, 0);
		}
		m_yComp = m_normal & m_xComp;
		m_xComp.normalize();
		m_yComp.normalize();
	}

	Vector getNormal() const {
		return m_normal;
	}
	Vector getXComp() const {
		return m_xComp;
	}
	Vector getYComp() const {
		return m_yComp;
	}
	void setNormal(const Vector& normal){
		m_normal = normal;
	}
	void setXComp(const Vector& xComp) {
		m_xComp = xComp;
	}
	void setYComp(const Vector& yComp) {
		m_yComp = yComp;
	}
};