using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable {
    
    public float startHealth;

    public float health { get; protected set; }
    protected bool dead;

    public event System.Action onDeath;

    protected virtual void Start()
    {
        health = startHealth;
    }

    //take hit method implemented from IDamageable interface for bullet damage
    public virtual void takeHit(float damage, Vector3 hitPoint, Vector3 hitDir, int bulletType)
    {
        //do stuff here with hit var like a particle effect
        takeDamage(damage);
    }

    //also from
    public virtual void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            die();
        }
    }

    public virtual void die()
    {
        dead = true;
        if (onDeath != null)
        {
            onDeath();
        }
        GameObject.Destroy(gameObject);
    }
}
