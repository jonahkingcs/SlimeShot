using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnTouch : MonoBehaviour
{
    [Tooltip("Optional: assign your slime to be explicit about what counts.")]
    public SlimeController slime; // leave null to restart on ANY slime found

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the player slime
        var rb = other.attachedRigidbody;
        var hitSlime = rb ? rb.GetComponent<SlimeController>() : null;

        if (hitSlime && (slime == null || hitSlime == slime))
        {
            // Ensure time scale is normal (in case it was paused earlier)
            Time.timeScale = 1f;

            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}