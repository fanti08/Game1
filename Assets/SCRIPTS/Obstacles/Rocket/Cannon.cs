using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private GameObject player;
    [Range(.01f, 50)] [SerializeField] private float minRadius;
    [Range(.01f, 50)] [SerializeField] private float maxRadius;
    [Range(.01f, 10)] [SerializeField] private float rocketCooldown;
    [Range(0, 1)] [SerializeField] private float turnFactor;
    [SerializeField] private float cooldown;
    [SerializeField] private bool isActive;
    [SerializeField] private Player script_player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cooldown = rocketCooldown;
    }

    void Update()
    {
        isActive = (minRadius <= Vector3.Distance(transform.position, player.transform.position)) && (Vector3.Distance(transform.position, player.transform.position) <= maxRadius);
        if (isActive)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.up = Vector2.Lerp(transform.up, direction, turnFactor);
            RaycastHit2D hit_up = Physics2D.Raycast(transform.position + (Vector3)(direction * minRadius) + .5f * transform.right, direction, maxRadius);
            RaycastHit2D hit_down = Physics2D.Raycast(transform.position + (Vector3)(direction * minRadius) - .5f * transform.right, direction, maxRadius);
            if (hit_up.collider.gameObject.tag == "Player" && hit_down.collider.gameObject.tag == "Player") SpawnRocket();
        }
        cooldown -= Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        if(script_player.isDebugActive)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Gizmos.DrawLine(transform.position + (Vector3)(direction * minRadius) + .49f * transform.right, transform.position + (Vector3)(direction * maxRadius) + 0.25f * transform.right);
            Gizmos.DrawLine(transform.position + (Vector3)(direction * minRadius) - .49f * transform.right, transform.position + (Vector3)(direction * maxRadius) - 0.25f * transform.right);
        }
    }

    void SpawnRocket()
    {
        if (cooldown <= 0)
        {
            Instantiate(rocketPrefab, transform.position, Quaternion.identity);
            cooldown = rocketCooldown;
        }
    }
}