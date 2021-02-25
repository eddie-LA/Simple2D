using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    List<GameObject> _instances; // a nicer way would be to just get all children
    List<PathNode> nodes;

    // Start is called before the first frame update
    void Start()
    {
        _instances = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
        nodes = new List<PathNode>();
        foreach (GameObject i in _instances)
        {
            nodes.Add(i.GetComponent<PathNode>());
        }
    }

    // Find nodes closest to start and then to dest
    // apply a* to get list between start and dest
    // for each node get position, add to path list and append dest and return list
    public List<Vector2> GetPath(Vector2 start, Vector2 dest)
    {
        // Find closest instance to start and dest
        float min1 = float.PositiveInfinity;
        float min2 = float.PositiveInfinity;
        PathNode startNode = null;
        PathNode destNode = null;

        for (int i = 0; i < _instances.Count; i++)
        {
            float temp1 = Vector2.Distance(start, _instances[i].transform.position);
            float temp2 = Vector2.Distance(dest, _instances[i].transform.position);
            if (min1 > temp1)
            {
                startNode = _instances[i].GetComponent<PathNode>();
                min1 = temp1;
            }
            if (min2 > temp2)
            {
                destNode = _instances[i].GetComponent<PathNode>();
                min2 = temp2;
            }
        }

        // Call A* between startNode and endNode
        List<PathNode> path = AStar(startNode, destNode);
        List<Vector2> vectorPath = new List<Vector2>();

        //vectorPath.Add(start); should be added by A*
        for(int i=0; i<path.Count; i++) 
        {
            vectorPath.Add(path[i].position);
        }
        vectorPath.Add(dest);
        return vectorPath;
    }

    List<PathNode> AStar(PathNode start, PathNode dest)
    {
        List<PathNode> openList = new List<PathNode>();
        openList.Add(start);
        List<PathNode> closedList = new List<PathNode>();

        Hashtable h_value = new Hashtable(); // heuristic, just distance between node and dest, the key is the node and the value is h, g or f
        Hashtable g_value = new Hashtable(); // the cost from source node to a node, for each node
        Hashtable parentNodes = new Hashtable();

        // Populate hashtables with first node
        h_value.Add(start, start.getDistToNode(dest));
        g_value.Add(start, 0f);

        while (openList.Count > 0)
        {
            //Debug.Log(g_value[openList[0]]);
            float hmin = (float)h_value[openList[0]];
            float gmin = (float)g_value[openList[0]];
            PathNode minNode = openList[0];

            // Get node with minimum f
            for (int i = 1; i < openList.Count; i++)
            {
                float h = (float)h_value[openList[i]];
                float g = (float)g_value[openList[i]];

                if (hmin + gmin > h + g)
                {
                    hmin = h; gmin = g;
                    minNode = openList[i];
                }
            }
            // If current node we are looking at is the dest node, break
            if (minNode == dest) break;

            openList.Remove(minNode);
            closedList.Add(minNode);
            //Debug.Log(minNode.neighbours);
            // Iterate through the neighbours of the minNode
            for (int i = 0; i < minNode.neighbours.Count; i++)
            {
                PathNode neighbour = minNode.neighbours[i];
                float cost = (float) g_value[minNode] + minNode.cost[i]; // calculate cost for the neighbour

                if(openList.Contains(neighbour))    // if openList contains neighbour, but new path is cheaper, remove it from the openList
                    if((float) g_value[neighbour] > cost)
                    {
                        openList.Remove(neighbour);
                        g_value.Remove(neighbour);
                        parentNodes.Remove(neighbour);
                    }

                if (closedList.Contains(neighbour)) // same with closedList
                    if ((float)g_value[neighbour] > cost)
                    {
                        closedList.Remove(neighbour);
                        g_value.Remove(neighbour);
                        parentNodes.Remove(neighbour);
                    }

                if(!openList.Contains(neighbour) && !closedList.Contains(neighbour)) // if PathNode isnt in either list, add it to openList and calculate
                {
                    openList.Add(neighbour);
                    g_value.Add(neighbour, cost);
                    parentNodes.Add(neighbour, minNode);

                    if (!h_value.Contains(neighbour)) h_value.Add(neighbour, neighbour.getDistToNode(dest)); 
                }

            }

        }
        // Starting with dest, we look for parent 
        PathNode currNode = dest;
        List<PathNode> path = new List<PathNode>();
        while(currNode != start)
        {
            if(currNode == null)
            {
                Debug.Log("No viable parent found, so no path found");
                return new List<PathNode>();
            }
            path.Insert(0, currNode);
            currNode = (PathNode) parentNodes[currNode];
        }

        path.Insert(0, start);

        return path;
    }
}
