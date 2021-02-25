using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyBehaviour : MonoBehaviour
{
    #region Init_And_Vars
    // Entity-related vars
    public int lives = 3;
    public bool ready = false;
    public TextMeshPro enemyBox;

    public GameObject pellet;

    // Path-planning algo vars
    public Graph graph;

    #endregion

    //  A method that takes a list of points and moves between them smoothly
    public IEnumerator Move(List<Vector2> path, float time)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(path[i], path[i+1], (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        yield break;
    }

    public IEnumerator CreatePelletsAroundPoint(int num, Vector2 point, float radius, GameObject createdObjects, float force)
    {
        for (int i = 0; i < num; i++)
        {
            var radians = 2 * Mathf.PI / num * i;                                                       // Distance around the circle

            float y = Mathf.Sin(radians);                                                               // Get the vector direction
            float x = Mathf.Cos(radians);

            Vector2 spawnDir = new Vector2(x, y);                                                       // Get the spawn direction
            Vector2 spawnPos = point + spawnDir * radius;                                               // and position

            GameObject newObj = Instantiate<GameObject>(createdObjects, spawnPos, Quaternion.identity); // Instaniate pellet - can reuse for stage 3 until here

            /*newObj.transform.LookAt(gameObject.transform.position);                                   // Rotate the pellets - rewrite another day, its 5:20am goddamit 

            Vector3 eulerAngles = newObj.transform.rotation.eulerAngles;                                // Fix pellet rotation! LookAt() screws it up!
            if(eulerAngles.x > 0)
            {
                if (eulerAngles.y > 0)
                    newObj.transform.rotation = Quaternion.Euler(0f, 0f, -180 - (eulerAngles.x - eulerAngles.y));
                else newObj.transform.rotation = Quaternion.Euler(0f, 0f, - (eulerAngles.x - eulerAngles.y)+180);
            }
            else {
                if (eulerAngles.y > 0)
                    newObj.transform.rotation = Quaternion.Euler(0f, 0f, 180 - (eulerAngles.x - eulerAngles.y));
                else newObj.transform.rotation = Quaternion.Euler(0f, 0f, - (eulerAngles.x - eulerAngles.y));

            }*/
            

            newObj.transform.Translate(new Vector3(0, newObj.transform.localScale.y, 0));               // Adjust height

            // Fling pellets away
            newObj.GetComponent<Rigidbody2D>().AddForce(
                (newObj.transform.position - gameObject.transform.position) * force,
                ForceMode2D.Force);
            //Debug.Log(player.GetComponent<Rigidbody2D>().isKinematic);
        }
        yield return null; // can return a value maybe to chuck X pellets instead of counting collisions 
    }

    public IEnumerator FirePellet(string direction, float radius, GameObject createdObjects, float force)
    {
        GameObject newObj = Instantiate(createdObjects) as GameObject;
        switch (direction)
        {
            case "left":
                newObj.transform.localPosition = (Vector2)transform.position + Vector2.left * radius;
                newObj.GetComponent<Rigidbody2D>().AddForce((Vector2.left) * force, ForceMode2D.Force);
                break;

            case "right":
                newObj.transform.localPosition = (Vector2)transform.position + Vector2.right * radius;
                newObj.GetComponent<Rigidbody2D>().AddForce((Vector2.right) * force, ForceMode2D.Force);
                break;

            case "up":
                newObj.transform.localPosition = (Vector2)transform.position + Vector2.up * radius;
                newObj.GetComponent<Rigidbody2D>().AddForce((Vector2.up) * force, ForceMode2D.Force);
                break;

            case "down":
                newObj.transform.localPosition = (Vector2)transform.position + Vector2.down * radius;
                newObj.GetComponent<Rigidbody2D>().AddForce((Vector2.down) * force, ForceMode2D.Force);
                break;
        }

        // TODO: Add facing direction
        //Debug.Log(player.GetComponent<Rigidbody2D>().isKinematic);

        yield return null; // can return a value maybe to chuck X pellets instead of counting collisions 
    }


    /*    public IEnumerator Hurt()
        {
            rend.material.color = Color.red;
            yield return new WaitForSecondsRealtime(0.5f);
            rend.material.color = Color.white;
        }
    */
}