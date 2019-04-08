using System.Collections;
using UnityEngine;

[CreateAssetMenu (fileName = "Disaster")]
public class disasterType : ScriptableObject {
    public string disasterName = "Disaster";
    public float duration = 30f;
    public GameObject[] threats;
    public int numberOfThreats;
}