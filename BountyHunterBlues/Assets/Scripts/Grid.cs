using UnityEngine;
using System.Collections;


public class GridPoint
{
    public int X;
    public int Y;
    public GridPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class Grid : MonoBehaviour {


    public float unitsize;
    
    public Node[,] nodes;
    public int width;
    public int height;

    private float worldWidth;
    private float worldHeight;
    
	// Use this for initialization
	void Start () {

        worldWidth = transform.localScale.x;
        worldHeight = transform.localScale.y;
        width = Mathf.RoundToInt(worldWidth / unitsize);
        height = Mathf.RoundToInt(worldHeight / unitsize);

        nodes = new Node[width, height];

        // set up initial grid
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                nodes[x, y] = new Node(x, y, gridToWorld(x, y), this);

        // make connections between unblocked nodes and mark nodes that have less than 2 connections as inactive
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                nodes[x, y].setupConnections();

        // remove connections to and from inactive nodes
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                nodes[x, y].cullInactiveConnections();

    }

    void Update()
    {
        // DEBUG MODE
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                nodes[x, y].draw();
    }
	
	public Vector2 gridToWorld(int x, int y)
    {
        return new Vector2(unitsize * x + transform.position.x, unitsize * y + transform.position.y);
        
    }

    public GridPoint worldToGrid(Vector2 worldPoint)
    {
        if (inBounds(worldPoint))
        {
            Vector2 gridSpacePoint = transform.InverseTransformPoint(worldPoint);
            gridSpacePoint.x *= worldWidth;
            gridSpacePoint.y *= worldHeight;
            Vector2 unitNormalizedPoint = gridSpacePoint / unitsize;
            GridPoint result = new GridPoint(Mathf.RoundToInt(unitNormalizedPoint.x), Mathf.RoundToInt(unitNormalizedPoint.y));
            return result;
        }
        return null;
    }

    public bool inBounds(Vector2 point)
    {
        return point.x >= transform.position.x && point.x <= transform.position.x + worldWidth
            && point.y >= transform.position.y && point.y <= transform.position.y + worldHeight;
    }
}
