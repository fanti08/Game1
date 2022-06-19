using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private bool follow;
    private float rotation_z;

    private void Awake()
    {
        Cursor.visible = false;
        if (follow)
            player = GameObject.Find("PLAYER").GetComponent<Transform>();
    }

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        if (follow)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.position;
            difference.Normalize();
            rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation_z - 135);
        }
    }
}
