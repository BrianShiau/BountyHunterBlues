using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour {

	public Grid grid;
	public Node start_node;
	public Node end_node;
	private List<Node> path = new List<Node>();
	private List<Node> open = new List<Node>();
	private List<Node> closed = new List<Node>();

	void Start () {
		grid = GameObject.Find("GridOverlay").GetComponent<Grid>();
	}

	public void initialize(){
		set_start_node();
	}

	public int length(){
		return path.Count;
	}

	public void set_start_node(){
		Vector2 position = new Vector2(transform.position.x, transform.position.y);
		GridPoint point = grid.worldToGrid(position);
		start_node = grid.nodes[point.X, point.Y];
	}

	public void set_end_node(Vector2 location){
		Vector2 position = new Vector2(location.x, location.y);
		GridPoint point = grid.worldToGrid(position);
		end_node = grid.nodes[point.X, point.Y];
	}

	public Vector2 get_world_space(int x, int y){
		return grid.gridToWorld(x, y);
	}
}
