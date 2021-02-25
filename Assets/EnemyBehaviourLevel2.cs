using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyBehaviourLevel2 : EnemyBehaviour
{
    #region Init_And_Vars
    private Renderer rend;

    // Entity-related vars
    private GameObject stage1goal;
    private GameObject stage2goal;
    [SerializeField] Player player;
    #endregion

    // Start is called before the first frame update
    IEnumerator Start()
    {
        player.lives = PlayerPrefs.GetInt("Lives", lives);
        // Path planning vars
        stage1goal = GameObject.Find("FirePoint");
        stage2goal = GameObject.Find("End");

        // Start stage 1
        yield return new WaitForSecondsRealtime(1f); // wait until player ready?
        yield return Stage1(true, 5);
    }

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

        // EXTRA: Fire pellet forward
        yield return new WaitForSecondsRealtime(0.5f);
        yield return FirePellet("right", 3f, pellet, 1000f);
        yield return new WaitForSecondsRealtime(1.5f);

        // Then forward...
        path = graph.GetPath(transform.position, stage2goal.transform.position);
        yield return Move(path, time / path.Count);

        // Shoot a BIG BONA-FIDE BARRAGE OF BACKSTABBY BULLETS, kek
        yield return new WaitForSecondsRealtime(0.5f);
        yield return FirePellet("left", 3f, pellet, 1000f);
        for (int i = 0; i < 3; i++)                                 // make the "i" bigger for some real hot diggity dog action
        {
            yield return new WaitForSecondsRealtime(0.2f);
            yield return FirePellet("left", 3f, pellet, 1000f);
        }
        
        yield return new WaitForSecondsRealtime(1.5f);

        // Then come back again
        path = graph.GetPath(transform.position, stage1goal.transform.position);
        yield return Move(path, time / path.Count);

        // ~~~~~~ Congratulations, you survived! You are a true champion! :) ~~~~~~~~
        GameObject.Destroy(GameObject.Find("Swarm"));
        gameObject.SetActive(false);

    }
}
