using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject dragon;
    public float speed;
    private float distance;
    public float enemyHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, dragon.transform.position);
        Vector2 direction = dragon.transform.position - transform.position;

        transform.position = Vector2.MoveTowards(this.transform.position, dragon.transform.position, speed * Time.deltaTime);
    }
}
