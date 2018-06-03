using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    
    public LayerMask collisionMask;
    float speed = 10; //bullet speed taken from gun object when used in game
    float damage = 1;
    float lifeTime = 3; //time bullet stays in game before destroy
    float skinWidth = .2f; //small area to help bullet hit box
    public Color trailColour;


    void Start()
    {
        Destroy(gameObject, lifeTime);
        Collider[] initalCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initalCollisions.Length > 0)
        {
            onHitObject(initalCollisions[0], transform.position);
        }
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColour);
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        checkCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void checkCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            onHitObject(hit.collider, hit.point);
        }
    }


    void onHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.takeHit(damage, hitPoint, transform.forward); //when collides with a object do a damage calculation witout the raycasting
        }
        GameObject.Destroy(gameObject);
    }
}
