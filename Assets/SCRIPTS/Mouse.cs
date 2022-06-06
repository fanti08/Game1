using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] Camera cam;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}
