using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public virtual void KillPlayer(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            collider.gameObject.GetComponent<Player>().isDead = true;
    }
}