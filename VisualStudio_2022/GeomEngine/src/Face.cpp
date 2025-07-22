#include "Face.h"
#include <fstream>
#include <vector>
#include <array>

double eps = 10e-4;

bool get_clockwise_of_points(Point start, Point end) {
	if (((start.getX() - end.getX()) < 0) && ((end.getY() - start.getY()) < 0))
		return true;
	else
		return false;
}


Point get_intersection_point(std::tuple<float, float, float> first, std::tuple<float, float, float> second) {

	float determ = std::get<0>(first) * std::get<1>(second) - std::get<1>(first)*std::get<0>(second);

	float determ_1 = std::get<1>(first) * std::get<2>(second) - std::get<1>(second) * std::get<2>(first);
	float determ_2 = std::get<2>(first) * std::get<0>(second) - std::get<0>(first) * std::get<2>(second);

	return Point(determ_1 / determ, determ_2 / determ, 0);
}

bool is_line_cross(std::tuple<float, float, float> first, std::tuple<float, float, float> second) {

	float determ = std::get<0>(first) * std::get<1>(second) - std::get<1>(first)*std::get<0>(second);

	if (determ == 0)
		return false;
	else
		return true;
}

std::tuple<float, float, float> get_equation_of_line(Point start, Point end) {

	float x0 = start.getX();
	float y0 = start.getY();

	float x1 = end.getX();
	float y1 = end.getY();

	float A = y1 - y0;
	float B = x0 - x1;
	float C = y0*x1 - x0*y1;

	return std::make_tuple(A, B, C);
}

bool is_point_on_line(std::tuple<float, float, float> line_coefs, Point point) {

	if (std::get<0>(line_coefs) * point.getX() + std::get<1>(line_coefs) * point.getY() + std::get<2>(line_coefs) == 0)
		return true;
	else
		return false;

}

bool is_enter_point(Point vec_of_box, Point vec_of_edge) {
	if (((vec_of_box.getX() * vec_of_edge.getY()) - (vec_of_box.getY() * vec_of_edge.getX())) > 0)
		return false;
	else
		return true;
}


bool is_all_point_used(std::vector<bool>& is_used) {
	for (int i = 0; i < is_used.size(); ++i) {
		if (is_used[i] == false)
			return false;
	}
	return true;
}

void dump_points_on_box(std::vector<std::pair<float, float>> points_on_box, std::ofstream& file, std::string name) {
	file << "points: " << name.c_str() << "\n";
	for (int i = 0; i < points_on_box.size(); ++i) {
		file << "(" << points_on_box[i].first << "," << points_on_box[i].second << ",0)" << (i + 1) * 5 << "\n";
	}
}

