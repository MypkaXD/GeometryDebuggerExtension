#include "Curve.h"
#include "Circle.h"
#include "Line.h"
#include "Vector.h"
#include "CustomCurve.h"
#include <iostream>

#pragma once

// ��������� Edge ��� ��������� �����, ����������� ��� �������
class Edge {
public:
	Curve* curve; // ������ �� ������� ������� ���� �����
	double t1, t2;
public:
	Edge(double t1, double t2, Curve* curve) :
		t1(t1), t2(t2), curve(curve)
	{
	}
	// �������������� getPoint, ��������� ���������� �������
	Point getPoint(const double& param) {
		return curve->getPoint(param);
	}
};

Edge CreateArcEdge(Circle* circle, double t1, double t2);

Edge CreateStraightEdge(Line* line, double t1, double t2);

Edge CreateCustomEdge(CustomCurve* line, double t1, double t2);