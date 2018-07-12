using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public enum FireMode { auto,burst,single};
    public FireMode fireMode;

    //public Transform muzzle; //a variable that spawns a projectile from the transform location
    public Transform[] projectileSpawn; //used for shooting style like shotgun probably remove later
    public Projectile projectile; //the projectile to shoot from the generic gun
    public float timeBetweenShot = 100; //miliseconds between each shot
    public float muzzleVelocity = 30;
    public int burstCount;

    bool triggerReleased;
    int burstShotsRemaining;

    [Header("Effects")]
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    MuzzleFlash muzzleflash;
    float shotInterval;

    //all this can be removed if i don't want to use recoil but it's growing on me
    Vector3 recoilSmoothDampVelocity; //for recoil may remove if i don't want recoil
    float recoilAngle; //for recoil up remove if i dont want
    float recoilRotSmoothDampVelocity; //for recoil up remove if i dont want

    [Header("Recoil")]
    public Vector2 minMaxKickBack = new Vector2(.05f, .2f); //max min rand range for kickback
    public Vector2 minMaxRecoil = new Vector2(3.0f, 5.0f); //max min rand range for kickback
    public float kickbackRecoverTime = .1f;
    public float recoilRecoverTime = .1f;

    public int magazineSize;//for reload probably don't want to use
    public int projectilesRemaininInMag;
    bool reloading;
    public float reloadTime = .3f;

    void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash>();
        burstShotsRemaining = burstCount;
        projectilesRemaininInMag = magazineSize;
    }

    //animate recoil remove if i don't want recoil
    void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, kickbackRecoverTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRecoverTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!reloading && projectilesRemaininInMag == 0)
        {
            reload();
        }
    }

    void shoot()
    {
        if(!reloading && Time.time > shotInterval && projectilesRemaininInMag >0) /*the && is for reloading if i decide to remove erase that part of conditional*/
        {
            if (fireMode == FireMode.burst)
            {
                //perhaps change burst somehow because i dont like having to hold it down
                if(burstShotsRemaining == 0)
                {
                    return;
                }
                burstShotsRemaining--;
            }
            else if(fireMode == FireMode.single)
            {
                if (!triggerReleased)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                //for reloading which i probably don't want
                if(projectilesRemaininInMag == 0)
                {
                    break;
                }
                projectilesRemaininInMag--;
                shotInterval = Time.time + timeBetweenShot / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile; //breaks game here and doesn't play sound but won't be a problem when i remove guns changing per round and change audio still very weird error
                newProjectile.setSpeed(muzzleVelocity);
            }
            //muzzleflash.activate(); //this line is for muzzle flash but I don't plan on using it

            transform.localPosition -= Vector3.forward * Random.Range(minMaxKickBack.x, minMaxKickBack.y); //for recoil but may remove if i don't want
            recoilAngle += Random.Range(minMaxRecoil.x, minMaxRecoil.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.playSound(shootAudio, transform.position);

            /* this block is for use with muzzle because in my game i probably won't want to use shot gun stuff
            shotInterval = Time.time + rateOfFire / 1000;
            Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation) as Projectile;
            newProjectile.setSpeed(muzzleVelocity);
            
            muzzleflash.activate();*/
        }
    }

    public void aim(Vector3 aimPoint)
    {
        //remove this if statement if i decide to remove reloading
        if (!reloading)
        {
            transform.LookAt(aimPoint);
        }
    }

    public void onTriggerHold()
    {
        shoot();
        triggerReleased = false;
    }

    public void onTriggerReleased()
    {
        triggerReleased = true;
        burstShotsRemaining = burstCount;
    }

    //reload function for the gun that may be deleted if i decide to not keep it
    public void reload()
    {
        if(!reloading && projectilesRemaininInMag != magazineSize)
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.playSound(reloadAudio, transform.position);
        }
    }

    // coroutine to animate the reload but delete if i don't keep reloading
    IEnumerator AnimateReload()
    {
        reloading = true;
        yield return new WaitForSeconds(.2f);
        float percent = 0;
        float reloadSpeed = 1 / reloadTime;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30f;

        while (percent< 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }
        reloading = false;
        projectilesRemaininInMag = magazineSize;
    }
}
