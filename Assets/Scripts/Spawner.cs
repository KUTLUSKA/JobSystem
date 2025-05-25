using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public static Transform[] TargetTransforms;
    public static Transform[] SeekerTransforms;

    public GameObject SeekerPrefab;
    public GameObject TargetPrefab;
    public int NumSeekers;
    public int NumTargets;

    // Sahneye yerleştirilen boş GameObject'leri buraya sürükle
    public List<Transform> SeekerSpawnPoints;
    public List<Transform> TargetSpawnPoints;

    public void Start()
    {
        Random.InitState(123);

        // Seeker spawn
        SeekerTransforms = new Transform[NumSeekers];
        for (int i = 0; i < NumSeekers; i++)
        {
            Transform spawnPoint = (i < SeekerSpawnPoints.Count) ? SeekerSpawnPoints[i] : null;
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

            GameObject go = Instantiate(SeekerPrefab, spawnPos, Quaternion.identity);
            SeekerTransforms[i] = go.transform;
        }

        // Target spawn
        TargetTransforms = new Transform[NumTargets];
        for (int i = 0; i < NumTargets; i++)
        {
            Transform spawnPoint = (i < TargetSpawnPoints.Count) ? TargetSpawnPoints[i] : null;
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

            GameObject go = Instantiate(TargetPrefab, spawnPos, Quaternion.identity);
            TargetTransforms[i] = go.transform;
        }
    }
}