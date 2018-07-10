using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySphere : Enemy
{
    float forceAdjust = 0.05f;
    Rigidbody sphereBody;
    protected override void Awake()
    {
        base.Awake();
        //enemyType = Type.Sphere;
        enemyType = 2;
    }

    protected override void Start()
    {
        base.Start();
        sphereBody = this.GetComponent<Rigidbody>();
        //this is going to be different for the sphere it will use a force to launch itself at the player instead and roll around
        StartCoroutine(UpdatePath());
    }

    protected override void Update()
    {
        base.Update();
    }

    //coroutine updated for the sphere just make it into a new updatepath coroutine and put the percent thing with damage in that coroutine instead
    //remove this attack coroutine after the sphere updatepath attack merge 
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

        //doesn't work but something i would like to include
        //sphereBody.AddForce(dirToTarget * sphereBody.mass * forceAdjust, ForceMode.Impulse);

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
    /*
    //Coroutine called Update path so the navigation Ai isn't updating path every frame
    protected override IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if (currentState == State.Chasing)
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
    }*/
}
