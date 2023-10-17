using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool hit;
    private float direction;
    private float lifetime;
    private float power;

    private BoxCollider2D bc;
    private Animator am;

    // Start is called before the first frame update
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        am = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hit) return;
        
        float travelSpeed = speed * Time.deltaTime * direction;
        transform.Translate(travelSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "PowerUp")
        {
            hit = true;
            bc.enabled = false;
            am.SetTrigger("explode");

            if (collision.tag == "Enemy")
            {
                collision.GetComponent<EnemyHealth>().TakeDamage(power);
            } else if (collision.tag == "Boss")
            {
                collision.GetComponent<Boss>().TakeDamage(power);
            }
        }
    }

    public void setPower(float _power)
    {
        power = _power;
    }

    public void setDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        bc.enabled = true;

        float xLocalScale = transform.localScale.x;

        if (Mathf.Sign(xLocalScale) != _direction)
        {
            xLocalScale = -xLocalScale;
        }

        transform.localScale = new Vector3(xLocalScale, transform.localScale.y, transform.localScale.z);
    }

    private void deactivate()
    {
        gameObject.SetActive(false);
    }
}

