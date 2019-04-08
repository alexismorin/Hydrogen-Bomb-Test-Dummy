using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingManager : MonoBehaviour {

    public GameObject building;

    public void Restore () {

        foreach (Transform child in this.transform.GetChild (0)) {
            GameObject.Destroy (child.gameObject);
        }

        GameObject.Instantiate (building, transform.GetChild (0).position, transform.rotation);
    }

}