using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public float flySpeed;
    public float directionLerp;
    public int damage;
    private Vector2 _direction;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        _direction = transform.up;
        rb.velocity = flySpeed * _direction;
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.up = Vector2.Lerp(_direction, direction, directionLerp);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }
}