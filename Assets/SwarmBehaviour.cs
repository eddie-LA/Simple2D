using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Swarm Behaviour")]
public class SwarmBehaviour : ScriptableObject
{
    // Init and vars
    public float[] weights = new float[4];

    public float cohesionConst = 1f; // amount of weights is equal to amount of behaviours
    public float alignmentConst = 1f;
    public float avoidanceConst = 1f;
    public float pullConst = 0.1f;
    public Vector2 target;

    //GameObject player = GameObject.FindGameObjectWithTag("player");

    //int mask;

    // -------------------------------- Swarm Behaviour Methods --------------------------------
    private Vector2 CalculateDirectional(SwarmEntity entity, Swarm swarm, List<Transform> environment) // directional component
    {
        return (target - (Vector2) entity.transform.position).normalized; // nudge the birds to player slightly
    }
    private Vector2 CalculateCohesion(SwarmEntity entity, Swarm swarm, List<Transform> environment) // cohesion logic component
    {
        if (environment.Count == 0) return Vector2.zero; // Since no neighbours exist, then no adjustment is necessary

        // Find mid point of swarm and try to move there - mid point is averaged sum of all points 
        Vector2 move = Vector2.zero;
        //LayerMask mask = LayerMask.GetMask("Skybox");

        foreach (Transform obj in environment)
        {
            move += (Vector2)obj.position;            
        }
        move /= environment.Count;

        // Start moving to that direction
        move -= (Vector2)entity.transform.position;
        return move;
    }

    private Vector2 CalculateAlignment(SwarmEntity entity, Swarm swarm, List<Transform> environment) // alignment logic component
    {
        if (environment.Count == 0) return entity.transform.up; // Since no neighbours exist, then fly on autopilot

        // Move forward with the swarm - mid point is averaged sum of all points 
        Vector2 move = Vector2.zero;
        foreach (Transform obj in environment)
        {
           move += (Vector2)obj.transform.up;
        }
        return move /= environment.Count;
    }

    private  Vector2 CalculateAvoidance(SwarmEntity entity, Swarm swarm, List<Transform> environment) // avoidance logic component
    {
        if (environment.Count == 0) return Vector2.zero; // Since no neighbours/enemies exist, then no adjustment is necessary

        // Find mid point of swarm and try to move there - mid point is averaged sum of all points 
        int entitiesToAvoid = 0;
        Vector2 move = Vector2.zero;
        foreach (Transform obj in environment)
        {
            if (Vector2.SqrMagnitude(obj.position - entity.transform.position) < (swarm.Sq_AvoidanceRadius))
            {
                entitiesToAvoid++;
                move += (Vector2)(entity.transform.position - obj.position);
            }

        }
        if (entitiesToAvoid > 0) move /= entitiesToAvoid;   // if no entities this returns Vector2.zero !!!

        return move;
    }
    // -------------------------------- Main Method --------------------------------
    public Vector2 Move(SwarmEntity entity, Swarm swarm, List<Transform> environment)
    {
        Vector2 move = Vector2.zero;

        //set weights just in case
        weights[0] = cohesionConst;
        weights[1] = alignmentConst;
        weights[2] = avoidanceConst; 
        //weights[3] = pullConst;

        // Cohesion, Alignment and Avoidance // this violates speed constraints!
        //move = CalculateCohesion(entity, swarm, environment) * cohesionConst + 
        //   CalculateAlignment(entity, swarm, environment) * alignmentConst + 
        //    CalculateAlignment(entity, swarm, environment) * avoidanceConst;


        Vector2 tempMove = CalculateCohesion(entity, swarm, environment) * weights[0];
        if(tempMove != Vector2.zero)
            if(tempMove.sqrMagnitude > Mathf.Pow(weights[0], 2))        // make sure this is not too big
            {
                tempMove.Normalize();
                tempMove *= weights[0];
            }
        move += tempMove;

        tempMove = CalculateAlignment(entity, swarm, environment) * weights[1];
        if (tempMove != Vector2.zero)
            if (tempMove.sqrMagnitude > Mathf.Pow(weights[1], 2))        // make sure this is not too big
            {
                tempMove.Normalize();
                tempMove *= weights[1];
            }
        move += tempMove;

        tempMove = CalculateAvoidance(entity, swarm, environment) * weights[2];
        if (tempMove != Vector2.zero)
            if (tempMove.sqrMagnitude > Mathf.Pow(weights[2], 2))        // make sure this is not too big
            {
                tempMove.Normalize();
                tempMove *= weights[2];
            }
        move += tempMove;

        tempMove = CalculateDirectional(entity, swarm, environment) * pullConst;
        if (tempMove != Vector2.zero)
            if (tempMove.sqrMagnitude > Mathf.Pow(pullConst, 2))        // make sure this is not too big
            {
                tempMove.Normalize();
                tempMove *= pullConst;
            }
        move += tempMove;

        return move;
    }

}
