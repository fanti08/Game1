using UnityEngine;

public class Holder : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse1;

    private float lastShootTime;
    private void Update()
    {
        if (lastShootTime + weapon.cooldown < Time.time)
        {
            if (Input.GetKeyDown(shootKey))
                weapon.StartChargingAttack();
            if (Input.GetKey(shootKey))
                weapon.ChargingAttack();
            if (Input.GetKeyUp(shootKey))
            {
                Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                dir.Normalize();
                weapon.RealizeAttack(gameObject, dir);
                lastShootTime = Time.time;
            }
        }
    }
}