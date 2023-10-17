using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilityPowerUp : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Player p;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(p.GetComponent<Player>().Invisibility(duration, gameObject));
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
