using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : Character {

    public float moveSpeed = 3;

    public Crosshairs crosshair;

    Camera viewCamera;
    PlayerController controller;
    GunController gunControl;

	protected override void Start () {
        base.Start();
    }

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunControl = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().onNewWave += onNewWave;
        gunControl.equipWeapon(0);
    }

    void onNewWave(int waveNum)
    {
        health = startHealth;
    }
	
    //for now movement is just there as a test I'll figure out how i want to actually do it later
	void Update () {
        //Movement inputs
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.move(moveVelocity);

        //Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.up * gunControl.gunHeight());
        float rayDistance;

        if (ground.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.blue);
            controller.lookAt(point);
            crosshair.transform.position = point;
            crosshair.onTarget(ray);
            if (((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude) > 1.5)
            {
                gunControl.aim(point);
            }
        }

        //Shooting input
        if (Input.GetMouseButton(0))
        {
            gunControl.onTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunControl.onTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R)){
            gunControl.reload();
        }
        if(transform.position.y < -10)
        {
            takeDamage(health);
        }

        //gunswitching
        if (Input.GetKeyDown("1"))
        {
            gunControl.equipWeapon(0);
        }
        if (Input.GetKeyDown("2"))
        {
            gunControl.equipWeapon(1);
        }
        if (Input.GetKeyDown("3"))
        {
            gunControl.equipWeapon(2);
        }
        if (Input.GetKeyDown("4"))
        {
            gunControl.equipWeapon(3);
        }
    }

    public override void die()
    {
        AudioManager.instance.playSound("Player Death", transform.position);
        base.die();
    }
}
