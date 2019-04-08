using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour {

    public GameObject spawnTarget;

    public void spawnDummies () {
        GameObject.Instantiate (spawnTarget, transform.position, Quaternion.identity);
    }

}