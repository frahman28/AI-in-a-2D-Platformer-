using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class ExplosionEnemy : MonoBehaviour
{
    private Animator am;
    private Rigidbody2D rb;

    private bool followPlayer;
    private Path p;
    private Seeker s;
    private int targetNode;
    private bool atDestination = false;

    [SerializeField] private bool BossMode = false;

    [SerializeField] private float visionRange;
    [SerializeField] private Transform player;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float nextNodeDistance;

    [SerializeField] private LayerMask playerLayer;
    float xScale;
    float yScale;
    float zScale;

    private void Awake()
    {
        am = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        s = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        xScale = transform.localScale.x;
        yScale = transform.localScale.y;
        zScale = transform.localScale.z;


        InvokeRepeating("RegeneratePath", 0f, 0.5f); 
    }

    private void FixedUpdate()
    {
        PlayerSeen();

        if (followPlayer)
        {
            if (p == null)
                return;

            if (targetNode >= p.vectorPath.Count)
            {
                atDestination = true;
                return;
            } else
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

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Player")
            collision.GetComponent<Health>().TakeDamage(100);

        GetComponent<EnemyHealth>().TakeDamage(100);
        StartCoroutine(TimeDeath());
    }

    private void PlayerSeen()
    {
        Collider2D[] playerSensed = Physics2D.OverlapCircleAll(transform.position, visionRange, playerLayer);

        if (playerSensed.Length == 1)
        {
            followPlayer = true;
        }
    }

    private IEnumerator TimeDeath()
    {
        yield return new WaitForSeconds(2);
        GetComponent<EnemyHealth>().enabled = false;
        this.enabled = false;
        this.gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }

    void PathGenerated(Path path)
    {
        if (!path.error)
        {
            p = path;
            targetNode = 0;
        }
    }
}
