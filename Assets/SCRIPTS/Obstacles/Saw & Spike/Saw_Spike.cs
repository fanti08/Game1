using UnityEngine;

public class Saw_Spike : Obstacle
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        KillPlayer(collision.collider);
    }
}