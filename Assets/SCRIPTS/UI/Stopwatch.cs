using TMPro;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float currentTime;

    void Update()
    {
        currentTime += Time.deltaTime;
        text.text = currentTime.ToString("F2").Replace(",", ".");
    }

    /*/public float GetCurrentTime() { return currentTime; }
    public void SetCurrentTime(float time) { currentTime = time; }
    public void AddTime(float timeToAdd) { currentTime += timeToAdd; }
    public void SubtractTime(float timeToSubtract) { currentTime -= timeToSubtract; }/*/
}
