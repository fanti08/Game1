using UnityEngine;

public class Weapon : ScriptableObject
{
    [Header("WeaponProperties")]
    public float offsetDist;
    public float cooldown;
    public float damage;
    [Header("BulletProperties")]
    public GameObject bullet;

    public virtual void StartChargingAttack()
    {

    }

    public virtual void ChargingAttack()
    {

    }

    public virtual void RealizeAttack(GameObject parent, Vector2 dir)
    {

    }


}