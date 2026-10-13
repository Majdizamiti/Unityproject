using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    [SerializeField] GameObject playerControl;
    [SerializeField] AudioSource levelJingle;
    [SerializeField] GameObject levelBGM;
    [SerializeField] GameOverScreen gameOverScreen;   // CHANGED: drag the panel that has the GameOverScreen script
    [SerializeField] string playerTag = "Player";

    bool finished;

    void OnTriggerEnter(Collider other)
    {
        if (finished) return;

        bool isPlayer = other.CompareTag(playerTag) ||
                        other.GetComponentInParent<CharacterController>() != null;
        if (!isPlayer) return;

        finished = true;

        if (playerControl != null)
        {
            PlayerControls controls = playerControl.GetComponent<PlayerControls>();
            if (controls != null) controls.enabled = false;
        }

        if (levelBGM != null) levelBGM.SetActive(false);
        if (levelJingle != null) levelJingle.Play();

        if (gameOverScreen != null)
        {
            gameOverScreen.Show();   // CHANGED: shows the panel and pauses the game
        }
        else
        {
            Debug.LogError("LevelFinish: Game Over Screen is not assigned in the Inspector!", this);
        }
    }
}
