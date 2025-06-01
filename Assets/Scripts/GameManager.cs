using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint; // y & z fixed, only x changes
    public float minX = -3f;
    public float maxX = 7f;

    [Header("Turn Settings")]
    public int ballsPerPlayer = 5;
    public int totalPlayers = 2;

    private int currentPlayer = 0;
    private int[] playerScores;
    private int ballsUsed = 0;

    [Header("UI Elements")]
    public Text turnText;
    public Text[] playerScoreTexts; // assign in order (Player 1 -> index 0, etc.)

    private GameObject currentBall;

    void Start()
    {
        playerScores = new int[totalPlayers];
        UpdateUI();
        SpawnBall();
    }

    void SpawnBall()
    {
        if (ballsUsed >= ballsPerPlayer)
        {
            ballsUsed = 0;
            currentPlayer++;

            if (currentPlayer >= totalPlayers)
            {
                turnText.text = "Game Over!";
                return;
            }
        }

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, spawnPoint.position.y, spawnPoint.position.z);

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        BasketballDragShoot dragShoot = currentBall.GetComponent<BasketballDragShoot>();
        if (dragShoot != null)
        {
            dragShoot.OnBallShot = OnBallShot;
            dragShoot.OnGoalScored = () => IncreaseScore(currentPlayer);
        }

        UpdateUI();
    }

    void OnBallShot()
    {
        StartCoroutine(WaitAndSpawnNext());
    }

    IEnumerator WaitAndSpawnNext()
    {
        yield return new WaitForSeconds(1.5f); // Wait after shot
        ballsUsed++;
        SpawnBall();
    }

    void IncreaseScore(int playerIndex)
    {
        playerScores[playerIndex]++;
        UpdateUI();
    }

    void UpdateUI()
    {
        turnText.text = $"Player {currentPlayer + 1}'s Turn";

        for (int i = 0; i < playerScoreTexts.Length && i < playerScores.Length; i++)
        {
            playerScoreTexts[i].text = $"Player {i + 1} Score: {playerScores[i]}";
        }
    }
}
