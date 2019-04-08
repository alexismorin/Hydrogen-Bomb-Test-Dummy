using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCursor : MonoBehaviour {

    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
    }

}