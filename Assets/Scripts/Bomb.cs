using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public GameObject explosionPrefab;

    void OnCollisionEnter (Collision other) {
        GameObject.Instantiate (explosionPrefab, transform.position, Random.rotation);
        Destroy (gameObject);
    }
}