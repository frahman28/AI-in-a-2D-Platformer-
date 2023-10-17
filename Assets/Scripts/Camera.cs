using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float offset;
    [SerializeField] private float offsetSmoothness;
    private float finalOffset;
    void Update()
    {
        transform.position = new Vector3(player.position.x + finalOffset, player.position.y - finalOffset / 20, transform.position.z);
        finalOffset = Mathf.Lerp(finalOffset, (offset * player.localScale.x), Time.deltaTime * offsetSmoothness);
    }
}
