using UnityEngine;

public class Target : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public ParticleSystem gunFirePrefab;
    [HideInInspector]
    public Vector3 TargetPosition;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, TargetPosition);
        if (distance > 6f) // Sadece mesafe 6'dan büyükse hareket et
        {
            Vector3 direction = (TargetPosition - transform.position).normalized;
            transform.position += direction * MoveSpeed * Time.deltaTime;
        }
       
    }
}