std::vector<std::vector<std::pair<float, float>>> get_cut_of_figure(BoundingBox box, std::vector<Edge*> edges) {

	if (edges.size() != 1 && edges.size() != 2)
		return{};
	else {

		// get equations of bounding box edges
		std::tuple<float, float, float> equation_of_left = get_equation_of_line(box.m_start_point, box.m_start_point + Point(0, box.m_height, 0));
		std::tuple<float, float, float> equation_of_right = get_equation_of_line(box.m_end_point, box.m_start_point + Point(box.m_width, 0, 0));
		std::tuple<float, float, float> equation_of_up = get_equation_of_line(box.m_start_point + Point(0, box.m_height, 0), box.m_end_point);
		std::tuple<float, float, float> equation_of_bottom = get_equation_of_line(box.m_start_point + Point(box.m_width, 0, 0), box.m_start_point);
		std::vector<std::tuple<float, float, float>> equations_of_box = { equation_of_left , equation_of_up, equation_of_right, equation_of_bottom };
		////

		// get equations of edges
		std::vector<std::tuple<float, float, float>> equations_of_edges(edges.size());
		for (int i = 0; i < edges.size(); ++i) {
			equations_of_edges[i] = get_equation_of_line(edges[i]->getPoint(edges[i]->getParams().first), edges[i]->getPoint(edges[i]->getParams().second));
		}
		////

		int count_of_intersection_points = 0;

		std::vector<std::pair<float, float>> lines_of_box = { std::make_pair(box.m_start_point.getX() , box.m_start_point.getY()), std::make_pair(box.m_start_point.getX(), box.m_start_point.getY() + box.m_height),std::make_pair(box.m_start_point.getX() + box.m_width, box.m_start_point.getY() + box.m_height), std::make_pair(box.m_start_point.getX() + box.m_width,box.m_start_point.getY())};

		std::vector<std::array<IntersectionPoint, 4>> intersection_point_on_boxes(edges.size(), std::array<IntersectionPoint, 4>{});

		for (int i = 0; i < equations_of_edges.size(); ++i) {
			
			BoundingBox current_box_of_edge = BoundingBox(edges[i]->getPoint(edges[i]->getParams().first), edges[i]->getPoint(edges[i]->getParams().second));
			
			for (int j = 0; j < equations_of_box.size(); ++j) {
				if (is_line_cross(equations_of_edges[i], equations_of_box[j])) {

					Point current_intersection_point = get_intersection_point(equations_of_edges[i], equations_of_box[j]);

					if (current_box_of_edge.is_point_inside(current_intersection_point) && box.is_point_inside(current_intersection_point)) {

						// check corner cases

						// 1. ≈сли уже существет точка на предыдущей стороне BoundingBox провер€ем их рассто€ние, если оно меньше Eps, то скипаем еЄ
						IntersectionPoint* intersection_point_on_prev_side_of_box_next = &intersection_point_on_boxes[i][((j + 1) + 4) % 4];
						IntersectionPoint* intersection_point_on_prev_side_of_box_prev = &intersection_point_on_boxes[i][((j - 1) + 4) % 4];
						if (intersection_point_on_prev_side_of_box_next->m_parent_index_of_box_line != -1) {
							float distance = std::sqrt(std::pow(current_intersection_point.getX() - intersection_point_on_prev_side_of_box_next->m_point.getX(), 2) + std::pow(current_intersection_point.getY() - intersection_point_on_prev_side_of_box_next->m_point.getY(), 2));

							if (distance <= eps)
								continue;
						}
						if (intersection_point_on_prev_side_of_box_prev->m_parent_index_of_box_line != -1) {
							float distance = std::sqrt(std::pow(current_intersection_point.getX() - intersection_point_on_prev_side_of_box_prev->m_point.getX(), 2) + std::pow(current_intersection_point.getY() - intersection_point_on_prev_side_of_box_prev->m_point.getY(), 2));

							if (distance <= eps)
								continue;
						}

						Point vec_of_box = Point(lines_of_box[(j + 1) % 4].first, lines_of_box[(j + 1) % 4].second, 0) - Point(lines_of_box[j].first, lines_of_box[j].second, 0);
						Point vec_of_edge = edges[i]->getPoint(edges[i]->getParams().second) - edges[i]->getPoint(edges[i]->getParams().first);
						bool is_current_point_enter = is_enter_point(vec_of_box, vec_of_edge);
						intersection_point_on_boxes[i][j] = IntersectionPoint(current_intersection_point, i, is_current_point_enter);
						++count_of_intersection_points;
					}
				}
			}
		}

		for (int i = 0; i < intersection_point_on_boxes.size(); ++i) {

			int count_of_intersection_points_for_current_edge = 0;
			int index_of_intersection_point = -1;

			for (int j = 0; j < intersection_point_on_boxes[i].size(); ++j) {
				if (intersection_point_on_boxes[i][j].m_parent_index_of_box_line != -1) {
					index_of_intersection_point = j;
					count_of_intersection_points_for_current_edge += 1;
				}
			}

			// 1. ≈сли точки текущего edge'a наход€тс€ за исходным BoungingBox и кол-во точек пересечени€ == 1
			BoundingBox box_with_eps = BoundingBox(box.m_start_point + Point(eps, eps, 0), box.m_end_point - Point(eps, eps, 0));
			if (!box_with_eps.is_point_inside(edges[i]->getPoint(edges[i]->getParams().first)) && !box_with_eps.is_point_inside(edges[i]->getPoint(edges[i]->getParams().second)) && count_of_intersection_points_for_current_edge == 1) {
				intersection_point_on_boxes[i].swap(std::array<IntersectionPoint, 4>{});
				count_of_intersection_points -= count_of_intersection_points_for_current_edge;
			}

			// 2. ≈сли точка пересечени€ находитс€ на углу » один их концов edge'a на углу » кол-во точек пересечени€ == 1
			if (count_of_intersection_points_for_current_edge == 1) {
				std::pair<float, float>* first_point_of_box = &lines_of_box[index_of_intersection_point];
				std::pair<float, float>* second_point_of_box = &lines_of_box[(index_of_intersection_point + 1) % 4];

				float dist_from_current_edge_start_to_first_point_of_box = std::sqrt(std::pow(edges[i]->getPoint(edges[i]->getParams().first).getX() - first_point_of_box->first, 2) + std::pow(edges[i]->getPoint(edges[i]->getParams().first).getY() - first_point_of_box->second, 2));
				float dist_from_current_edge_end_to_first_point_of_box = std::sqrt(std::pow(edges[i]->getPoint(edges[i]->getParams().second).getX() - first_point_of_box->first, 2) + std::pow(edges[i]->getPoint(edges[i]->getParams().second).getY() - first_point_of_box->second, 2));
				float dist_from_current_edge_start_to_second_point_of_box = std::sqrt(std::pow(edges[i]->getPoint(edges[i]->getParams().first).getX() - second_point_of_box->first, 2) + std::pow(edges[i]->getPoint(edges[i]->getParams().first).getY() - second_point_of_box->second, 2));
				float dist_from_current_edge_end_to_second_point_of_box = std::sqrt(std::pow(edges[i]->getPoint(edges[i]->getParams().second).getX() - second_point_of_box->first, 2) + std::pow(edges[i]->getPoint(edges[i]->getParams().second).getY() - second_point_of_box->second, 2));

				if (dist_from_current_edge_start_to_first_point_of_box <= eps || dist_from_current_edge_end_to_first_point_of_box <= eps || dist_from_current_edge_start_to_second_point_of_box <= eps || dist_from_current_edge_end_to_second_point_of_box <= eps) {
					float dist_from_current_intersection_point_to_first_point_of_box = std::sqrt(std::pow(intersection_point_on_boxes[i][index_of_intersection_point].m_point.getX() - first_point_of_box->first, 2) + std::pow(intersection_point_on_boxes[i][index_of_intersection_point].m_point.getY() - first_point_of_box->second, 2));
					float dist_from_current_intersection_point_to_second_point_of_box = std::sqrt(std::pow(intersection_point_on_boxes[i][index_of_intersection_point].m_point.getX() - second_point_of_box->first, 2) + std::pow(intersection_point_on_boxes[i][index_of_intersection_point].m_point.getY() - second_point_of_box->second, 2));

					if (dist_from_current_intersection_point_to_first_point_of_box <= eps || dist_from_current_intersection_point_to_second_point_of_box <= eps) {
						intersection_point_on_boxes[i].swap(std::array<IntersectionPoint, 4>{});
						count_of_intersection_points -= count_of_intersection_points_for_current_edge;
					}
				}

			}
		}


		//if (edges.size() == 2 && (count_of_intersection_points == 2 || count_of_intersection_points == 4)) {
		if (edges.size() == 2) {

			float dist_between_first_end_and_second_start = std::pow(edges[0]->getPoint(edges[0]->getParams().second).getX() - edges[1]->getPoint(edges[1]->getParams().first).getX(), 2) + std::pow(edges[0]->getPoint(edges[0]->getParams().second).getY() - edges[1]->getPoint(edges[1]->getParams().first).getY(), 2);
			float dist_between_first_start_and_second_end = std::pow(edges[0]->getPoint(edges[0]->getParams().first).getX() - edges[1]->getPoint(edges[1]->getParams().second).getX(), 2) + std::pow(edges[0]->getPoint(edges[0]->getParams().first).getY() - edges[1]->getPoint(edges[1]->getParams().second).getY(), 2);

			Point* point_of_box = nullptr;

			if (dist_between_first_end_and_second_start < eps * eps) {
				if (box.is_point_inside(edges[0]->getPoint(edges[0]->getParams().second))) {
					point_of_box = &edges[0]->getPoint(edges[0]->getParams().second);
				}
			}
			else if (dist_between_first_start_and_second_end < eps * eps) {
				if (box.is_point_inside(edges[0]->getPoint(edges[0]->getParams().first))) {
					point_of_box = &edges[0]->getPoint(edges[0]->getParams().first);
				}
			}
			
			if (point_of_box != nullptr) {
				for (int i = 0; i < intersection_point_on_boxes.size(); ++i) {
					for (int j = 0; j < intersection_point_on_boxes[i].size(); ++j) {

						if (intersection_point_on_boxes[i][j].m_parent_index_of_box_line == -1)
							continue;

						float current_dist = std::pow(point_of_box->getX() - intersection_point_on_boxes[i][j].m_point.getX(), 2) + std::pow(point_of_box->getY() - intersection_point_on_boxes[i][j].m_point.getY(),2);

						if (current_dist < eps * eps) {
							intersection_point_on_boxes[i][j] = IntersectionPoint();
							count_of_intersection_points -= 1;
						}
					}
				}
			}


			/*for (int i = 0; i < 4; ++i) {
				IntersectionPoint* first_point = &intersection_point_on_boxes[0][i];
				IntersectionPoint* second_point = &intersection_point_on_boxes[1][i];

				if (first_point->m_parent_index_of_box_line == -1 || second_point->m_parent_index_of_box_line == -1)
					continue;
				else {
					float dist_between_points = std::pow(first_point->m_point.getX() - second_point->m_point.getX(), 2) + std::pow(first_point->m_point.getY() - second_point->m_point.getY(), 2);
					float dist_between_edges_first = std::pow(edges[0]->getPoint(edges[0]->getParams().second).getX() - edges[1]->getPoint(edges[1]->getParams().first).getX(), 2) + std::pow(edges[0]->getPoint(edges[0]->getParams().second).getY() - edges[1]->getPoint(edges[1]->getParams().first).getY(), 2);
					float dist_between_edges_second = std::pow(edges[0]->getPoint(edges[0]->getParams().first).getX() - edges[1]->getPoint(edges[1]->getParams().second).getX(), 2) + std::pow(edges[0]->getPoint(edges[0]->getParams().first).getY() - edges[1]->getPoint(edges[1]->getParams().second).getY(), 2);
					if (dist_between_points < eps * eps || dist_between_edges_first < eps * eps || dist_between_edges_second < eps * eps) {
						intersection_point_on_boxes[0][i] = IntersectionPoint();
						intersection_point_on_boxes[1][i] = IntersectionPoint();
						count_of_intersection_points -= 2;
					}
				}
			}
			*/


		}

	
		std::vector<IntersectionPoint> points_of_box;

		for (int i = 0; i < equations_of_box.size(); ++i) {
			points_of_box.emplace_back(Point(lines_of_box[i].first, lines_of_box[i].second, 0), -1, -1);

			if (edges.size() == 2) {

				std::vector<IntersectionPoint*> poins_on_current_side;

				for (int j = 0; j < equations_of_edges.size(); ++j) {
					if (intersection_point_on_boxes[j][i].m_parent_index_of_box_line == -1)
						continue;
					poins_on_current_side.emplace_back(&intersection_point_on_boxes[j][i]);
				}

				if (poins_on_current_side.size() == 2) {

					Point vec_of_box = Point(lines_of_box[(i + 1) % 4].first, lines_of_box[(i + 1) % 4].second, 0) - Point(lines_of_box[i].first, lines_of_box[i].second, 0);
					if (dot((poins_on_current_side[0]->m_point - poins_on_current_side[1]->m_point), vec_of_box) > 0) {
						points_of_box.emplace_back(*poins_on_current_side[1]);
						points_of_box.emplace_back(*poins_on_current_side[0]);
					}
					else {
						points_of_box.emplace_back(*poins_on_current_side[0]);
						points_of_box.emplace_back(*poins_on_current_side[1]);
					}
				}
				else if (poins_on_current_side.size() == 1) {
					points_of_box.emplace_back(*poins_on_current_side[0]);
				}
			}
			else {
				for (int j = 0; j < equations_of_edges.size(); ++j) {
					if (intersection_point_on_boxes[j][i].m_parent_index_of_box_line == -1)
						continue;
					points_of_box.emplace_back(intersection_point_on_boxes[j][i]);
				}
			}

		}

		int start_index = -1;

		for (int i = 0; i < points_of_box.size(); ++i) {
			if (points_of_box[i].m_type_of_intersection_point == 0) {
				start_index = i;
				break;
			}
		}

		if (start_index == -1)
			return {};

		std::vector<std::pair<float, float>> result_points;

		if (count_of_intersection_points == 2 || count_of_intersection_points == 4) {

			if (edges.size() == 2) {
				if (box.is_point_inside(edges[0]->getPoint(edges[0]->getParams().second))) {
					result_points.push_back(std::make_pair(edges[0]->getPoint(edges[0]->getParams().second).getX(),
						edges[0]->getPoint(edges[0]->getParams().second).getY()));
				}
				if (box.is_point_inside(edges[0]->getPoint(edges[0]->getParams().first))) {
					result_points.push_back(std::make_pair(edges[0]->getPoint(edges[0]->getParams().first).getX(),
						edges[0]->getPoint(edges[0]->getParams().first).getY()));
				}
			}

			std::vector<std::vector<std::pair<float, float>>> result;

			int current_index = start_index;

			while (true) {

				if (points_of_box[current_index].m_is_visited) {
				
					bool is_all_visited = true;

					for (int i = 0; i < points_of_box.size(); ++i) {
						if (!points_of_box[i].m_is_visited && points_of_box[i].m_type_of_intersection_point != -1) {
							is_all_visited = false;
							if (points_of_box[i].m_type_of_intersection_point == 0) {
								current_index = i;
								break;
							}
						}
					}

					if (is_all_visited) {
						result.push_back(result_points);
						break;
					}
					else {
						result.push_back(result_points);
						result_points.clear();
						current_index = start_index;
					}

				}

				if (points_of_box[current_index].m_type_of_intersection_point != -1) {

					if (points_of_box[current_index].m_type_of_intersection_point == 0) {
						result_points.push_back({ points_of_box[current_index].m_point.getX(), points_of_box[current_index].m_point.getY() });
						points_of_box[current_index].m_is_visited = true;
					}
					else {
						result_points.push_back({ points_of_box[current_index].m_point.getX(), points_of_box[current_index].m_point.getY() });
						points_of_box[current_index].m_is_visited = true;

						int temp_start_index = current_index;

						int temp_index = (current_index + 1) % points_of_box.size();
						bool is_meet_attachment_enter_point = false;
						while (temp_start_index != temp_index) {

							if (points_of_box[temp_index].m_type_of_intersection_point != -1 && points_of_box[temp_index].m_type_of_intersection_point == 0 && points_of_box[current_index].m_parent_index_of_box_line == points_of_box[temp_index].m_parent_index_of_box_line)
								break;
							if (points_of_box[temp_index].m_type_of_intersection_point != -1 && points_of_box[temp_index].m_type_of_intersection_point == 0) {
								is_meet_attachment_enter_point = true;
								start_index = temp_index;
							}

							temp_index = (temp_index + 1) % points_of_box.size();
						}

						if (temp_index == temp_start_index)
							continue;

						if (is_meet_attachment_enter_point)
							current_index = temp_index;
						else
							current_index = (temp_index - 1) % points_of_box.size();
					}
				}
				else {
					result_points.push_back({ points_of_box[current_index].m_point.getX(), points_of_box[current_index].m_point.getY() });
					points_of_box[current_index].m_is_visited = true;
				}

				current_index = (current_index + 1) % points_of_box.size();
			}

			return result;

		}
		else {
			return {};
		}

		return {};

	}
}

