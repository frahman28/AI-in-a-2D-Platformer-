using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : MonoBehaviour
{
    [SerializeField] private Animator am;
    [SerializeField] private Transform enemy;

    [Header("Variables")]
    [SerializeField] private Transform rightSide;
    [SerializeField] private Transform leftSide;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private bool patrol;

    private int direction;
    private bool movingRight;
    private float waitTimer;

    private Vector3 scale;

    private void Awake()
    {
        scale = enemy.localScale;
    }

    private void OnDisable()
    {
        am.SetBool("moving", false);
    }

    private void Update()
    {
        if (patrol)
        {
            if (movingRight)
            {
                if (enemy.position.x <= rightSide.position.x)
                {
                    direction = 1;
                    Move();
                }
                else
                {
                    changeDirection();
                }
            }
            else
            {
                if (enemy.position.x >= leftSide.position.x)
                {
                    direction = -1;
                    Move();
                }
                else
                {
                    changeDirection();
                }
            }
        }
    }

    private void changeDirection()
    {
        am.SetBool("moving", false);
        waitTimer += Time.deltaTime;

        if (waitTimer > waitTime)
        {
            movingRight = !movingRight;
        }
    }

    private void Move()
    {
        waitTimer = 0;

        am.SetBool("moving", true);

        enemy.localScale = new Vector3(Mathf.Abs(scale.x) * direction, scale.y, scale.z);

        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * direction * speed, enemy.position.y, enemy.position.z);
    }

}
