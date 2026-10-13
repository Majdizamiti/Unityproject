using UnityEngine;

public class ScoreControl : MonoBehaviour
{
    [SerializeField] GameObject scoreBox;            // current score text
    [SerializeField] GameObject bestScoreBox;        // CHANGED: optional, a second text for the best score

    public static int totalScore = 0;
    public static int bestScore = 0;                 // CHANGED

    const string BestScoreKey = "BestScore";         // CHANGED: name under which the value is saved

    TMPro.TMP_Text scoreText;
    TMPro.TMP_Text bestScoreText;                    // CHANGED

    void Start()
    {
        totalScore = 0;                                          // each run starts at 0
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);         // CHANGED: load the saved best score

        scoreText = scoreBox.GetComponent<TMPro.TMP_Text>();
        if (bestScoreBox != null)
            bestScoreText = bestScoreBox.GetComponent<TMPro.TMP_Text>();
    }

    void Update()
    {
        // CHANGED: new record, update it and save it
        if (totalScore > bestScore)
        {
            bestScore = totalScore;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
        }

        scoreText.text = "SCORE: " + totalScore;
        if (bestScoreText != null)
            bestScoreText.text = "BEST: " + bestScore;           // CHANGED
    }
}