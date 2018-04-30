using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public Transform weaponHold; //empty game object to put the gun
    public Gun[] allGuns; //default gun for the player to hold
    Gun equipped; //variable to see if player has a gun

    void Start()
    {

    }

    public void equipWeapon(Gun gunToEquip)
    {
        if(equipped != null)
        {
            Destroy(equipped.gameObject);
        }
        equipped = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equipped.transform.parent = weaponHold;
    }

    public void equipWeapon(int weaponIndex)
    {
        equipWeapon(allGuns[weaponIndex]);
    }

    public void onTriggerHold()
    {
        if (equipped != null)
        {
            equipped.onTriggerHold();
        }
    }

    public void onTriggerRelease()
    {
        if (equipped != null)
        {
            equipped.onTriggerReleased();
        }
    }

    public float gunHeight()
    {
        return weaponHold.position.y;
        
    }

    public void aim(Vector3 aimPoint)
    {
        if (equipped != null)
        {
            equipped.aim(aimPoint);
        }
    }

    public void reload()
    {
        if (equipped != null)
        {
            equipped.reload();
        }
    }
}
