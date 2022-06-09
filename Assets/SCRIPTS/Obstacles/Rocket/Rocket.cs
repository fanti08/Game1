using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float flySpeed;
    [SerializeField] private float directionLerp;
    [SerializeField] private int damage;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private Player script_player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        script_player = player.GetComponent<Player>();
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
        if (col.gameObject.CompareTag("Player"))
            script_player.isDead = true;
        Destroy(gameObject);
    }
}