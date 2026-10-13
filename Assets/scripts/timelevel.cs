using System.Collections;
using UnityEngine;

public class TimeLevel : MonoBehaviour
{
    [SerializeField] GameObject timeBox;
    [SerializeField] int timeLeft = 30;

    [SerializeField] GameObject levelBGM;
    [SerializeField] AudioSource timeUpSound;
    [SerializeField] GameObject playerControl;
    [SerializeField] GameOverScreen gameOverScreen;
    [SerializeField] GameObject timeUpText;          // CHANGED: the text that must appear only at 0

    TMPro.TMP_Text timeText;
    bool timeUp;

    void Start()
    {
        if (timeUpText != null) timeUpText.SetActive(false);   // CHANGED: hidden at the start

        timeText = timeBox.GetComponent<TMPro.TMP_Text>();
        timeText.text = "TIME LEFT: " + timeLeft;
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft -= 1;
            timeText.text = "TIME LEFT: " + timeLeft;
        }

        TimeUp();
    }

    void TimeUp()
    {
        if (timeUp) return;
        timeUp = true;

        if (timeUpText != null) timeUpText.SetActive(true);    // CHANGED: shown only when the time reaches 0

        if (levelBGM != null) levelBGM.SetActive(false);
        if (timeUpSound != null) timeUpSound.Play();

        if (playerControl != null)
        {
            PlayerControls controls = playerControl.GetComponent<PlayerControls>();
            if (controls != null) controls.enabled = false;

            Animator anim = playerControl.GetComponentInChildren<Animator>();
            if (anim != null) anim.CrossFade("breath", 0.15f);
        }

        if (gameOverScreen != null)
            gameOverScreen.Show();
        else
            Debug.LogError("TimeLevel: Game Over Screen is not assigned!", this);
    }
}