using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Animator am;

    [Header("Health")]
    [SerializeField] private float startHealth;
    private float health;
    private bool dead;

    

    private void Awake()
    {
        am = GetComponent<Animator>();
        //sprite = GetComponent<SpriteRenderer>();
        health = startHealth;
        dead = false;
    }

    private void OnEnable()
    {
        am = GetComponent<Animator>();
        health = startHealth;
        dead = false;
    }

    public void TakeDamage(float _damage)
    {
        health = Mathf.Clamp(health - _damage, 0, health);

        if (health > 0)
        {
            am.SetTrigger("hurt");

        }
        else
        {
            if (!dead)
            {
                am.SetTrigger("die");
                
                if (GetComponent<MeleeEnemy>() != null)
                    GetComponent<MeleeEnemy>().enabled = false;

                if (GetComponent<RangedEnemy>() != null) 
                    GetComponent<RangedEnemy>().enabled = false;

                if (GetComponent<ExplosionEnemy>() != null)
                    GetComponent<ExplosionEnemy>().enabled = false;

                dead = true;
            }
        }
    }


    private void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }
}
