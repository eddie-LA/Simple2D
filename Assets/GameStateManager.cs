using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] EnemyBehaviour level1;
    [SerializeField] EnemyBehaviour level2;
    // Start is called before the first frame update

    void Start()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name.Equals("scene1"))
        {
            StartCoroutine(Check(level1));
            Debug.Log("SCENE CHECK");
        }
            

    }

    IEnumerator Check(EnemyBehaviour obj)
    {
        for (; ; )
        {
            if (!CheckIfReady(obj))
            {
                yield return new WaitForSeconds(.2f);
                Debug.Log("Scene 1");
            }
                
            else
            {
                Debug.Log("Loading scene 2...");
               // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    bool CheckIfReady(EnemyBehaviour obj)
    {
        return obj.ready;
    }
}
