using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBoss : MonoBehaviour
{
    [SerializeField] private GameObject spawn;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera CameraBoss;
    [SerializeField] private GameObject BossCharacter;

    private void Start()
    {
        MainCamera.enabled = true;
        CameraBoss.enabled = false;
        BossCharacter.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D colliderObject)
    {
        colliderObject.gameObject.transform.position = spawn.transform.position;
        MainCamera.enabled = false;
        CameraBoss.enabled = true;
        BossCharacter.SetActive(true);
        colliderObject.gameObject.GetComponent<Health>().bossPhase = true;
    }
}
