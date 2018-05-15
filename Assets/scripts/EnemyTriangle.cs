using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTriangle : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        enemyType = Type.Triangle;
    }

    protected override void Start()
    {
        base.Start();
        currentState = State.Shooting;

        //StartCoroutine(Firing());
    }
    //a shooting method for the triangle enemy
    protected virtual IEnumerator Firing()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if (currentState == State.Shooting)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (enemyCollisionRadius + targetCollisionRadius + attackDistance / 2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
