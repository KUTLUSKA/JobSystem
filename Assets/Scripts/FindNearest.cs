using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FindNearest : MonoBehaviour
{
    NativeArray<float3> TargetPositions;
    NativeArray<float3> SeekerPositions;
    NativeArray<float3> NearestTargetPositions;

    public GameObject lineRendererPrefab;
    public float damageRange = 6f;
    public float damageAmount = 10f;
    public ParticleSystem gunFirePrefab;
    private Dictionary<Transform, float> seekerNextFireTimes = new Dictionary<Transform, float>();


    private List<LineRenderer> activeLines = new List<LineRenderer>();

    public void Start()
    {
        Spawner spawner = Object.FindObjectOfType<Spawner>();
        TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
        SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        TargetPositions.Dispose();
        SeekerPositions.Dispose();
        NearestTargetPositions.Dispose();

        foreach (var line in activeLines)
        {
            Destroy(line.gameObject);
        }
    }

   public void Update()
{
    // Pozisyon dizilerini güncelle
    for (int i = 0; i < TargetPositions.Length; i++)
        TargetPositions[i] = Spawner.TargetTransforms[i].position;

    for (int i = 0; i < SeekerPositions.Length; i++)
        SeekerPositions[i] = Spawner.SeekerTransforms[i].position;

    // SEEKER → NEAREST TARGET
    FindNearestJob findForSeekers = new FindNearestJob
    {
        TargetPositions = TargetPositions,
        SeekerPositions = SeekerPositions,
        NearestTargetPositions = NearestTargetPositions,
    };

    JobHandle seekerHandle = findForSeekers.Schedule(SeekerPositions.Length, 100);
    seekerHandle.Complete();

    for (int i = 0; i < SeekerPositions.Length; i++)
    {
        Vector3 seekerPos = SeekerPositions[i];
        Vector3 targetPos = NearestTargetPositions[i];

        // Hareket ettir
        Spawner.SeekerTransforms[i].GetComponent<Seeker>().TargetPosition = targetPos;

        // Hasar kontrolü
        float distance = Vector3.Distance(seekerPos, targetPos);
        if (distance <= damageRange)
        {
            Transform seekerTransform = Spawner.SeekerTransforms[i];
            Transform targetTransform = FindMatchingTargetTransform(targetPos);

            if (targetTransform != null)
            {
                HealthComponent hc = targetTransform.GetComponent<HealthComponent>();
                if (hc != null)
                {
                    // Zamanlayıcı kontrolü
                    float nextFireTime;
                    if (!seekerNextFireTimes.TryGetValue(seekerTransform, out nextFireTime))
                    {
                        nextFireTime = 0f;
                    }

                    if (Time.time >= nextFireTime)
                    {
                        hc.TakeDamage(damageAmount);

                        // Ateş efekti
                        ParticleSystem gunFX = seekerTransform.GetComponent<Seeker>().gunFirePrefab;
                        if (gunFX != null)
                        {
                            Instantiate(gunFX, seekerTransform.position, Quaternion.identity);
                        }

                        // Yeni zaman belirle
                        seekerNextFireTimes[seekerTransform] = Time.time + 2f;
                    }
                }
            }
        }


        // Çizgi çiz
        if (i >= activeLines.Count)
        {
            GameObject lineObj = Instantiate(lineRendererPrefab);
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            activeLines.Add(lr);
        }

        LineRenderer lineRenderer = activeLines[i];
        lineRenderer.SetPosition(0, seekerPos);
        lineRenderer.SetPosition(1, targetPos);
    }

    // TARGET → NEAREST SEEKER
    for (int i = 0; i < TargetPositions.Length; i++)
    {
        float minDist = float.MaxValue;
        Vector3 nearestSeeker = Vector3.zero;

        Vector3 targetPos = TargetPositions[i];
        for (int j = 0; j < SeekerPositions.Length; j++)
        {
            float dist = Vector3.Distance(targetPos, SeekerPositions[j]);
            if (dist < minDist)
            {
                minDist = dist;
                nearestSeeker = SeekerPositions[j];
            }
        }

        // Hareket ettir
        Spawner.TargetTransforms[i].GetComponent<Target>().TargetPosition = nearestSeeker;
    }
}


    private Transform FindMatchingTargetTransform(Vector3 pos)
    {
        foreach (Transform t in Spawner.TargetTransforms)
        {
            if (Vector3.Distance(t.position, pos) < 0.1f)
            {
                return t;
            }
        }
        return null;
    }
}
