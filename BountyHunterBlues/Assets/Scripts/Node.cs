using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NodeConnection
{

    public Node source;
    public Node destination;

    public NodeConnection(Node source, Node destination)
    {
        this.source = source;
        this.destination = destination;
    }

    public void draw()
    {
        Debug.DrawLine(source.worldPosition, destination.worldPosition, Color.yellow);
    }

}

public class Node {

    public bool active; // node becomes active with more than 2 connections
    public bool dynamic; // true if the node can change active states after being initialized (e.g. with the opening of a door)

    public GridPoint point;
    public bool visited;

    public Vector2 worldPosition;

    private Grid grid;
    public List<NodeConnection> connections;

    public Node(int x, int y, Vector2 position, Grid grid)
    {
        connections = new List<NodeConnection>();
        worldPosition = position;
        point = new GridPoint(x, y);
        this.grid = grid;
        visited = false;
    }

    public void setupConnections()
    {
        float diagonalDist = Mathf.Sqrt(Mathf.Pow(grid.unitsize, 2) + Mathf.Pow(grid.unitsize, 2));
        if(point.X > 0)
        {
           createConnection(grid.nodes[point.X - 1, point.Y], grid.unitsize);
            if (point.Y < grid.height - 1)
                createConnection(grid.nodes[point.X - 1, point.Y + 1], diagonalDist);
            if (point.Y > 0)
                createConnection(grid.nodes[point.X - 1, point.Y - 1], diagonalDist);
        }
        if (point.X < grid.width - 1)
        {
            createConnection(grid.nodes[point.X + 1, point.Y], grid.unitsize);
            if (point.Y < grid.height - 1)
                createConnection(grid.nodes[point.X + 1, point.Y + 1], diagonalDist);
            if (point.Y > 0)
                createConnection(grid.nodes[point.X + 1, point.Y - 1], diagonalDist);
        }
        if (point.Y < grid.height - 1)
            createConnection(grid.nodes[point.X, point.Y + 1], grid.unitsize);
        if (point.Y > 0)
            createConnection(grid.nodes[point.X, point.Y - 1], grid.unitsize);

        active = connections.Count > 0;
    }

    public void cullInactiveConnections()
    {
        List<NodeConnection> newConnections = new List<NodeConnection>();
        foreach (NodeConnection connection in connections)
        {
            if (connection.destination.active)
                newConnections.Add(connection);
        }

        connections = newConnections;
    }

    public void draw()
    {
        if (active)
        {
            // simulate a point
            for (int i = 0; i < 360; i += 5)
                Debug.DrawRay(worldPosition, (new Vector2(Mathf.Cos(Mathf.Deg2Rad * i), Mathf.Sin(Mathf.Deg2Rad * i))) * grid.unitsize / 16, Color.blue);
            foreach (NodeConnection connection in connections)
                connection.draw();
        }
    }

    private bool createConnection(Node destination, float rayDist)
    {
        if (grid.inBounds(destination.worldPosition))
        {
            Vector2 dir = destination.worldPosition - worldPosition;
            dir.Normalize();
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, dir, rayDist);
            IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
            bool validConnection = true;
            foreach (RaycastHit2D hit in sortedHits)
            {
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Interactable")
                {
                    validConnection = false;
                    break;
                }
            }

            if (validConnection)
            {
                connections.Add(new NodeConnection(this, destination));
                return true;
            }
        }

        return false;
    }

}
