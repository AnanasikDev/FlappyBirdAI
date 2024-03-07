using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
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
}
