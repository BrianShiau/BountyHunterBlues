using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathData
{
    public float trueDistance;
    public float moveCost;
    public float total_cost;
    public PathData parent;
    public Node node;

    public PathData(float trueDistance, float moveCost, Node node, PathData parent = null)
    {
        this.trueDistance = trueDistance;
        this.moveCost = moveCost;
        this.total_cost = trueDistance + moveCost;
        this.parent = parent;
        this.node = node;
    }
}

public class PathFinding : MonoBehaviour {

	private Grid grid;
	public Node start_node;
	public Node end_node;
	private List<Node> path = new List<Node>();
	private List<Node> path_check = new List<Node>();
	private List<PathData> open = new List<PathData>();
	private List<PathData> closed = new List<PathData>();
	private int path_threshold;
	private bool audio_detected;

	void Start () {
		grid = GameObject.Find("GridOverlay").GetComponent<Grid>();
		path_threshold = 100;
		audio_detected = false;
	}

	public void set_threshold(int threshold){
		path_threshold = threshold;
	}

	public void set_audio_detected(bool value){
		audio_detected = value;
	}

	public void initialize(Vector3 start_position, Vector3 end_position){
		path.Clear();
		open.Clear();
		closed.Clear();
		start_node = null;
		end_node = null;
		set_start_node(start_position);
		set_end_node(end_position);
	}

	public Node get_node(int index){
		return path[index];
	}

	public void calc_path(){
		bool found = false;
		float distance = Vector2.Distance(start_node.worldPosition, end_node.worldPosition);
		PathData data = new PathData(distance, 0, start_node);
		open.Add(data);

		while(open.Count > 0){
			PathData current = get_lowest_cost();

			if(current.node.point.X == end_node.point.X && current.node.point.Y == end_node.point.Y){
				reconstruct_path(current);
				break;
			}

			if(check_path_length(current) && audio_detected){
				path.Clear();
				break;
			}

			open.Remove(current);
			closed.Add(current);

			for(int i = 0; i < current.node.connections.Count; i++){
				found = false;
				Node neighbor = current.node.connections[i].destination;
				distance = Vector2.Distance(neighbor.worldPosition, end_node.worldPosition);
				data = new PathData(distance, current.moveCost + 1, neighbor, current);
				
				for(int j = 0; j < closed.Count; j++){
					if(neighbor.point.X == closed[j].node.point.X && neighbor.point.Y == closed[j].node.point.Y){
						found = true;
						break;
					}
				}
				if(found){
					continue;
				}
				
				float total_move_cost = current.moveCost + data.moveCost;
				
				if(open.Count == 0){
					open.Add(data);
					found = true;
				}
				else{
					for(int j = 0; j < open.Count; j++){
						if(neighbor.point.X != open[j].node.point.X || neighbor.point.Y != open[j].node.point.Y){
							open.Add(data);
							found = true;
							break;
						}
					}
				}
				if(!found){
					if(total_move_cost >= data.moveCost){
						continue;
					}
				}

				data.parent = current;
				data.moveCost = total_move_cost;
				data.total_cost = data.moveCost + data.trueDistance;
			}
		}
	}

	public PathData get_lowest_cost(){
		int index = 0;
		float value = float.PositiveInfinity;
		for(int i = 0; i < open.Count; i++){
			if((open[i].total_cost) < value){
				value = open[i].total_cost;
				index = i;
			}
		}

		return open[index];
	}

	public bool check_path_length(PathData current){
		List<PathData> temp = new List<PathData>();
		while(current.parent != null){
			temp.Add(current);
			current = current.parent;
		}
		if(temp.Count > path_threshold){
			return true;
		}
		return false;
	}

	public void reconstruct_path(PathData current){
		List<PathData> temp = new List<PathData>();
		while(current.parent != null){
			temp.Add(current);
			current = current.parent;
		}
		for(int i = (temp.Count - 1); i >= 0; i--){
			path.Add(temp[i].node);
		}
	}

	public void clear(){
		path.Clear();
	}

	public int length(){
		return path.Count;
	}

	public void set_start_node(Vector3 start_position){
		GridPoint point = grid.worldToGrid(new Vector2(start_position.x, start_position.y));
		start_node = grid.nodes[point.X, point.Y];
	}

	public void set_end_node(Vector3 location){
		GridPoint point = grid.worldToGrid(new Vector2(location.x, location.y));
		end_node = grid.nodes[point.X, point.Y];
	}
}
