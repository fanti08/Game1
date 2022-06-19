using UnityEngine;

public class _cam : MonoBehaviour
{
    [SerializeField] private Transform player;
     
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);
    }
}
