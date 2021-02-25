using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public TextMeshPro playerBox;
    public TextMeshPro enemyBox;
    public Player player;
    public EnemyBehaviour enemy;

    // Start is called before the first frame update
    void Start()
    {
        playerBox.enabled = true;
        playerBox.enabled = true;
        StartCoroutine(UpdateUI());
    }
    IEnumerator UpdateUI() // more efficient than Update()
    {
        for (; ; )
        {
            UpdateText();
            yield return new WaitForSeconds(.5f);
        }
    }
    private void UpdateText()
    {
        if (player.lives < 1)
        {
            playerBox.SetText("Game over.");
            enemyBox.SetText("Game over.");
        }
        else
        {
            playerBox.SetText("Lives left: " + player.lives.ToString());
            enemyBox.SetText("Enemy lives: " + enemy.lives.ToString());
        }
        
    }
}

