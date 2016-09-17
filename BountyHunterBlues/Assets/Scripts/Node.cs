using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeConnection
{

    public Node source;
    public Node destination;
    public bool active;

    public NodeConnection(Node source, Node destination, Grid grid)
    {
        this.source = source;
        this.destination = destination;
        active = true;
    }

}

public class Node {

    public bool active; // node becomes active with more than 2 connections

    public GridPoint point;

    public Vector2 worldPosition;

    private Grid grid;
    private List<NodeConnection> connections;

    public Node(int x, int y, Vector2 position, Grid grid)
    {
        worldPosition = position;
        point = new GridPoint(x, y);
        this.grid = grid;
    }

    public void setupConnections()
    {
        float diagonalDist = Mathf.Sqrt(Mathf.Pow(grid.unitsize, 2) + Mathf.Pow(grid.unitsize, 2));
        if(point.X > 0)
        {
           createConnection(grid.nodes[point.X - 1, point.Y]);
            if (point.Y < grid.height - 1)
                createConnection(grid.nodes[point.X - 1, point.Y + 1]);
            if (point.Y > 0)
                createConnection(grid.nodes[point.X - 1, point.Y - 1]);
        }
        if (point.X < grid.width - 1)
        {
            createConnection(grid.nodes[point.X + 1, point.Y]);
            if (point.Y < grid.height - 1)
                createConnection(grid.nodes[point.X + 1, point.Y + 1]);
            if (point.Y > 0)
                createConnection(grid.nodes[point.X + 1, point.Y - 1]);
        }
        if (point.Y < grid.height - 1)
            createConnection(grid.nodes[point.X, point.Y + 1]);
        if (point.Y > 0)
            createConnection(grid.nodes[point.X, point.Y - 1]);

        active = connections.Count > 2;
    }

    public void cullInactiveConnections()
    {
        foreach (NodeConnection connection in connections)
        {
            if (!connection.destination.active)
                connection.active = false;
        }
    }

    private bool createConnection(Node destination)
    {

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, destination.worldPosition - worldPosition, grid.unitsize);
        if ((hit.collider == null || hit.collider.tag == "GameActor") && grid.inBounds(destination.worldPosition))
        {
            connections.Add(new NodeConnection(this, destination, grid));
            return true;
        }

        return false;
    }

}
