using UnityEngine;

public class Laser : Obstacle
{
    [SerializeField] private bool laserIsEnabled;
    [SerializeField] private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        if (laserIsEnabled)
        {
            line.SetPosition(0, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up);
            if (hit.collider != null)
            {
                line.SetPosition(1, hit.point);
                KillPlayer(hit.collider);
            }
            else
                line.SetPosition(1, transform.position + -transform.up * 1000);
        }
        else
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position);
        }
    }

    public void SetLaserState(bool enable)
    {
        laserIsEnabled = enable;
    }
}