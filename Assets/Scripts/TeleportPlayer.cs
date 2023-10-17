using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private GameObject SpawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.position = SpawnPoint.transform.position;
    }
}
