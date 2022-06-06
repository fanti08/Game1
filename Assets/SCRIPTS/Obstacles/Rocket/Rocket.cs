using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public float flySpeed;
    public float directionLerp;
    private Vector2 _direction;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        _direction = transform.up;
        rb.velocity = flySpeed * _direction;
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.up = Vector2.Lerp(_direction, direction, directionLerp);
    }
}
