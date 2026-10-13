using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [SerializeField] GameOverScreen gameOverScreen;      // drag the panel that has the GameOverScreen script
    [SerializeField] float fallDistance = 10f;           // how far below the start height counts as "fallen"
    [SerializeField] bool useFixedHeight = false;        // tick to use a fixed Y value instead
    [SerializeField] float fixedFallY = -10f;            // the fixed Y value, if enabled

    float startY;
    bool triggered;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        if (triggered) return;

        float limit = useFixedHeight ? fixedFallY : startY - fallDistance;

        if (transform.position.y < limit)
        {
            triggered = true;

            if (gameOverScreen != null)
                gameOverScreen.Show();     // shows the panel, pauses the game, frees the cursor
            else
                Debug.LogError("FallDetector: Game Over Screen is not assigned in the Inspector!", this);
        }
    }
}