using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private Vector2 forceDir;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(force * forceDir.normalized, ForceMode2D.Impulse);
    }
}
