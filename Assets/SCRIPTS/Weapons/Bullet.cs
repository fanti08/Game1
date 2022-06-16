using UnityEngine;
using System;
using System.Threading.Tasks;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask stopLayer;
    [SerializeField] private float yOffset;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool stuck;
    [SerializeField] private Vector2 previousPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        previousPos = rb.position;
    }

    private async void FixedUpdate()
    {
        Vector3 vel = rb.velocity;
        Vector2 offset = (transform.lossyScale.y + yOffset) * (Vector2)transform.up;
        Vector2 dir = (rb.position + offset) - previousPos;
        RaycastHit2D hit = Physics2D.Raycast(previousPos, dir.normalized, dir.magnitude, stopLayer);
        if (hit.collider == null)
        {
            stuck = false;
            rb.isKinematic = false;
            rb.freezeRotation = false;
            float zAngle = Quaternion.LookRotation(Vector3.forward, vel).eulerAngles.z;
            rb.SetRotation(zAngle);
        }
        if (stuck) 
            return;
        if (hit.collider != null)
        {
            stuck = true;
            rb.isKinematic = true;
            rb.position = hit.point - offset;
            rb.velocity = Vector2.zero;
            rb.freezeRotation = true;
            await Task.Delay(TimeSpan.FromSeconds(3));
            Destroy(gameObject);
        }
        previousPos = rb.position;
    }
}