using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : MonoBehaviour
{
    private Animator am;
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private SpriteRenderer sp;

    [Header("Movement")]
    [SerializeField] public float speedCap;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float friction;
    private float resistance;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float jumpStrength;
    public bool grounded;
    [SerializeField] private float increasedGravity;

    [Header("Attacks")]
    [SerializeField] private float meleeCooldown;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float homingCooldown;
    [SerializeField] public float shotPower;
    [SerializeField] private float meleePower;
    private float cooldownTimer = 100000000;
    [SerializeField] private Transform blastOrigin;
    [SerializeField] private GameObject[] blasts;
    [SerializeField] private Transform meleePoint;
    [SerializeField] private float meleeRange;
    [SerializeField] private LayerMask enemies;
    [SerializeField] private float homingRange;

    [Header("Other")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask deathPlane;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        float xInput = Input.GetAxis("Horizontal");

        grounded = checkGroundCollision();
        CheckOutOfBounds();

        //Move player according to x input
        evaluateHorizontalSpeed(xInput);
        rb.velocity = new Vector2(horizontalSpeed, rb.velocity.y);

        //Change player sprite direction
        if (xInput > 0.01f)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else if (xInput < -0.01f)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
        }

        //Check space bar pressed
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            playerJump();
        }

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 1 * increasedGravity;
        }
        else
        {
            rb.gravityScale = 1;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        }

        if (Input.GetKey(KeyCode.P) && cooldownTimer > shootCooldown)
        {
            shoot();
        }

        if (Input.GetKey(KeyCode.O) && cooldownTimer > meleeCooldown)
        {
            melee();
        }

        if (Input.GetKey(KeyCode.I) && !grounded && cooldownTimer > homingCooldown)
        {
            airAttack();
        }

        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("MainLevel");
        }

        cooldownTimer += Time.deltaTime;

        //Set animator conditions
        am.SetBool("run", horizontalSpeed > 8 || horizontalSpeed < -8);
        am.SetBool("walk", (xInput != 0 && horizontalSpeed < 8) || (xInput != 0 && horizontalSpeed > -8));
        am.SetBool("grounded", grounded);
    }

    //Function calculating player x velocity
    private void evaluateHorizontalSpeed(float xInput)
    {

        if (!grounded)
        {
            resistance = 2 * friction;
        }
        else
        {
            resistance = friction;
        }

        if (xInput < 0)
        {
            if (horizontalSpeed > 0)
            {
                horizontalSpeed -= deceleration;

                if (horizontalSpeed <= 0)
                {
                    horizontalSpeed = (float)-0.5;
                }
            }
            else if (horizontalSpeed > -speedCap)
            {
                horizontalSpeed -= acceleration;
                if (horizontalSpeed <= -speedCap)
                {
                    horizontalSpeed = -speedCap;
                }
            }
        }

        if (xInput > 0)
        {
            if (horizontalSpeed < 0)
            {
                horizontalSpeed += deceleration;

                if (horizontalSpeed >= 0)
                {
                    horizontalSpeed = (float)0.5;
                }
            }
            else if (horizontalSpeed < speedCap)
            {
                horizontalSpeed += acceleration;
                if (horizontalSpeed >= speedCap)
                {
                    horizontalSpeed = speedCap;
                }
            }
        }

        if (xInput == 0)
        {
            horizontalSpeed -= Mathf.Min(Mathf.Abs((float)horizontalSpeed), resistance) * horizontalSpeed;
        }
    }

    //Set jump functions
    private void playerJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        am.SetTrigger("jump");
    }

    //Check ground collision
    private bool checkGroundCollision()
    {
        RaycastHit2D rc = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        return rc.collider != null;
    }
    private void CheckOutOfBounds()
    {
        RaycastHit2D rc = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0.1f, deathPlane);

        if (rc)
        {
            GetComponent<Health>().InstantDeath();
        }
    }
    
    private void melee()
    {
        am.SetTrigger("melee");
        cooldownTimer = 0;

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(meleePoint.position, meleeRange, enemies);

        for (int i = 0; i < enemiesHit.Length; i++)
        {
            if (enemiesHit[i].GetComponent<EnemyHealth>() != null)
            {
                enemiesHit[i].GetComponent<EnemyHealth>().TakeDamage(meleePower);
            } else if (enemiesHit[i].GetComponent<Boss>() != null)
            {
                enemiesHit[i].GetComponent<Boss>().TakeDamage(meleePower);
            }
        }
    }

    private void shoot()
    {
        am.SetTrigger("shoot");
        cooldownTimer = 0;

        blasts[chooseProjectile()].transform.position = blastOrigin.position;
        blasts[chooseProjectile()].GetComponent<Projectile>().setPower(shotPower);
        blasts[chooseProjectile()].GetComponent<Projectile>().setDirection(Mathf.Sign(transform.localScale.x));
    }
    private void airAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, homingRange, enemies);

        int chosenEnemyIndex = chooseEnemyToHome(enemiesHit);
        float targetX = Mathf.Infinity;
        float targetY = Mathf.Infinity;

        Debug.Log(chosenEnemyIndex);

        if (chosenEnemyIndex > enemiesHit.Length)
        {
        } else
        {
            targetX = enemiesHit[chosenEnemyIndex].transform.position.x;
            targetY = enemiesHit[chosenEnemyIndex].transform.position.y;

            rb.transform.position = new Vector3(targetX + Time.deltaTime * 16, targetY + Time.deltaTime * 16, rb.transform.position.z);

            if (enemiesHit[chosenEnemyIndex].GetComponent<EnemyHealth>() != null)
            {
                enemiesHit[chosenEnemyIndex].GetComponent<EnemyHealth>().TakeDamage(6);
            }
            else if (enemiesHit[chosenEnemyIndex].GetComponent<Boss>() != null)
            {
                enemiesHit[chosenEnemyIndex].GetComponent<Boss>().TakeDamage(6);
            }

            cooldownTimer = 0;
        }
    }
    private int chooseEnemyToHome(Collider2D[] _enemies)
    {
        Collider2D[] enemiesHit = _enemies;

        int indexOfClosestEnemy = 1000;

        if (enemiesHit.Length == 0)
        {

        } else if (enemiesHit.Length == 1)
        {
            indexOfClosestEnemy = 0;
        } else
        {
            float[] enemyDistance = new float[enemiesHit.Length];

            for (int i = 0; i < enemiesHit.Length; i++)
            {
                float distance = Mathf.Sqrt(Mathf.Abs(rb.position.x - enemiesHit[i].transform.position.x) * Mathf.Abs(rb.position.x - enemiesHit[i].transform.position.x) + Mathf.Abs(rb.position.y - enemiesHit[i].transform.position.y) * Mathf.Abs(rb.position.y - enemiesHit[i].transform.position.y));
                enemyDistance[i] = distance;
            }

            indexOfClosestEnemy = Array.IndexOf(enemyDistance, enemyDistance.Min());

        }

        return indexOfClosestEnemy;
    }

    private int chooseProjectile()
    {
        for (int i = 0; i < blasts.Length; i++)
        {
            if (!blasts[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    public IEnumerator Invisibility(float _duration, GameObject g)
    {
        gameObject.layer = 9;

        sp.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(_duration);
        sp.color = Color.white;

        gameObject.layer = 7;

        g.SetActive(false);
    }

    public void SpeedUp()
    {
        if (!(speedCap >= 14)) speedCap += (float) 0.5;
        if (!(acceleration >= 0.07)) acceleration += (float) 0.003;
        if (!(meleeCooldown <= 0.4)) meleeCooldown -= (float) 0.02;
        if (!(shootCooldown <= 0.4)) shootCooldown -= (float) 0.03;
        if (!(homingCooldown <= 1)) shootCooldown -= (float) 0.05;
    }

    public void increasePower()
    {
        if (!(shotPower >= 3)) shotPower += (float) 0.2;
        if (!(meleePower >= 5)) meleePower += (float) 0.2;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, homingRange);
    }
}
