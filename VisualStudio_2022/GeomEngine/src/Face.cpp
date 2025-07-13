#include "Face.h"
#include <fstream>

float eps = 1e-6;

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

	float determ = std::get<0>(first) * std::get<1>(second) - std::get<1>(first)*std::get<0>(first);

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
			if (edges[i] != nullptr)
				equations_of_edges[i] = get_equation_of_line(edges[i]->getPoint(edges[i]->getParams().first), edges[i]->getPoint(edges[i]->getParams().second));
		}
		////

		int count_of_intersection_points = 0;

		Point intersection_point;
		BoundingBox box_of_edge;

		// we have all intersection points in clockwise direction
		// and now we must to combine this points with points of box

		std::vector<std::pair<float, float>> points_on_box;
		std::vector<bool> is_visited;
		std::vector<bool> is_enter;
		std::vector<bool> is_exit;
		std::vector<bool> is_intersection;
		std::vector<int> attachments;
	
		int start_index = 0;

		std::vector<std::pair<float, float>> lines_of_box = { std::make_pair(box.m_start_point.getX() , box.m_start_point.getY()), std::make_pair(box.m_start_point.getX(), box.m_start_point.getY() + box.m_height),std::make_pair(box.m_start_point.getX() + box.m_width, box.m_start_point.getY() + box.m_height), std::make_pair(box.m_start_point.getX() + box.m_width,box.m_start_point.getY())};

		for (int i = 0; i < equations_of_box.size(); ++i) {

			bool is_exist_other_intersection_point_on_this_line = false;

			points_on_box.push_back(lines_of_box[i]);
			is_intersection.push_back(false);
			is_visited.push_back(false);
			is_exit.push_back(false);
			is_enter.push_back(false);
			attachments.push_back(-1);

			for (int j = 0; j < equations_of_edges.size(); ++j) {

				if (is_line_cross(equations_of_edges[j], equations_of_box[i])) { // if lines is cross (main determ != 0)

					intersection_point = get_intersection_point(equations_of_edges[j], equations_of_box[i]); // get Point of intersection
					box_of_edge = BoundingBox(edges[j]->getPoint(edges[j]->getParams().first), edges[j]->getPoint(edges[j]->getParams().second)); // get bounding box of edge

					if (box_of_edge.is_point_inside(intersection_point) && box.is_point_inside(intersection_point)) { // if intersection point in bounding box of edge

						points_on_box.push_back(std::make_pair(intersection_point.getX(), intersection_point.getY()));

						Point vec_of_box = Point(lines_of_box[(i + 1) % 4].first, lines_of_box[(i + 1) % 4].second, 0) - Point(lines_of_box[i].first, lines_of_box[i].second, 0);
						Point vec_of_edge = edges[j]->getPoint(edges[j]->getParams().second) - edges[j]->getPoint(edges[j]->getParams().first);

						if (is_enter_point(vec_of_box, vec_of_edge)) {
							is_enter.push_back(true);
							is_exit.push_back(false);
						}
						else {
							is_enter.push_back(false);
							is_exit.push_back(true);
							//start_index = points_on_box.size() - 1;
						}

						attachments.push_back(j);

						if (is_exist_other_intersection_point_on_this_line) {

							Point prev_point = Point(points_on_box[points_on_box.size() - 2].first, points_on_box[points_on_box.size() - 2].second, 0);
							Point vec_of_box = Point(lines_of_box[(i + 1) % 4].first, lines_of_box[(i + 1) % 4].second, 0) - Point(lines_of_box[i].first, lines_of_box[i].second, 0);

							if (dot((intersection_point - prev_point), vec_of_box) < 0) {
								std::swap(points_on_box[points_on_box.size() - 2], points_on_box[points_on_box.size() - 1]);
								is_enter.swap(is_enter[is_enter.size() - 2], is_enter[is_enter.size() - 1]);
								is_exit.swap(is_exit[is_exit.size() - 2], is_exit[is_exit.size() - 1]);
								std::swap(attachments[attachments.size() - 2], attachments[attachments.size() - 1]);
								//start_index = points_on_box.size() - 2;
							}

						}
						
						count_of_intersection_points += 1;
						is_intersection.push_back(true);
						is_visited.push_back(false);

						is_exist_other_intersection_point_on_this_line = true;

					}
				}

			}
		}

		for (int i = 0; i < points_on_box.size(); ++i) {
			if (is_exit[i]) {
				start_index = i;
				break;
			}
		}

		std::vector<std::pair<float, float>> result_points;

		/*if (count_of_intersection_points != 2 && count_of_intersection_points != 4)
			return{};*/

		if (box.is_point_inside(edges[0]->getPoint(edges[0]->getParams().second))) {
			result_points.push_back(std::make_pair(edges[0]->getPoint(edges[0]->getParams().second).getX(),
				edges[0]->getPoint(edges[0]->getParams().second).getY()));
		}

		std::vector<std::vector<std::pair<float, float>>> result;

		int current_index = start_index;

		while (true) {

			if (is_visited[current_index]) {
				
				bool is_all_visited = true;

				for (int i = 0; i < is_visited.size(); ++i) {
					if (!is_visited[i] && is_intersection[i]) {
						is_all_visited = false;
						if (is_exit[i]) {
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

			if (is_intersection[current_index]) {

				if (is_exit[current_index]) {
					result_points.push_back(points_on_box[current_index]);
					is_visited[current_index] = true;
				}
				else {
					result_points.push_back(points_on_box[current_index]);
					is_visited[current_index] = true;

					int temp_start_index = current_index;

					int temp_index = (current_index + 1) % points_on_box.size();
					bool is_meet_attachment_enter_point = false;
					while (temp_start_index != temp_index) {

						if (is_intersection[temp_index] && is_exit[temp_index] && attachments[current_index] == attachments[temp_index])
							break;
						if (is_intersection[temp_index] && is_exit[temp_index]) {
							is_meet_attachment_enter_point = true;
							start_index = temp_index;
						}

						temp_index = (temp_index + 1) % points_on_box.size();
					}

					if (temp_index == temp_start_index)
						continue;

					if (is_meet_attachment_enter_point)
						current_index = temp_index;
					else
						current_index = (temp_index - 1) % points_on_box.size();
				}
			}
			else {
				result_points.push_back(points_on_box[current_index]);
				is_visited[current_index] = true;
			}

			current_index = (current_index + 1) % points_on_box.size();
		}

		return result;
	}
}