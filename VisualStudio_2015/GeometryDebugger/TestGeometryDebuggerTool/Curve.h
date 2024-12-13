#pragma once

#include "Point.h"

class Curve {
private:
	Point m_normal;
	Point m_xComp;
	Point m_yComp;
public:
	Point virtual getPoint(const double& param) = 0;
	void setBasis() {

		m_normal.normalize();
		m_xComp = m_normal & Point(1, 0, 0);
		if (m_xComp.length() < 0.3) {
			m_xComp = m_normal & Point(0, 1, 0);
		}
		m_yComp = m_normal & m_xComp;



		m_xComp.normalize();
		m_yComp.normalize();
	}
	Point getNormal() const {
		return m_normal;
	}
	Point getXComp() const {
		return m_xComp;
	}
	Point getYComp() const {
		return m_yComp;
	}
	void setNormal(const Point& normal) {
		m_normal = normal;
	}
	void setXComp(const Point& xComp) {
		m_xComp = xComp;
	}
	void setYComp(const Point& yComp) {
		m_yComp = yComp;
	}
};