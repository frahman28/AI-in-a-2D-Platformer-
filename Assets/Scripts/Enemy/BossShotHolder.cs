using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShotHolder : MonoBehaviour
{
    [SerializeField] private Transform boss;

    private void Update()
    {
        transform.localScale = boss.localScale;
    }
}
