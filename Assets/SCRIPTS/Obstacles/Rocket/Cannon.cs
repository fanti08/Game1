using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject rocketPrefab;
    public GameObject player;

    [Range(.01f, 50)] public float minRadius;
    [Range(.01f, 50)] public float maxRadius;
    [Range(.01f, 10)] public float rocketCooldown;

    [SerializeField] private float _cooldown;
    [SerializeField] private bool isActive;

    void Awake()
    {
        _cooldown = rocketCooldown;
    }

    void Update()
    {
        isActive = (minRadius <= Vector3.Distance(transform.position, player.transform.position)) &&
                   (Vector3.Distance(transform.position, player.transform.position) <= maxRadius);
        if (isActive)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position + (Vector3)(direction * minRadius),
                direction,
                maxRadius
            );
            if (hit.collider.gameObject.tag == "Player") SpawnRocket();
        }
        _cooldown -= Time.deltaTime;
    }

    void SpawnRocket()
    {
        if (_cooldown <= 0)
        {
            Instantiate(rocketPrefab, transform.position, Quaternion.identity);
            _cooldown = rocketCooldown;
        }
    }
}