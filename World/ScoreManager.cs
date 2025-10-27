using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text YourScoreText;
    public int FinalScore = 0;

    void Start()
    {
        FinalScore = PlayerPrefs.GetInt("FinalPlayerScore", 0);

        ShowFinalScore();
    }

    void ShowFinalScore()
    {
        YourScoreText.text = FinalScore.ToString();
    }
}
