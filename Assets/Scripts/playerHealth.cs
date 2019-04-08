using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour {

    public float health = 100f;
    public Image bar;
    public Gradient healthColor;
    public gameStateManager manager;

    void Update () {
        bar.rectTransform.sizeDelta = new Vector2 (health * 10f, 50f);
        bar.color = healthColor.Evaluate (Mathf.Lerp (0f, 1f, health / 100f));

        if (health <= 0f) {
            manager.Die ();
        }

    }

    public void SetHealth (float newHealth) {
        health = newHealth;
    }

    void OnCollisionEnter (Collision collision) {

        if (collision.relativeVelocity.magnitude > 5f) {
            health -= collision.relativeVelocity.magnitude * 0.25f;
        }
    }
}