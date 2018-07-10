using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy {

    public Projectile projectile;
    public Transform projectileSpawn;
    public float turnRate = 1f;
    public float muzzleVelocity = .5f;

    float shotInterval;

    protected override void Awake()
    {
        base.Awake();
        //special enemy type for the boss enemy
        enemyType = 0;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdatePath());
        StartCoroutine(Firing());

    }

    protected override void Update()
    {
        base.Update();
    }

    //a shooting method for the triangle enemy not tracking so i can disable when player is dead using coroutines and enumerators
    protected virtual IEnumerator Firing()
    {
        float refreshRate = .7f;
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation) as Projectile;
                newProjectile.setSpeed(muzzleVelocity);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
