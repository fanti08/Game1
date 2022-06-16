using UnityEngine;
using TMPro;

public class Pause: MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject countdownScreen;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isResumed;
    [SerializeField] private float countdownTime;
    [SerializeField] private Player player;

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        isResumed = false;
        pauseScreen.SetActive(true);
        countdownScreen.SetActive(false);
        countdownTime = 3;
    }

    public void ResumeGame()
    {
        countdownTime = 3;
        isResumed = true;
        pauseScreen.SetActive(false);
        countdownScreen.SetActive(true);
    }

    void Update()
    {
        if (isResumed) 
            countdownTime -= Time.unscaledDeltaTime;
        if (countdownTime <= 0 && isPaused)
            ResumeImmediate();
        countdownText.text = Mathf.Round(countdownTime).ToString().Replace("0", "GO!");
    }

    void ResumeImmediate()
    {
        Time.timeScale = 1;
        isPaused = false;
        isResumed = false;
        countdownScreen.SetActive(false);
    }
}

