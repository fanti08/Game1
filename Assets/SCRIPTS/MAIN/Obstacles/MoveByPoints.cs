using UnityEngine;
using System.Collections.Generic;

public class MoveByPoints : MonoBehaviour
{
    [SerializeField] private List<Transform> movePoints = new List<Transform>();
    [SerializeField] private float speed = 0.4f;
    [SerializeField] private bool move;
    [Range(0, 1)] [SerializeField] private float time;
    [Header("LoopProperties")]
    [SerializeField] private bool loop;
    [SerializeField] private bool loopClockWise;

    Vector2 targetPos;
    float targetZRot;

    void FixedUpdate()
    {
        if (move)
        {
            if (loop)
                time = loopClockWise ? Mathf.Repeat(Time.time * speed, 1) : 1 - Mathf.Repeat(Time.time * speed, 1);
            else
                time = Mathf.PingPong(Time.time * speed, 1);
        }

        if (movePoints.Count == 1)
        {
            targetZRot = movePoints[0].eulerAngles.z + time * 360;
            targetPos = movePoints[0].position;
        }
        else
        {
            float generalDist = 0;
            List<float> distsBetween = new List<float>();
            for (int i = 1; i < movePoints.Count; i++)
            {
                float dist = Vector3.Distance(movePoints[i].position, movePoints[i - 1].position);
                generalDist += dist;
                distsBetween.Add(dist);
            }

            if (loop)
            {
                float dist = Vector3.Distance(movePoints[0].position, movePoints[movePoints.Count - 1].position);
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
                    int firstIndex = i;
                    int secondIndex = i + 1;
                    if (secondIndex > movePoints.Count - 1) 
                        secondIndex = 0;
                    targetPos = movePoints[firstIndex].position + ((movePoints[secondIndex].position - movePoints[firstIndex].position).normalized * targetDist);
                    targetZRot = movePoints[firstIndex].eulerAngles.z + ((movePoints[secondIndex].eulerAngles.z - movePoints[firstIndex].eulerAngles.z) * (targetDist / distsBetween[i]));
                    break;
                }
            }
        }

        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetZRot));
    }
}