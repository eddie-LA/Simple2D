using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Swarm : MonoBehaviour
{
    // Settable constants
    [SerializeField] int count = 50;
    public float speedMult = 5f;
    public float maxSpeed = 3f;
    public float nbRadius = 1.5f;
    public float avoidanceRadiusMult = 0.4f;
    //const float density = 1f;

    // Initialization of swarm components
    public SwarmEntity entity;
    List<SwarmEntity> entities = new List<SwarmEntity>();
    public SwarmBehaviour behaviour;

    // Cache squares of our constants to make it faster
    float sq_maxSpeed;
    float sq_nbRadius;
    float sq_avoidanceRadius;
    public float Sq_AvoidanceRadius { get {return sq_avoidanceRadius;} }

    public GameObject X, Y;     // for random point to spawn

    // Start is called before the first frame update
    void Start()
    {
        sq_maxSpeed = Mathf.Pow(maxSpeed, 2f);
        sq_nbRadius = Mathf.Pow(nbRadius, 2f);
        sq_avoidanceRadius = sq_nbRadius * Mathf.Pow(avoidanceRadiusMult, 2f);

        for (int i = 0; i < count; i++)
        {
            // Instantiate the entity in a random circle around the origin point of the swarm
            // Set the entity's parent to the transform, since we are using transforms instead of game objects
            SwarmEntity newEntity = Instantiate<SwarmEntity>
                (entity, 
                    RandomPointInBox(transform.position, 
                    transform.position - X.transform.position, 
                    transform.position - Y.transform.position), // * count * density, - spawn inside the box 
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform                                                       
                );
            newEntity.name = "SwarmEntity " + i;
            entities.Add(newEntity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(SwarmEntity entity in entities)
        {
            List<Transform> environment = GetNearbyObjects(entity);

            //Debug.Log(environment.Count);
            //Debug.Log(entity);
            //Debug.Log(this.ToString());

            Vector2 move = behaviour.Move(entity, this, environment);
            move *= speedMult;
            if (move.sqrMagnitude > sq_maxSpeed) move = move.normalized * maxSpeed;
            
            entity.Move(move);
        }
    }

    private static Vector2 RandomPointInBox(Vector2 center, Vector2 sizeX, Vector2 sizeY)
    {

        Vector2 centerTemp = new Vector2((Random.value - 0.5f) * sizeX.x, (Random.value - 0.5f) * sizeY.y);
        Debug.DrawRay(center, centerTemp, Color.red, 100f);
        return center + new Vector2((Random.value - 0.5f) * sizeX.x, (Random.value - 0.5f) * sizeY.y);
    }

    List<Transform> GetNearbyObjects(SwarmEntity entity)
    {
        Collider2D[] envColliders = Physics2D.OverlapCircleAll(entity.transform.position, nbRadius);
        List<Transform> environment = new List<Transform>();

        foreach(Collider2D col in envColliders)
        {
            if(col != entity.EntityCollider)
            {
                environment.Add(col.transform);
            }
        }
        return environment;

    }
}
