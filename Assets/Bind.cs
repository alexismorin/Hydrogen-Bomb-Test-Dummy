using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bind : MonoBehaviour {

    public GameObject breakSpawn; // object spawned on break
    public LayerMask bindMask;

    //   void OnJointBreak (float breakForce) {
    //       if (Random.Range (0, 8) == 1) {
    //          GameObject.Instantiate (breakSpawn, transform.position, Quaternion.identity);
    //      }

    //  }

    void Awake () {

        RaycastHit hitUp;
        RaycastHit hitDown;
        RaycastHit hitLeft;
        RaycastHit hitRight;
        RaycastHit hitForwards;
        RaycastHit hitBack;

        float jointBreakForce = 125f;

        // Down
        if (Physics.Raycast (transform.GetComponent<Renderer> ().bounds.center, transform.TransformDirection (Vector3.down), out hitDown, 2f, bindMask)) {
            if (hitDown.transform.gameObject != this.gameObject) {
                FixedJoint joint = gameObject.AddComponent (typeof (FixedJoint)) as FixedJoint;
                joint.connectedBody = hitDown.transform.gameObject.GetComponent<Rigidbody> ();
                joint.breakForce = jointBreakForce;
                joint.connectedAnchor = hitDown.transform.GetComponent<Renderer> ().bounds.center;
            }
        }

        // Up
        if (Physics.Raycast (transform.GetComponent<Renderer> ().bounds.center, transform.TransformDirection (Vector3.up), out hitUp, 2f, bindMask)) {
            if (hitUp.transform.gameObject != this.gameObject) {
                FixedJoint joint = gameObject.AddComponent (typeof (FixedJoint)) as FixedJoint;
                joint.connectedBody = hitUp.transform.gameObject.GetComponent<Rigidbody> ();
                joint.breakForce = jointBreakForce;
                joint.connectedAnchor = hitUp.transform.GetComponent<Renderer> ().bounds.center;
            }
        }

        // Left
        if (Physics.Raycast (transform.GetComponent<Renderer> ().bounds.center, transform.TransformDirection (Vector3.left), out hitLeft, 2f, bindMask)) {
            if (hitLeft.transform.gameObject != this.gameObject) {
                FixedJoint joint = gameObject.AddComponent (typeof (FixedJoint)) as FixedJoint;
                joint.connectedBody = hitLeft.transform.gameObject.GetComponent<Rigidbody> ();
                joint.breakForce = jointBreakForce;
                joint.connectedAnchor = hitLeft.transform.GetComponent<Renderer> ().bounds.center;
            }
        }

        // Right
        if (Physics.Raycast (transform.GetComponent<Renderer> ().bounds.center, transform.TransformDirection (Vector3.right), out hitRight, 2f, bindMask)) {
            if (hitRight.transform.gameObject != this.gameObject) {
                FixedJoint joint = gameObject.AddComponent (typeof (FixedJoint)) as FixedJoint;
                joint.connectedBody = hitRight.transform.gameObject.GetComponent<Rigidbody> ();
                joint.breakForce = jointBreakForce;
                joint.connectedAnchor = hitRight.transform.GetComponent<Renderer> ().bounds.center;
            }
        }

        // Forward
        if (Physics.Raycast (transform.GetComponent<Renderer> ().bounds.center, transform.TransformDirection (Vector3.forward), out hitForwards, 2f, bindMask)) {
            if (hitForwards.transform.gameObject != this.gameObject) {
                FixedJoint joint = gameObject.AddComponent (typeof (FixedJoint)) as FixedJoint;
                joint.connectedBody = hitForwards.transform.gameObject.GetComponent<Rigidbody> ();
                joint.breakForce = jointBreakForce;
                joint.connectedAnchor = hitForwards.transform.GetComponent<Renderer> ().bounds.center;
            }
        }

        // Back
        if (Physics.Raycast (transform.GetComponent<Renderer> ().bounds.center, transform.TransformDirection (Vector3.back), out hitBack, 2f, bindMask)) {
            if (hitBack.transform.gameObject != this.gameObject) {
                FixedJoint joint = gameObject.AddComponent (typeof (FixedJoint)) as FixedJoint;
                joint.connectedBody = hitBack.transform.gameObject.GetComponent<Rigidbody> ();
                joint.breakForce = jointBreakForce;
                joint.connectedAnchor = hitBack.transform.GetComponent<Renderer> ().bounds.center;
            }
        }

    }

}