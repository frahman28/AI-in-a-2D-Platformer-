using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sp;
    [SerializeField] private Sprite health;
    [SerializeField] private Sprite shield;
    [SerializeField] private Sprite invisibility;
    [SerializeField] private Sprite invincibility;
    [SerializeField] private Sprite strength;
    [SerializeField] private Sprite speed;
    [SerializeField] private Sprite tough;

    private string p;

    [SerializeField] private float healthValue;
    [SerializeField] private float shieldValue;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private float invisibilityDuration;

    [SerializeField] private GameObject player;

    private bool inputHealth;
    private bool inputShield;
    private bool inputToughness;
    private bool inputStrength;
    private bool inputSpeed;

    private float healthProbability = 10;
    private float shieldProbability = 10;
    private float toughnessProbability = 10;
    private float strengthProbability = 10;
    private float speedProbability = 10;
    private float invisProbability = 10;
    private float invincibilityProbability = 10;

    private void Start()
    {
        sp = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnBecameVisible()
    {
        //chooseRandomPowerUp();
        if (p == null)
        {
            FuzzyInputs();
            setProbabilityDistribution();
            choosePowerUp();
        }
        Debug.Log(p);
    }

    private void choosePowerUp()
    {
        int index = Random.Range(0, 70);

        Debug.Log(index);

        if (index < healthProbability)
        {
            sp.sprite = health;
            p = "health";
        }
        else if (index < healthProbability + shieldProbability)
        {
            sp.sprite = shield;
            p = "shield";
        }
        else if (index < healthProbability + shieldProbability + invisProbability)
        {
            sp.sprite = invisibility;
            p = "invis";
        }
        else if (index < healthProbability + shieldProbability + invisProbability + invincibilityProbability)
        {
            sp.sprite = invincibility;
            p = "invinc";
        }
        else if (index < healthProbability + shieldProbability + invisProbability + invincibilityProbability + strengthProbability)
        {
            sp.sprite = strength;
            p = "strength";
        }
        else if (index < healthProbability + shieldProbability + invisProbability + invincibilityProbability + strengthProbability + speedProbability)
        {
            sp.sprite = speed;
            p = "speed";
        }
        else if (index < healthProbability + shieldProbability + invisProbability + invincibilityProbability + strengthProbability + speedProbability + toughnessProbability)
        {
            sp.sprite = tough;
            p = "tough";
        }
    }
    private void chooseRandomPowerUp()
    {
        int index = Random.Range(0, 7);

        if (index == 0)
        {
            sp.sprite = health;
            p = "health";
        }
        else if (index == 1)
        {
            sp.sprite = shield;
            p = "shield";
        }
        else if (index == 2)
        {
            sp.sprite = invisibility;
            p = "invis";
        }
        else if (index == 3)
        {
            sp.sprite = invincibility;
            p = "invinc";
        }
        else if (index == 4)
        {
            sp.sprite = strength;
            p = "strength";
        }
        else if (index == 5)
        {
            sp.sprite = speed;
            p = "speed";
        }
        else if (index == 6)
        {
            sp.sprite = tough;
            p = "tough";
        }
    }

    private void FuzzyInputs()
    {
        //Normalise Inputs 
        float normalisedHealth = player.GetComponent<Health>().health / 10;
        float normaliseShield = player.GetComponent<Health>().shield / 5;
        float normaliseToughness = 1 - player.GetComponent<Health>().damageReductionMultiplier;
        float normaliseStrength = (player.GetComponent<Player>().shotPower - 1) / 2;
        float normaliseSpeed = (player.GetComponent<Player>().speedCap - 9) / 5;

        //Linguistic Rules for inputs, true = High, false = Low
        if (normalisedHealth > 0.5)
        {
            inputHealth = true;
        } else if (normalisedHealth <= 0.5)
        {
            inputHealth = false;
        }
        if (normaliseShield > 0.5)
        {
            inputShield = true;
        } else if (normaliseShield <= 0.5)
        {
            inputShield = false;
        }
        if (normaliseToughness > 0.5)
        {
            inputToughness = true;
        } else if (normaliseToughness <= 0.5)
        {
            inputToughness = false;
        }
        if (normaliseStrength > 0.5)
        {
            inputStrength = true;
        } else if (normaliseStrength <= 0.5)
        {
            inputStrength = false;
        }
        if (normaliseSpeed > 0.5)
        {
            inputSpeed = true;
        } else if (normaliseSpeed <= 0.5)
        {
            inputSpeed = false;
        }
    }

    private void setProbabilityDistribution()
    {
        if (!inputHealth && !inputShield && !inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 10;
            toughnessProbability = 10;
            strengthProbability = 10;
            speedProbability = 10;
            invisProbability = 5;
            invincibilityProbability = 10;
            Debug.Log("LLLLL");
        }
        else if (!inputHealth && !inputShield && !inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 15;
            toughnessProbability = 15;
            strengthProbability = 15;
            speedProbability = 0;
            invisProbability = 5;
            invincibilityProbability = 5;
            Debug.Log("LLLLH");
        }
        else if (!inputHealth && !inputShield && !inputToughness && inputStrength && !inputSpeed) {
            healthProbability = 20;
            shieldProbability = 10;
            toughnessProbability = 10;
            strengthProbability = 10;
            speedProbability = 0;
            invisProbability = 10;
            invincibilityProbability = 10;
            Debug.Log("LLLHL");
        }
        else if (!inputHealth && !inputShield && !inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 15;
            toughnessProbability = 10;
            strengthProbability = 10;
            speedProbability = 10;
            invisProbability = 5;
            invincibilityProbability = 5;
            Debug.Log("LLLHH");
        }
        else if (!inputHealth && !inputShield && inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 10;
            toughnessProbability = 0;
            strengthProbability = 15;
            speedProbability = 15;
            invisProbability = 5;
            invincibilityProbability = 10;
            Debug.Log("LLHLL");
        }
        else if (!inputHealth && !inputShield && inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 10;
            toughnessProbability = 5;
            strengthProbability = 10;
            speedProbability = 5;
            invisProbability = 10;
            invincibilityProbability = 15;
            Debug.Log("LLHLH");
        }
        else if (!inputHealth && !inputShield && inputToughness && inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 10;
            toughnessProbability = 5;
            strengthProbability = 5;
            speedProbability = 15;
            invisProbability = 15;
            invincibilityProbability = 10;
            Debug.Log("LLHHL");
        }
        else if (!inputHealth && !inputShield && inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 15;
            toughnessProbability = 5;
            strengthProbability = 5;
            speedProbability = 5;
            invisProbability = 10;
            invincibilityProbability = 5;
            Debug.Log("LLHHH");
        }
        else if (!inputHealth && inputShield && !inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 0;
            toughnessProbability = 15;
            strengthProbability = 15;
            speedProbability = 15;
            invisProbability = 5;
            invincibilityProbability = 5;
            Debug.Log("LHLLL");
        }
        else if (!inputHealth && inputShield && !inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 5;
            toughnessProbability = 15;
            strengthProbability = 15;
            speedProbability = 5;
            invisProbability = 10;
            invincibilityProbability = 5;
            Debug.Log("LHLLH");
        }
        else if (!inputHealth && inputShield && !inputToughness && inputStrength && !inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 0;
            toughnessProbability = 10;
            strengthProbability = 5;
            speedProbability = 15;
            invisProbability = 15;
            invincibilityProbability = 10;
            Debug.Log("LHLHL");
        }
        else if (!inputHealth && inputShield && !inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 20;
            shieldProbability = 5;
            toughnessProbability = 15;
            strengthProbability = 5;
            speedProbability = 5;
            invisProbability = 10;
            invincibilityProbability = 10;
            Debug.Log("LHLHH");
        }
        else if (!inputHealth && inputShield && inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 5;
            toughnessProbability = 5;
            strengthProbability = 15;
            speedProbability = 15;
            invisProbability = 10;
            invincibilityProbability = 5;
            Debug.Log("LHHLL");
        }
        else if (!inputHealth && inputShield && inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 5;
            toughnessProbability = 5;
            strengthProbability = 15;
            speedProbability = 5;
            invisProbability = 10;
            invincibilityProbability = 15;
            Debug.Log("LHHLH");
        }
        else if (!inputHealth && inputShield && inputToughness && inputStrength && !inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 5;
            toughnessProbability = 0;
            strengthProbability = 5;
            speedProbability = 15;
            invisProbability = 15;
            invincibilityProbability = 15;
            Debug.Log("LHHHL");
        }
        else if (!inputHealth && inputShield && inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 10;
            toughnessProbability = 5;
            strengthProbability = 10;
            speedProbability = 10;
            invisProbability = 10;
            invincibilityProbability = 10;
            Debug.Log("LHHHH");
        }
        else if (inputHealth && !inputShield && !inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 10;
            toughnessProbability = 10;
            strengthProbability = 10;
            speedProbability = 10;
            invisProbability = 10;
            invincibilityProbability = 10;
            Debug.Log("HLLLL");
        }
        else if (inputHealth && !inputShield && !inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 15;
            toughnessProbability = 15;
            strengthProbability = 15;
            speedProbability = 0;
            invisProbability = 10;
            invincibilityProbability = 5;
            Debug.Log("HLLLH");
        }
        else if (inputHealth && !inputShield && !inputToughness && inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 15;
            toughnessProbability = 15;
            strengthProbability = 5;
            speedProbability = 15;
            invisProbability = 5;
            invincibilityProbability = 5;
            Debug.Log("HLLHL");
        }
        else if (inputHealth && !inputShield && !inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 15;
            toughnessProbability = 15;
            strengthProbability = 5;
            speedProbability = 5;
            invisProbability = 10;
            invincibilityProbability = 10;
            Debug.Log("HLLHH");
        }
        else if (inputHealth && !inputShield && inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 15;
            toughnessProbability = 0;
            strengthProbability = 15;
            speedProbability = 15;
            invisProbability = 10;
            invincibilityProbability = 5;
            Debug.Log("HLHLL");
        }
        else if (inputHealth && !inputShield && inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 15;
            toughnessProbability = 5;
            strengthProbability = 15;
            speedProbability = 5;
            invisProbability = 10;
            invincibilityProbability = 10;
            Debug.Log("HLHLH");
        }
        else if (inputHealth && !inputShield && inputToughness && inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 15;
            toughnessProbability = 5;
            strengthProbability = 5;
            speedProbability = 15;
            invisProbability = 10;
            invincibilityProbability = 10;
            Debug.Log("HLHHL");
        }
        else if (inputHealth && !inputShield && inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 15;
            toughnessProbability = 5;
            strengthProbability = 5;
            speedProbability = 5;
            invisProbability = 15;
            invincibilityProbability = 15;
            Debug.Log("HLHHH");
        }
        else if (inputHealth && inputShield && !inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 0;
            toughnessProbability = 15;
            strengthProbability = 15;
            speedProbability = 15;
            invisProbability = 15;
            invincibilityProbability = 0;
            Debug.Log("HHLLL");
        }
        else if (inputHealth && inputShield && !inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 5;
            toughnessProbability = 15;
            strengthProbability = 15;
            speedProbability = 5;
            invisProbability = 15;
            invincibilityProbability = 5;
            Debug.Log("HHLLH");
        }
        else if (inputHealth && inputShield && !inputToughness && inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 5;
            toughnessProbability = 15;
            strengthProbability = 5;
            speedProbability = 15;
            invisProbability = 15;
            invincibilityProbability = 5;
            Debug.Log("HHLHL");
        }
        else if (inputHealth && inputShield && !inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 5;
            toughnessProbability = 15;
            strengthProbability = 5;
            speedProbability = 5;
            invisProbability = 15;
            invincibilityProbability = 10;
            Debug.Log("HHLHH");
        }
        else if (inputHealth && inputShield && inputToughness && !inputStrength && !inputSpeed)
        {
            healthProbability = 10;
            shieldProbability = 5;
            toughnessProbability = 15;
            strengthProbability = 15;
            speedProbability = 15;
            invisProbability = 5;
            invincibilityProbability = 5;
            Debug.Log("HHHLL");
        }
        else if (inputHealth && inputShield && inputToughness && !inputStrength && inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 5;
            toughnessProbability = 5;
            strengthProbability = 15;
            speedProbability = 5;
            invisProbability = 15;
            invincibilityProbability = 10;
            Debug.Log("HHHLH");
        }
        else if (inputHealth && inputShield && inputToughness && inputStrength && !inputSpeed)
        {
            healthProbability = 15;
            shieldProbability = 5;
            toughnessProbability = 5;
            strengthProbability = 10;
            speedProbability = 20;
            invisProbability = 15;
            invincibilityProbability = 10;
            Debug.Log("HHHHL");
        }
        else if (inputHealth && inputShield && inputToughness && inputStrength && inputSpeed)
        {
            healthProbability = 20;
            shieldProbability = 5;
            toughnessProbability = 5;
            strengthProbability = 5;
            speedProbability = 5;
            invisProbability = 20;
            invincibilityProbability = 10;
            Debug.Log("HHHHH");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (p == "health")
            {
                collision.GetComponent<Health>().AddHealth(healthValue);
                gameObject.SetActive(false);
            }
            else if (p == "shield")
            {
                collision.GetComponent<Health>().AddShield(shieldValue);
                gameObject.SetActive(false);
            }
            else if (p == "invis")
            {
                StartCoroutine(collision.GetComponent<Player>().Invisibility(invisibilityDuration, gameObject));
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
            else if (p == "invinc")
            {
                StartCoroutine(collision.GetComponent<Health>().Invulnerability("power-up", invincibilityDuration, gameObject));
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
            else if (p == "strength")
            {
                collision.GetComponent<Player>().increasePower();
                gameObject.SetActive(false);
            }
            else if (p == "speed")
            {
                collision.GetComponent<Player>().SpeedUp();
                gameObject.SetActive(false);
            }
            else if (p == "tough")
            {
                collision.GetComponent<Health>().IncreaseToughness();
                gameObject.SetActive(false);
            }
        }
    }
}
