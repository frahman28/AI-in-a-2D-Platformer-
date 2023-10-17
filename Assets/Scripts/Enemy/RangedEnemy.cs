using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;
    private float cooldownTimer = 1000000;
    [SerializeField] private float colliderDistance;

    [SerializeField] private Transform shotOrigin;
    [SerializeField] private GameObject[] shots; 

    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    private Animator am;
    private Health playerHealth;

    private int probabilityUp;
    private int probabilityStraight;
    private int probabilityDown;

    private void Awake()
    {
        am = GetComponent<Animator>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerSeen())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                am.SetTrigger("ranged");
            }
        }
    }

    private bool PlayerSeen()
    {
        probabilityStraight = 0;
        probabilityDown = 0;
        probabilityUp = 0;

        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(bc.bounds.size.x * range, bc.bounds.size.y, bc.bounds.size.z), 0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            float playerVerticalVel = hit.collider.GetComponent<Rigidbody2D>().velocity.y;
            bool playerGrounded = hit.collider.GetComponent<Player>().grounded;

            TransformerFunction(playerVerticalVel, playerGrounded);
        }

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bc.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(bc.bounds.size.x * range, bc.bounds.size.y, bc.bounds.size.z));
    }

    private void RangedAttack()
    {
        int index = Random.Range(0, probabilityUp+probabilityStraight+probabilityDown);
        string shotAngle = "straight";
        if (index < probabilityUp)
        {
            shotAngle = "up";
        } else if (index < probabilityUp + probabilityStraight)
        {
            shotAngle = "straight";
        } else if (index < probabilityUp + probabilityStraight + probabilityDown)
        {
            shotAngle = "down";
        }
        cooldownTimer = 0;
        shots[chooseShot()].transform.position = shotOrigin.position;
        shots[chooseShot()].GetComponent<EnemyProjectile>().ActivateProjectile(shotAngle);
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

    private void TransformerFunction(float _playerYVel, bool _playerGrounded)
    {
        RaycastHit2D checkUp = Physics2D.Raycast(bc.bounds.center, transform.TransformDirection(new Vector2(1f, -0.1f)) * transform.localScale.x, (float)(range * 0.4), groundLayer);
        RaycastHit2D checkDown = Physics2D.Raycast(bc.bounds.center, transform.TransformDirection(new Vector2(1f, 0.1f)) * transform.localScale.x, (float)(range * 0.4), groundLayer);

        if (checkUp.collider != null && checkDown.collider != null)
        {
            probabilityStraight += 1;
        } else
        {
            if (checkUp.collider != null)
            {
                probabilityStraight += 1;
                probabilityDown += 1;
            } else if (checkDown.collider != null)
            {
                probabilityStraight += 1;
                probabilityUp += 1;
            }
        }

        if (_playerGrounded)
        {
            probabilityStraight += 1;
        } else
        {
            if (_playerYVel > 2)
            {
                probabilityUp += 1;
            }
            if (_playerYVel < -2)
            {
                probabilityDown += 1;
            }
            if (_playerYVel <= 2 && _playerYVel >= -2)
            {
                probabilityStraight += 1;
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.GetComponent<Health>().TakeDamage(damage);
    }
}
