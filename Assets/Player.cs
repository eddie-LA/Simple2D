using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    // SerializeField means you can access field from Unity editor
    public int lives = 10;
    [SerializeField] TextMeshPro playerBox;
    [SerializeField] private float MovSpeed = 10f;
    [SerializeField] private float JumpMult = 1f;

    [SerializeField] private LayerMask platformLayerMask;
    private Vector3 playerCenterPoint;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private Renderer rend;

    public Vector3 respawnPoint;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        rend = GetComponent<Renderer>();

        playerCenterPoint = rend.bounds.center;
        respawnPoint = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector2.up * JumpMult;

            gameObject.transform.RotateAround(playerCenterPoint, Vector3.left, 60 * Time.deltaTime);
        }
        if (isGrounded()) Movement();
    }

    // Handle collisions with objects - would be better for each object to have its own collision checks but this is simpler for a prototype game
    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidingObject = collision.gameObject;
        string objName = collidingObject.name;

        if(objName.Contains("Goal"))
            Physics2D.IgnoreCollision(collidingObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        if (objName == "EndGame" || objName == "Enemy" || objName.Contains("SwarmEntity"))
        {
            // compare lives and end game if lives < 0
            if (lives > 1) // take life and respawn at (0, 0.18,0)
            {
                lives--;
                gameObject.transform.position = respawnPoint;
                yield return Hurt();

            }
            else // end the game
            {
                lives = 0;
                yield return Hurt();

                rend.enabled = false;
                boxCollider2D.enabled = false;

                DeleteAll();
            }
        }
    }

    public void DeleteAll()
    {
        GameObject.Destroy(GameObject.Find("Swarm"));
    }

    private void Movement()
    {
        Vector3 charScale = transform.localScale;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-MovSpeed, rb.velocity.y);
            charScale.x = 2;
        }
        else
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.velocity = new Vector2(+MovSpeed, rb.velocity.y);
                charScale.x = -2;
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // nothing pressed
            }
        }
        transform.localScale = charScale;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 1f, platformLayerMask);
        return raycastHit2D.collider != null;
    }

    IEnumerator Hurt()
    {
        rend.material.color = Color.red;
        yield return new WaitForSecondsRealtime(0.5f);
        rend.material.color = Color.white;
    }


}
