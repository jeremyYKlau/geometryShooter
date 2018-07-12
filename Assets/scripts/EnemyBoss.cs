using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy {

    public Projectile projectile;
    public Transform projectileSpawn1;
    public Transform projectileSpawn2;
    public Transform projectileSpawn3;
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
                Projectile newProjectile1 = Instantiate(projectile, projectileSpawn1.position, projectileSpawn1.rotation) as Projectile;
                Projectile newProjectile2 = Instantiate(projectile, projectileSpawn2.position, projectileSpawn2.rotation) as Projectile;
                Projectile newProjectile3 = Instantiate(projectile, projectileSpawn3.position, projectileSpawn3.rotation) as Projectile;
                newProjectile1.setSpeed(muzzleVelocity);
                newProjectile2.setSpeed(muzzleVelocity);
                newProjectile3.setSpeed(muzzleVelocity);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
