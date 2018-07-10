using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface so we can use these variables wth the player and the enemy
public interface IDamageable
{
    void takeHit (float damage, Vector3 hitPoint, Vector3 hitDir, int bulletType);//damage calculation with raycast with mostly used with bullets probably obsolete

    void takeDamage(float damage);//damage calculation without rayhit 
    
}
