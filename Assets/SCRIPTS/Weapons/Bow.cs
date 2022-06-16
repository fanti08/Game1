using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Weapon/Bow")]
public class Bow : Weapon
{
    public float maxChargeDuration;
    public float minSpeed;
    public float maxSpeed;
    private float chargeValue;
    private float startChargingTime;

    public override void StartChargingAttack()
    {
        base.StartChargingAttack();
        startChargingTime = Time.time;
    }

    public override void ChargingAttack()
    {
        base.ChargingAttack();
        chargeValue = Mathf.Clamp01((Time.time - startChargingTime) / maxChargeDuration);
    }

    public override void RealizeAttack(GameObject parent, Vector2 dir)
    {
        Vector2 offsetttedPos = (Vector2)parent.transform.position + (dir * offsetDist);
        base.RealizeAttack(parent, dir);
        GameObject newBullet = Instantiate(bullet, offsetttedPos, parent.transform.rotation);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        float force = ((maxSpeed - minSpeed) * chargeValue) + minSpeed;
        rb.AddForce(force * dir, ForceMode2D.Impulse);
    }
}