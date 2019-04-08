using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour {
    public float explosionForce = 4;

    void Start () {

        var cols = Physics.OverlapSphere (transform.position, 5f);
        var rigidbodies = new List<Rigidbody> ();

        foreach (var col in cols) {
            if (col.attachedRigidbody != null && !rigidbodies.Contains (col.attachedRigidbody)) {
                rigidbodies.Add (col.attachedRigidbody);
            }
        }
        foreach (var rb in rigidbodies) {
            rb.AddExplosionForce (explosionForce * 2f, transform.position, 5f, 5f, ForceMode.Impulse);
        }
    }

}