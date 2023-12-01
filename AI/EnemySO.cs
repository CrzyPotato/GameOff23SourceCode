using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Entities/Enemies", fileName = "New Enemy")]
public class EnemySO : ScriptableObject
{
    public GameObject enemyPrefab;
    [FormerlySerializedAs("cost")] public int spawnCost;
    public int waveToSpawn; // What wave can this enemy start spawning?
    public int enemyHealth;
    public int enemyDamage;
    public float enemySpeed;
}
