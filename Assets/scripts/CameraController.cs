using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//current bug don't know what to do with it after player is dead
public class CameraController : MonoBehaviour {
    
    public Transform playerPos;
    public float smooth = 0.3f;
    public float zOffset = 10f;
    public float yOffset = 7f;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 pos = new Vector3();
        if (playerPos != null)
        {
            pos.x = playerPos.position.x;
            pos.z = playerPos.position.z - zOffset;
            pos.y = playerPos.position.y + yOffset;
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smooth);
        }

        else if (playerPos == null)
        {
            pos.x = 1f;
            pos.y = 1f;
            pos.z = 1f;
        }
    } 
}
