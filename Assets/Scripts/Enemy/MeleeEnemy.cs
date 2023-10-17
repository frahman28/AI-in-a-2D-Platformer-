using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;
    private float cooldownTimer = 1000000;
    [SerializeField] private float colliderDistance;
    [SerializeField] private Transform player;
    [SerializeField] private float movementSpeed = 300;
    [SerializeField] private float nextNodeDistance = 2;
    [SerializeField] private float visionRange = 10;

    [Header("Other")]
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    private Path p;
    private Seeker s;
    private int targetNode;
    private bool atDestination = false;

    private Animator am;
    private Health playerHealth;

    private PatrolBehaviour pb;
    private Rigidbody2D rb;

    private bool followPlayer;

    private void Awake()
    {
        am = GetComponent<Animator>();
        pb = GetComponentInParent<PatrolBehaviour>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        s = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        transform.localScale = new Vector3(-1.6f, 1.7f, 1.6f);

        InvokeRepeating("RegeneratePath", 0f, 0.5f);
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerSeen())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                am.SetTrigger("melee");
            }
        }

        if (pb != null)
        {
            pb.enabled = !PlayerSeen();
        }

        am.SetBool("moving", rb.velocity.x >= 0.01f || rb.velocity.x <= -0.01f);
    }

    private void FixedUpdate()
    {
        DetectPlayer();

        if (followPlayer)
        {
            if (p == null)
                return;

            if (targetNode >= p.vectorPath.Count)
            {
                atDestination = true;
                return;
            }
            else
            {
                atDestination = false;
            }

            Vector2 direction = ((Vector2)p.vectorPath[targetNode] - rb.position).normalized;
            Vector2 force = direction * movementSpeed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, p.vectorPath[targetNode]);

            if (distance < nextNodeDistance)
            {
                targetNode++;
            }

            if (force.x >= 0.01f)
            {
                transform.localScale = new Vector3(1.6f, 1.7f, 1.6f);
            }
            else if (force.x <= -0.01f)
            {
                transform.localScale = new Vector3(-1.6f, 1.7f, 1.6f);
            }

            if (checkWall() && force.y >= 0.01f)
            {
                if (checkGroundCollision()) rb.AddForce(Vector2.up * 12000 * Time.deltaTime);
            }
        }
    }

    private bool PlayerSeen()
    {
        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(bc.bounds.size.x * range, bc.bounds.size.y, bc.bounds.size.z), 0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
        }
        return hit.collider != null;
    }

    private bool checkWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(bc.bounds.size.x * range, bc.bounds.size.y, bc.bounds.size.z), 0, Vector2.left, 0, groundLayer);

        return hit.collider != null;
    }

    private void DetectPlayer()
    {
        Collider2D[] playerSensed = Physics2D.OverlapBoxAll(transform.position, new Vector2(visionRange, 2), 0, playerLayer);

        if (playerSensed.Length == 1)
        {
            followPlayer = true;
        } else if (playerSensed.Length == 0)
        {
            followPlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(visionRange, 2));
        //Gizmos.DrawWireCube(bc.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(bc.bounds.size.x * range, bc.bounds.size.y, bc.bounds.size.z));
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.GetComponent<Health>().TakeDamage(damage);
    }

    private void DamagePlayer()
    {
        if (PlayerSeen())
        {
            playerHealth.TakeDamage(damage);
        }
    }

    void RegeneratePath()
    {
        if (followPlayer)
        {
            if (s.IsDone())
            {
                s.StartPath(rb.position, player.position, PathGenerated);
            }
        }
    }

    void PathGenerated(Path path)
    {
        if (!path.error)
        {
            p = path;
            targetNode = 0;
        }
    }

    private bool checkGroundCollision()
    {
        RaycastHit2D rc = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        return rc.collider != null;
    }
}
