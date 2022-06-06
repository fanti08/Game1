using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z - 90);
    }
}