int condition_that_box_inside(const Node* list, std::tuple<float, float, float>& equation_of_horizontal_line) {
	
	if (list == nullptr)
		return -1;
	
	for (int i = 0; i < list->m_edges.size(); ++i) {
		if (list->m_edges[i] == nullptr)
			continue;
		std::tuple<float, float, float> equation_of_current_edge = get_equation_of_line(list->m_edges[i]->getPoint(list->m_edges[i]->getParams().first), list->m_edges[i]->getPoint(list->m_edges[i]->getParams().second));

		if (is_line_cross(equation_of_horizontal_line, equation_of_current_edge)) {

			BoundingBox current_box_of_edge = BoundingBox(list->m_edges[i]->getPoint(list->m_edges[i]->getParams().first), list->m_edges[i]->getPoint(list->m_edges[i]->getParams().second));

			Point current_intersection_point = get_intersection_point(equation_of_horizontal_line, equation_of_current_edge);

			if (current_intersection_point.getX() <= list->m_box.m_start_point.getX())
				continue;

			if (current_box_of_edge.is_point_inside(current_intersection_point)) {

				Point dir1 = list->m_box.m_start_point + Point(list->m_box.m_width, list->m_box.m_height / 2, 0) - list->m_box.m_start_point - Point(list->m_box.m_width / 2, list->m_box.m_height / 2, 0);
				Point dir2 = list->m_edges[i]->getPoint(list->m_edges[i]->getParams().second) - list->m_edges[i]->getPoint(list->m_edges[i]->getParams().first);
				Point cross = dir1 & dir2;

				return cross.getZ() < 0;
			}
		}
	}

	return -1;
}

