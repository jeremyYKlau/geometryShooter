using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody myRigidBody;

	void Start () {
        myRigidBody = GetComponent<Rigidbody>();
	}
	
	public void move (Vector3 v) {
        velocity = v;
	}

    private void FixedUpdate()
    {
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
    }

    public void lookAt(Vector3 lookPoint)
    {
        Vector3 correctedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(correctedPoint);
    }
}
