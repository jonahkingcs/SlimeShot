using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI label;   // assign TimerText in Inspector

    [Header("Behaviour")]
    public bool autoStart = true;   // start counting when scene starts

    float elapsed;                  // seconds
    bool running;

    void Start()
    {
        if (autoStart) StartTimer();
        if (label) label.text = "00:00";
    }

    void Update()
    {
        if (!running) return;
        elapsed += Time.deltaTime;          // respects pauses (timeScale=0)
        if (label) label.text = Format(elapsed);
    }

    public void StartTimer()
    {
        elapsed = 0f;
        running = true;
        if (label) label.text = "00:00";
    }

    public void StopTimer()
    {
        running = false;
        if (label) label.text = Format(elapsed); // ensure final value is shown
    }

    public float GetElapsedSeconds() => elapsed;

    static string Format(float t)
    {
        int total = Mathf.FloorToInt(t);
        int m = total / 60;
        int s = total % 60;
        return $"{m:00}:{s:00}";
    }
}