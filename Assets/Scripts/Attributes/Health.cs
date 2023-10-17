using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private Animator am;
    private SpriteRenderer sp;
    private Rigidbody2D rb;

    [Header("Health")]
    [SerializeField] private float startHealth;
    public float health;

    [SerializeField] private Image fullHealthBar;
    [SerializeField] private Image healthBar;
    private bool dead;

    [SerializeField] private GameObject checkpoint;

    [Header("Shield")]
    [SerializeField] private float startShield;
    public float shield;

    [SerializeField] private Image fullShieldBar;
    [SerializeField] private Image shieldBar;

    [Header("iFrames")]
    [SerializeField] private float invincibilityTime;
    [SerializeField] private int flashNo;

    private bool invincible;
    public float damageReductionMultiplier = 1;
    public bool bossPhase;

    private void Awake()
    {
        am = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        health = startHealth;
        shield = 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        fullHealthBar.fillAmount = health / 10;
        fullShieldBar.fillAmount = shield / 5;
    }

    // Update is called once per frame
    private void Update()
    {
        healthBar.fillAmount = health / 10;
        fullShieldBar.fillAmount = shield / 5;

        
        if (dead)
        {
            if (bossPhase)
            {
                SceneManager.LoadScene("MainLevel");
            }
            else
            {
                dead = false;
                Respawn();
            }
        }
        
    }

    public void TakeDamage(float _damage)
    {
        if (invincible) return;

        if (shield > 0)
        {
            shield = Mathf.Clamp(shield - _damage, 0, shield);
            return;
        }

        health = Mathf.Clamp(health - (damageReductionMultiplier * _damage), 0, health);
      

        if (health > 0)
        {
            am.SetTrigger("hurt");
            StartCoroutine(Invulnerability("hit", invincibilityTime, gameObject));

        } else
        {
            if (!dead)
            {
                am.SetTrigger("die");
                GetComponent<Player>().enabled = false;
                dead = true;
            } 
        }
    }

    
    public void InstantDeath()
    {
        am.SetTrigger("die");
        GetComponent<Player>().enabled = false;
        health = 0;
        shield = 0;
        dead = true;
    }

    private void Respawn()
    {
        transform.position = checkpoint.transform.position;
        am.ResetTrigger("die");
        am.Play("Idle");
        health = startHealth;
        GetComponent<Player>().enabled = true;
        rb.velocity = new Vector2(0, 0);
    }

    public void setCheckpoint(GameObject cp)
    {
        checkpoint = cp;
    }

    public void AddHealth(float _healthAmount)
    {
        health = Mathf.Clamp(health + _healthAmount, 0, startHealth);
    }

    public void AddShield(float _shieldAmount)
    {
        shield = _shieldAmount;
    }

    public IEnumerator Invulnerability(string _type, float _duration, GameObject g)
    {
        if (_type == "power-up")
        {
            Health h = GetComponentInParent<Health>();
            invincible = true;
            Physics2D.IgnoreLayerCollision(7, 8, true);     

            sp.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(_duration);
            sp.color = Color.white;

            Physics2D.IgnoreLayerCollision(7, 8, false);
            invincible = false;
            g.SetActive(false);

        } else
        {
            Physics2D.IgnoreLayerCollision(7, 8, true);
            invincible = true;

            for (int i = 0; i < flashNo; i++)
            {
                sp.color = new Color(1, 0, 0, 0.5f);
                yield return new WaitForSeconds(_duration / (flashNo * 2));
                sp.color = Color.white;
                yield return new WaitForSeconds(_duration / (flashNo * 2));
            }

            Physics2D.IgnoreLayerCollision(7, 8, false);
            invincible = false;
        }
    }

    public void IncreaseToughness()
    {
        damageReductionMultiplier -= (float) 0.1;
    }
}
