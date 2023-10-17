using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthPowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Player>().increasePower();
            gameObject.SetActive(false);
        }
    }
}
