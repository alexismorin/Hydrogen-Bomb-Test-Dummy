using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class debugControls : MonoBehaviour {
    void Update () {
        if (Input.GetKeyDown (KeyCode.F10)) {
            Scene scene = SceneManager.GetActiveScene ();
            SceneManager.LoadScene (scene.name);
        }

        if (Input.GetKeyUp (KeyCode.Escape)) {
            Application.Quit ();
        }

    }
}