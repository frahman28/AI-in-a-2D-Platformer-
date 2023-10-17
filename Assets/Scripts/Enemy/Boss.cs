using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private Animator am;
    private SpriteRenderer sp;

    [Header("Health")]
    [SerializeField] private float startHealth;

    private float health;
    private bool dead;
    private bool shielded;
    private Vector3 scale;
    private float playerDistance;
    private bool jumping;
    private bool landing;
    private float yPos;
    private float cooldownTimer = 0;

    //[SerializeField] private Image fullHealthBar;
    //[SerializeField] private Image healthBar;

    [SerializeField] private GameObject winImage;

    [SerializeField] private float attackPower;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask playerProjectile;
    private GameObject player;

    [SerializeField] private Transform shotOrigin;
    [SerializeField] private GameObject[] shots;

    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private float colliderDistance;
    [SerializeField] private float range;
    [SerializeField] private float actionCooldown;
    
    private int probabilityJump;
    private int probabilityShoot;
    private int probabilityMelee;
    private int probabilityShield;

    private void Awake()
    {
        am = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        health = startHealth;

        scale = transform.localScale;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //fullHealthBar.fillAmount = health / 100;

        yPos = transform.position.y;
    }

    // Update is called once per frame
    private void Update()
    {
        //healthBar.fillAmount = health / 100;

        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);

            playerDistance = player.transform.position.x - transform.position.x;
        } 
        else if (player.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(scale.x) * -1, scale.y, scale.z);

            playerDistance = player.transform.position.x - transform.position.x;
        }

        if (cooldownTimer > actionCooldown)
        {
            chooseAction();
        }

        if (jumping)
        {
            float travelSpeed = 15 * Time.deltaTime * 1;
            transform.Translate(0, travelSpeed, 0);
        }

        if (landing)
        {
            float travelSpeed = 30 * Time.deltaTime * -1;
            transform.Translate(0, travelSpeed, 0);
        }

        cooldownTimer += Time.deltaTime;

        am.SetBool("shielded", shielded);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.GetComponent<Health>().TakeDamage(1);
    }

    private bool PlayerProjectileSeen()
    {
        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center + transform.right * (2*range) * transform.localScale.x * colliderDistance, new Vector3(bc.bounds.size.x * range, bc.bounds.size.y, bc.bounds.size.z), 0, Vector2.left, 0, playerProjectile);

        return hit.collider != null;
    }

    private void chooseAction()
    {
        probabilityJump = 1;
        probabilityMelee = 1;
        probabilityShield = 1;
        probabilityShoot = 1;

        TransformerFunction();

        int index = Random.Range(0, probabilityJump+probabilityMelee+probabilityShield+probabilityShoot);

        if (index < probabilityShield) Shield();
        else if (index < probabilityShield+probabilityMelee && index >= probabilityShield) AreaOfAttack();
        else if (index < probabilityShield+probabilityMelee+probabilityJump && index >= probabilityShield + probabilityMelee) Jump();
        else if (index < probabilityShield + probabilityMelee + probabilityJump+probabilityShoot && index >= probabilityShield + probabilityMelee + probabilityJump) Shoot();

        Debug.Log(index);
        Debug.Log("shoot: "+probabilityShoot);
        Debug.Log("jump: "+probabilityJump);
        Debug.Log("shield: "+probabilityShield);
        Debug.Log("melee: "+probabilityMelee);
    }

    private void AreaOfAttack()
    {
        am.SetTrigger("attack");

        Collider2D[] playerHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        for (int i = 0; i < playerHit.Length; i++)
        {
            playerHit[i].GetComponent<Health>().TakeDamage(attackPower);
        }
        cooldownTimer = 0;
    }

    private void Jump()
    {
        am.SetTrigger("jump");
        StartCoroutine(TimeJump());
        cooldownTimer = 0;
    }

    private void Shield()
    {
        StartCoroutine(Shielding(3));
        cooldownTimer = 0;
    }

    private void Shoot()
    {
        am.SetTrigger("ranged");
        cooldownTimer = 0;
    }

    private void RangedAttack()
    {
        shots[chooseShot()].transform.position = shotOrigin.position;
        shots[chooseShot()].SetActive(true);
        shots[chooseShot()].GetComponent<ExplosionEnemy>().enabled = true;
        shots[chooseShot()].GetComponent<EnemyHealth>().enabled = true;
    }

    private IEnumerator TimeJump()
    {
        jumping = true;
        yield return new WaitForSeconds(3);
        jumping = false;

        transform.Translate(playerDistance, 0, 0);

        landing = true;
        yield return new WaitForSeconds((float) 1.5);
        landing = false;

        transform.Translate(0, yPos - transform.position.y, 0);
    }

    private IEnumerator Shielding(float _duration)
    {
        shielded = true;
        //Physics2D.IgnoreLayerCollision(7, 8, true);

        sp.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(_duration);
        sp.color = Color.white;

        //Physics2D.IgnoreLayerCollision(7, 8, false);
        shielded = false;
    }

    public void TakeDamage(float _damage)
    {
        if (shielded) return;

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

                if (GetComponent<Boss>() != null)
                    GameWin();

                dead = true;
            }
        }
    }

    private int chooseShot()
    {
        for (int i = 0; i < shots.Length; i++)
        {
            if (!shots[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    private void TransformerFunction()
    {
        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        float playerXVel = player.GetComponent<Rigidbody2D>().velocity.x;
        Debug.Log(distanceToPlayer);

        if (playerXVel >= 3 || playerXVel <= -3)
        {
            probabilityJump -= 1;
            probabilityShoot += 1;
        }

        if (PlayerProjectileSeen())
        {
            probabilityShield += 2;
            probabilityJump += 1;
            Debug.Log("yes");
        }

        if (distanceToPlayer <= attackRange)
        {
            probabilityMelee += 1;
            probabilityShield += 1;
            probabilityShoot -= 1;
        } else if (distanceToPlayer > attackRange)
        {
            probabilityJump += 1;
            probabilityShoot += 1;
        }

        if ((playerXVel > 1 || playerXVel < -1) && player.GetComponent<Player>().speedCap > 11.5 && distanceToPlayer <= attackRange)
        {
            probabilityJump -= 1;
            probabilityMelee += 1;
            probabilityShield += 1;
            probabilityShoot -= 1;
        }

        if (player.GetComponent<Player>().shotPower > 2)
        {
            probabilityShield += 2;
        }
    }

    private void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }

    private void GameWin()
    {
        winImage.SetActive(true);
        GetComponent<Boss>().enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bc.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(bc.bounds.size.x * range, bc.bounds.size.y, bc.bounds.size.z));
    }
}