int func(const Node* list, std::tuple<float, float, float>& equation_of_horizontal_line) {

	if (list == nullptr)
		return false;

	int result = condition_that_box_inside(list, equation_of_horizontal_line);

	if (result == 0 || result == 1)
		return result;

	for (int i = 0; i < list->m_childrens.size(); ++i) {
		if (list->m_childrens[i] == nullptr)
			continue;
		int current_result = func(list->m_childrens[i], equation_of_horizontal_line);

		if (current_result == 0 || current_result == 1)
			return current_result;
	}

	return -1;
}

bool is_bounding_box_inside(const Node* list, std::tuple<float, float, float>& equation_of_horizontal_line) {

	if (list == nullptr)
		return false;

	int result = condition_that_box_inside(list, equation_of_horizontal_line);

	if (result == 0 || result == 1)
		return result;

	Node* parrent = list->m_parent;
	if (parrent == nullptr)
		return false;

	int index_of_list = -1;
	for (int i = 0; i < parrent->m_childrens.size(); ++i) {
		if (parrent->m_childrens[i] == list) {
			index_of_list = i;
			break;
		}
	}

	if (index_of_list == -1)
		return false;

	if (index_of_list == 0)
		return is_bounding_box_inside(parrent->m_childrens[3]->get_list(), equation_of_horizontal_line);
	else if (index_of_list == 1)
		return is_bounding_box_inside(parrent->m_childrens[2]->get_list(), equation_of_horizontal_line);
	else {

		Node* parrent_of_parrent = nullptr;
		int index_of_list_parent = -1;

		while (true) {
			parrent_of_parrent = parrent->m_parent;
			if (parrent_of_parrent == nullptr)
				return false;
			else {
				for (int i = 0; i < parrent_of_parrent->m_childrens.size(); ++i) {
					if (parrent_of_parrent->m_childrens[i] == parrent) {
						index_of_list_parent = i;
						break;
					}
				}
			}

			if (index_of_list_parent != 0 && index_of_list_parent != 1)
				parrent = parrent_of_parrent;
			else {
				result = func(parrent_of_parrent->m_childrens[index_of_list_parent == 0 ? 3 : 2], equation_of_horizontal_line);

				if (result == 1 || result == 0)
					return result;
				else {
					parrent = parrent_of_parrent;
					continue;
				}
			}
		}
	}
}