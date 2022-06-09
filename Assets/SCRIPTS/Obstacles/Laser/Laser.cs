using UnityEngine;
using System.Collections.Generic;

public class Laser : MonoBehaviour
{
    public bool laserIsEnabled;
    [SerializeField] private List<Transform> movePoints = new List<Transform>();
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool pingPong;
    [Range(0, 1)] [SerializeField] private float time;
    [SerializeField] private Vector2 targetPos;
    [SerializeField] private float targetZRot;
    [SerializeField] private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        if (pingPong) 
            time = Mathf.PingPong(Time.time * speed, 1);

        float generalDist = 0;
        List<float> distsBetween = new List<float>();
        for (int i = 1; i < movePoints.Count; i++)
        {
            float dist = Vector2.Distance(movePoints[i].position, movePoints[i - 1].position);
            generalDist += dist;
            distsBetween.Add(dist);
        }
        float targetDist = generalDist * time;

        for (int i = 0; i < distsBetween.Count; i++)
        {
            if (targetDist - distsBetween[i] >= 0)
                targetDist -= distsBetween[i];
            else
            {
                targetPos = movePoints[i].position + ((movePoints[i + 1].position - movePoints[i].position).normalized * targetDist);
                targetZRot = movePoints[i].eulerAngles.z + ((movePoints[i + 1].eulerAngles.z - movePoints[i].eulerAngles.z) * (targetDist / distsBetween[i]));
                break;
            }
        }

        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetZRot));

        if (laserIsEnabled)
        {
            line.SetPosition(0, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up);
            if (hit.collider != null)
            {
                line.SetPosition(1, hit.point);
                if (hit.collider.CompareTag("Player"))
                    hit.collider.GetComponent<Player>().isDead = true;
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