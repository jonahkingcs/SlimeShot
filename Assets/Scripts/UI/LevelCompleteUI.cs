using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{
    public GameObject panel;     // Assign the LevelCompletePanel in Inspector
    public SlimeController slime; // Assign your slime in Inspector

    public void Show()
    {
        if (slime)
        {
            slime.enabled = false; // stop Update()
            var rb = slime.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        if (panel) panel.SetActive(true);
        Time.timeScale = 0f; // pause game
    }

    public void RestartLevel()
    {
        Debug.Log("RestartLevel clicked");
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}