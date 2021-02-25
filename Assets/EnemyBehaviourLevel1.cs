using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyBehaviourLevel1 : EnemyBehaviour
{
    #region Init_And_Vars
    private Renderer rend;
    // Entity-related vars
    private GameObject stage1goal;
    private GameObject stage2goal;

    [SerializeField]Player player; //for lives safekeeping, since no game manager right now

    #endregion
    // Start is called before the first frame update
    IEnumerator Start()
    {
        rend = GetComponent<Renderer>();
        
        // Entity List

        // Path planning vars
        stage1goal = GameObject.Find("End");
        stage2goal = GameObject.Find("Start");

        // Start stage 1
        yield return new WaitForSecondsRealtime(1f); // wait until player ready?
        yield return Stage1(true, 5);

        // Start stage 2
        yield return new WaitForSecondsRealtime(3f);
        yield return Stage2();

        // Cleanup and transition to Scene 2
        ready = true;
        PlayerPrefs.SetInt("Lives", player.lives);
        StopAllCoroutines();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    // ---------------- Stage 1, go around player, if player cannot dodge, they take dmg ----------------
    IEnumerator Stage1(bool debug, float time)
    {
        List<Vector2> path = graph.GetPath(transform.position, stage1goal.transform.position);

        // ---------- DEBUG PATH - SHOW RED LINE ON SCREEN ----------
        if (debug)
        {
            //Debug.Log(path.ToString());
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.Log(path[i].ToString()); // coordinates without the last one
                Debug.DrawLine(path[i], path[i + 1], Color.red, 1000f);
            }
        }
        // -----------------------------------------------------------
        // Move forward
        yield return Move(path, time / path.Count);

        // Then back
        path = graph.GetPath(transform.position, stage2goal.transform.position);
        yield return Move(path, time / path.Count);
    }

    // ---------------- Stage 2, chuck pellets at the player from above for a while ----------------
    IEnumerator Stage2()
    {
        while (lives > 0)
        {
            yield return CreatePelletsAroundPoint(10, gameObject.transform.position, 5f, pellet, 100f); // launch one set of pellets
            
            yield return new WaitForSecondsRealtime(3);

            //Debug.Log(i);
        }
        //Debug.Log("Done");
        yield break;
    }

    public IEnumerator Hurt()
    {
        rend.material.color = Color.red;
        yield return new WaitForSecondsRealtime(0.5f);
        rend.material.color = Color.white;
    }

    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        string collidingObject = collision.gameObject.tag;
        //Debug.Log(collidingObject);
        if (collidingObject == "Pellet")
        {
            // compare lives and end game if lives < 0
            if (lives > 1) // take life and respawn at (0, 0.18,0)
            {
                lives--;
                //enemy.transform.position = respawnPoint;
                StartCoroutine(Hurt());
            }
            else // end the game
            {
                lives = 0;
                yield return Hurt();
                //gameObject.SetActive(false); <-------- DO NOT *E V E R* DO THIS! PLEASE! 
            }
        }
    }
}