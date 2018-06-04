using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySphere : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        //enemyType = Type.Sphere;
        enemyType = 2;
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(UpdatePath());
    }

    protected override void Update()
    {
        base.Update();
    }

    //coroutine updated for the sphere
    protected override IEnumerator Attack()
    {
        currentState = State.Attacking;
        //disable pathfinder coroutine so that it doesn't run at the same time as attack coroutine
        pathFinder.enabled = false;

        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (enemyCollisionRadius);

        bool hasDamaged = false;
        float percent = 0;
        float attackSpeed = 2;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasDamaged)
            {
                hasDamaged = true;
                targetEntity.takeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);

            yield return null;
        }
        
        currentState = State.Chasing;
        //reenable pathfinder once attack is done
        pathFinder.enabled = true;
    }
}
