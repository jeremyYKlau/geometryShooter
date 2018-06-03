using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTriangle : Enemy
{
    public Projectile projectile;
    public Transform projectileSpawn;
    public float turnRate = 1f;
    public float muzzleVelocity = .5f;

    float shotInterval;

    protected override void Awake()
    {
        base.Awake();
        enemyType = Type.Triangle;
    }

    protected override void Start()
    {
        base.Start();
        currentState = State.Shooting;
        StartCoroutine(Firing());
        
    }
    protected override void Update()
    {
        //if has target look for and rotate towards target
        if (hasTarget)
        {
            Vector3 targetDirection = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnRate * Time.deltaTime);
        }

        if (!hasTarget)
        {
            currentState = State.Idle;
        }
    }

    //a shooting method for the triangle enemy not tracking so i can disable when player is dead using coroutines and enumerators
    protected virtual IEnumerator Firing()
    {
        float refreshRate = .7f;
        while (hasTarget)
        {
            if (currentState == State.Shooting)
            {
                Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation) as Projectile;
                newProjectile.setSpeed(muzzleVelocity);
              
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
