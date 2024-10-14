using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Debris")
        {
            //Give the player loot
            DebrisSpawning.Instance.debrisObjects.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
    }
}
