using UnityEngine;
using Unity.Services.Leaderboards;
using System.Collections.Generic;

[DefaultExecutionOrder(-10)]
public class LeaderboardManager : MonoBehaviour
{
    public bool servicesInitialized = false;
    
    [SerializeField] GameManager gameManager;

    private readonly string leaderboardID = "Thief_Leaderboard_Dev";

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
            Debug.Log($"{playerName} est classé {gameManager.rankOnline.text} ème.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la récupération du rang du joueur : " + e.Message);
        }
    }

    /*public async void DisplayLeaderboard()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync("highscore_leaderboard");
            foreach (var score in scores.Results)
            {
                // Afficher les scores
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de l'affichage du tableau de classement : " + e.Message);
        }
    }*/
}