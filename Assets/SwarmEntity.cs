using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmEntity : MonoBehaviour
{
    Collider2D entityCollider;
    public Collider2D EntityCollider { get { return entityCollider; } }

    void Start()
    {
        entityCollider = GetComponent<Collider2D>();
    }

    public void Move(Vector2 vel)
    {
        transform.up = vel;                                     // face the velocity vector, so move everytime
        transform.position += (Vector3)vel * Time.deltaTime;   // move towards the velocity vector
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidingObject = collision.gameObject;
        string objTag = collidingObject.tag;

        if (objTag.Contains("Pellet")) Physics2D.IgnoreCollision(collidingObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
    }
}
