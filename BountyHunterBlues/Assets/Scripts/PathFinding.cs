using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathData
{
    public float trueDistance;
    public float moveCost;
    public PathData parent;
    public Node node;

    public PathData(float trueDistance, float moveCost, PathData parent, Node node)
    {
        this.trueDistance = trueDistance;
        this.moveCost = moveCost;
        this.parent = parent;
        this.node = node;
    }
}

public class PathFinding : MonoBehaviour {

	public Grid grid;
	public Node start_node;
	public Node end_node;
	private List<Node> path = new List<Node>();
	private List<PathData> open = new List<PathData>();
	private List<PathData> closed = new List<PathData>();

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
