using UnityEngine;
using Unity.Services.Leaderboards;
using System.Collections.Generic;
using TMPro;

[DefaultExecutionOrder(-10)]
public class LeaderboardManager : MonoBehaviour
{
    public bool servicesInitialized = false;
    
    [SerializeField] GameManager gameManager;

    private readonly string leaderboardID = "Thief_Leaderboard_Dev";
    [SerializeField] TextMeshProUGUI [] playersNames;
    [SerializeField] TextMeshProUGUI [] playersScores;
    [SerializeField] GameObject leaderBoardUI;

    public async void AddScoreToLeaderboard(string playerName, int score)
    {
        try
        {
            string playerId = playerName;
            var metadata = new Dictionary<string, object>
            {
                { "playerId", playerId }
            };
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, score, new AddPlayerScoreOptions { Metadata = metadata });
            Debug.Log("Score added to leaderboard successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de l'ajout du score au tableau de classement : " + e.Message);
        }
    }

    public async void GetPlayerRank(string playerName)
    {
        try
        {
            var playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardID);
            int temprank = playerScore.Rank + 1;
            gameManager.rankOnline.text = temprank.ToString();
            gameManager.primeScore = (int)playerScore.Score;
            gameManager.primeScoreVisual.text = playerScore.Score.ToString();
            Debug.Log($"{playerName} est classé {gameManager.rankOnline.text} ème.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la récupération du rang du joueur : " + e.Message);
        }
    }


    public void OpenLeaderBoardMenu()
    {
        leaderBoardUI.SetActive(true);
        DisplayLeaderboard();
    }

    public void CloseLeaderBoardMenu()
    {
        leaderBoardUI.SetActive(false);
    }

    public async void DisplayLeaderboard()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);

            int count = Mathf.Min(scores.Results.Count, 10);
            
            for (int i = 0; i < count; i++)
            {
                if(scores.Results[i] == null) return;
                playersNames[i].text = scores.Results[i].Metadata;
                playersScores[i].text = scores.Results[i].Score.ToString();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de l'affichage du tableau de classement : " + e.Message);
        }
    }
}