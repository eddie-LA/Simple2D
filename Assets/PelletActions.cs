using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletActions : MonoBehaviour
{

    [SerializeField] float selfDestructTimer = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDestructTimer);
        Destroy(gameObject);
    }
}
