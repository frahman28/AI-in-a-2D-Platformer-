using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float damage;

    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private Animator am;

    private void Awake()
    {
        am = GetComponent<Animator>();
    }

    public void ActivateProjectile(string _shotAngle )
    {
        lifetime = 0;
        transform.rotation = Quaternion.identity;

        if (_shotAngle == "down")
        {
            transform.Rotate(0, 0, -10);
        } else if (_shotAngle == "up")
        {
            transform.Rotate(0, 0, 10);
        }
        
        gameObject.SetActive(true);
    }
    private void Update()
    {
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "PowerUp")
        {
            if (collision.tag == "Player")
                collision.GetComponent<Health>().TakeDamage(damage);

            am.SetTrigger("explode");

            gameObject.SetActive(false);
        }
    }
}
