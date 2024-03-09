using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI recordText; // displays max score and max time of all generations
    public float maxTime = 0;
    public int maxScore = 0;

    public int score;

    public static ScoreManager instance;
    private void Awake() => instance = this;

    private void UpdateText()
    {
        scoreText.text = score.ToString();
    }
    public void SetScore(int n)
    {
        score = n;
        UpdateText();
    }
    public void IncreaseScore(int n) => SetScore(score + n);

    public void Restart()
    {
        SetScore(0);
    }
    public void UpdateRecord(int score, float time)
    {
        if (maxScore < score) maxScore = score;
        if (maxTime < time) maxTime = time;

        recordText.text = $"Max score: {maxScore}\nMax time: {System.Math.Round(maxTime, 2)}";
    }
}
