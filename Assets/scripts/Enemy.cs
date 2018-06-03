using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character {

    //important to know whether the enemy is attacking chasing or idle using enums and a State
    public enum State { Idle, Chasing, Attacking, Shooting};
    public enum Type { Square, Sphere, Triangle, Boss};
    protected Type enemyType;
    protected State currentState;

    public ParticleSystem deathEffect;
    public static event System.Action onDeathStatic;

    protected NavMeshAgent pathFinder; //a navmesh for path finding made with component->navigation->NavMesh and then baked
    protected Transform target; //target position for ai to move towards
    protected Character targetEntity; //the actual target character in this case player

    //delete later just for changing colour when attacking which i don't want
    protected Material skin;
    protected Color originalColour;

    protected float attackDistance = 1f;
    protected float timeBetweenAtk = 1f;
    protected float damage = 1;

    protected float nextAttackTime;
    protected float enemyCollisionRadius;
    protected float targetCollisionRadius;

    protected bool hasTarget;

    protected virtual void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectsWithTag("Player") != null)
        {
            hasTarget = true;

            //there's an error here if the player dies while the enemy is being instantiated as hasTarget never gets set to false so looks for null player
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<Character>();

            enemyCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
        else
        {
            hasTarget = false;
            currentState = State.Idle;
        }
    }

    protected override void Start () {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.onDeath += onTargetDeath;
            
        }
	}

    public override void takeHit(float damage, Vector3 hitPoint, Vector3 hitDir)
    {
        AudioManager.instance.playSound("Impact", transform.position);
        if (damage >= health)
        {
            if (onDeathStatic != null)
            {
                onDeathStatic();
            }
            AudioManager.instance.playSound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDir)), deathEffect.main.startLifetime.constant);
        }
        base.takeHit(damage, hitPoint, hitDir);
    }

    public void setStats(float moveSpeed, int damageToKill, float charHealth, Color skinColour)
    {
        var main = deathEffect.main;
        pathFinder.speed = moveSpeed;
        if (hasTarget)
        {
            damage = (int)Mathf.Ceil(targetEntity.startHealth / damageToKill); //change to just damage = damage when finished this is just some janky thing to make it hits to kill player and not damage
        }
        startHealth = charHealth;

        main.startColor = new Color(skinColour.r, skinColour.g, skinColour.b, 1);
        skin = GetComponent<Renderer>().material;
        skin.color = skinColour;
        originalColour = skin.color;//probably delete after done tutorial
    }

    void onTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    protected virtual void Update () {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < Mathf.Pow(attackDistance + enemyCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAtk;
                    AudioManager.instance.playSound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }
    //coroutine a small lunge when close to the player
    protected virtual IEnumerator Attack()
    {
        currentState = State.Attacking;
        //disable pathfinder coroutine so that it doesn't run at the same time as attack coroutine
        pathFinder.enabled = false;
        
        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (enemyCollisionRadius);

        float percent = 0;
        float attackSpeed = 2;

        skin.color = Color.black;//proably delete after done tutorial
        bool hasDamaged = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasDamaged)
            {
                hasDamaged = true;
                targetEntity.takeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);

            yield return null;
        }

        skin.color = originalColour;//again delete after tutorial
        currentState = State.Chasing;
        //reenable pathfinder once attack is done
        pathFinder.enabled = true;
    }

    //Coroutine called Update path so the navigation Ai isn't updating path every frame
    protected virtual IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while(hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (enemyCollisionRadius + targetCollisionRadius + attackDistance/2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
