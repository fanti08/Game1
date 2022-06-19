using UnityEngine;
using System.Threading.Tasks;
using System;

public class Rocket : Obstacle
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float flySpeed;
    [SerializeField] private float directionLerp;
    [SerializeField] private int damage;
    [SerializeField] private Vector2 _direction;

    private async void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<SpriteRenderer>().enabled = false;
        await Task.Delay(TimeSpan.FromSeconds(.3f));
        GetComponent<SpriteRenderer>().enabled = true;

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
        KillPlayer(col);
        Destroy(gameObject);
    }
}