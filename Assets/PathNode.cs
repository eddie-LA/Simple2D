using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    #region Init_And_Vars
    static List<PathNode> _instances = new List<PathNode>();
    List<PathNode> _neighbours;
    List<float> _cost;
    public static float radius = 12;            // this is fine :)

    public static List<PathNode> instances { get { return _instances; } }
    public List<PathNode> neighbours { get { return _neighbours; } }
    public List<float> cost { get { return _cost; } }


    Transform _transform;                       // Cache the transform, good practise for bigger graphs etc.

    public Vector2 position { 
        get { 
            return _transform.position; 
        } 
    }
    #endregion
    #region UNITY_Methods
    void Awake() {_transform = transform; _instances.Add(this); }      // Cache reference of position (Transform) for faster access

    void Start()                        
    {
        _neighbours = new List<PathNode>();
        _cost = new List<float>();
        //yield return new WaitForSeconds(0.5f);  // Has to wait since at Start() the objects are just initialized, so no others exist...

        //Debug.Log(_neighbours.ToString());
        for (int i = 0; i < _instances.Count; i++)  // Populate the graph
        {
            if (gameObject == _instances[i]) {continue;}
            float dist = Vector2.Distance(gameObject.transform.position, _instances[i].transform.position);
            
            if (dist < radius)                  // Find all neighbours within a certain radius
            {
                
                //Debug.Log();
                _neighbours.Add(_instances[i]);
                _cost.Add(dist);
            }
        }
    }

    void OnDisable() {
        _instances.Remove(this);
    } // Remove instance from list, add way to remove neighbours OnDisable()
    #endregion
    #region PUBLIC_Methods

    public float getDistToNode(PathNode node)
    {
        return Vector2.Distance(transform.position, node.transform.position);
    }
    public static PathNode FindClosest(Vector2 point)
    {
        PathNode closest = null;                // Initialize nearest pathnode
        {
            Vector2 nearestPos = Vector3.zero;
            float nearestDist = float.PositiveInfinity;
            {
                int instanceCount = _instances.Count;
                int i = 0;
                if (instanceCount > 0)
                {
                    closest = _instances[0];
                    nearestPos = closest._transform.position;
                    nearestDist = Vector2.Distance(point, nearestPos);
                    i = 1;
                }
                for (; i < instanceCount; i++)
                {
                    PathNode next = _instances[i];
                    Vector2 nextPos = next._transform.position;
                    float dist = Vector2.Distance(point, nextPos);
                    if (dist < nearestDist)
                    {
                        closest = next;
                        nearestPos = next._transform.position;
                        nearestDist = dist;
                    }
                }
            }
        }
        return closest;
    }
}
#endregion
