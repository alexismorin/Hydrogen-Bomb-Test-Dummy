using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameStateManager : MonoBehaviour {

    public Text uiText;
    public Text timerText;
    public Image dullScreen;
    public float lobbyTime = 3f;
    public disasterType[] diasters;
    public int round = 0;

    public disasterType currentDisaster;
    public float currentTimer;
    public bool isInThreat;

    void Start () {
        EnterLobby ();
    }

    public void Die () {
        CancelInvoke ("AnnounceDisaster");
        CancelInvoke ("StartDisaster");
        CancelInvoke ("SpawnThreat");
        CancelInvoke ("EnterLobby");
        dullScreen.enabled = true;
        uiText.text = "Wipeout!";
        uiText.color = Color.red;
        Invoke ("ReloadLevel", 1.5f);
    }

    public void ReloadLevel () {
        Scene scene = SceneManager.GetActiveScene ();
        SceneManager.LoadScene (scene.name);
    }

    void EnterLobby () {

        dullScreen.enabled = true;

        GameObject[] pieces = GameObject.FindGameObjectsWithTag ("piece");
        foreach (GameObject piece in pieces) {
            Destroy (piece);
        }

        GameObject[] buildings = GameObject.FindGameObjectsWithTag ("building");
        foreach (GameObject building in buildings) {
            building.SendMessage ("Restore", SendMessageOptions.DontRequireReceiver);
        }

        GameObject[] spawners = GameObject.FindGameObjectsWithTag ("spawner");
        foreach (GameObject spawner in spawners) {
            spawner.SendMessage ("spawnDummies", SendMessageOptions.DontRequireReceiver);
        }
        round++;
        uiText.text = "Round " + round + " !";
        Invoke ("AnnounceDisaster", lobbyTime);
        currentDisaster = diasters[Random.Range (0, diasters.Length)];
    }

    void AnnounceDisaster () {
        uiText.text = currentDisaster.name + "!";
        currentTimer = currentDisaster.duration;
        Invoke ("StartDisaster", 3f);
    }

    void StartDisaster () {
        dullScreen.enabled = false;
        uiText.text = "";
        isInThreat = true;
        InvokeRepeating ("SpawnThreat", 0f, (currentDisaster.duration / currentDisaster.numberOfThreats) * 0.9f);
    }

    void SpawnThreat () {
        Vector3 spawnLocation = new Vector3 (Random.Range (-20f, 40f), Random.Range (15f, 40f), Random.Range (-50f, -10f));
        GameObject.Instantiate (currentDisaster.threats[Random.Range (0, currentDisaster.threats.Length)], spawnLocation, Quaternion.identity);
    }

    void Update () {
        if (currentTimer <= 0f && isInThreat == true) {
            EndDisaster ();
        }

        if (isInThreat) {
            currentTimer -= 1f * Time.deltaTime;
            timerText.text = Mathf.Round (currentTimer).ToString ();
        }

    }

    void EndDisaster () {
        CancelInvoke ("SpawnThreat");
        DestroyThreats ();
        isInThreat = false;
        timerText.text = "";
        Invoke ("EnterLobby", 1f);
    }

    void DestroyThreats () {

        GameObject[] npcs = GameObject.FindGameObjectsWithTag ("npc");
        foreach (GameObject npc in npcs) {
            Destroy (npc);
        }

        GameObject[] threats = GameObject.FindGameObjectsWithTag ("threat");
        foreach (GameObject threat in threats) {
            Destroy (threat);
        }

    }
}