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
        width = (int)(worldWidth / unitsize);
        height = (int)(worldHeight / unitsize);

        nodes = new Node[width, height];

        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                nodes[x, y] = new Node(x, y, gridToWorld(x, y), this);

        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                nodes[x, y].setupConnections();

        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++ y)
                nodes[x, y].cullInactiveConnections();

    }
	
	public Vector2 gridToWorld(int x, int y)
    {
        Vector2 result = new Vector2();

        return result;
        
    }

    public GridPoint worldToGrid(Vector2 worldPoint)
    {
        return new GridPoint(0, 0);
    }

    public bool inBounds(Vector2 point)
    {
        return point.x >= transform.position.x && point.x <= transform.position.x + worldWidth
            && point.y >= transform.position.y && point.y <= transform.position.y + worldHeight;
    }
}
