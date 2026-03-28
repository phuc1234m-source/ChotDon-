using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public float score = 0f;
    public float scoreMultiplier = 1f;

    public TextMeshProUGUI scoreText;

    private RoadSpawner roadSpawner;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        roadSpawner = FindFirstObjectByType<RoadSpawner>();
    }

    void Update()
    {
        // Tăng điểm theo tốc độ scroll
        score += roadSpawner.scrollSpeed * scoreMultiplier * Time.deltaTime;

        scoreText.text = Mathf.FloorToInt(score).ToString();
    }

    public void ResetScore()
    {
        score = 0;
    }
}