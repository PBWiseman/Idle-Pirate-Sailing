using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisPickup : MonoBehaviour
{

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
