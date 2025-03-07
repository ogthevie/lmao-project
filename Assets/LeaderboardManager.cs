using UnityEngine;
using Unity.Services.Leaderboards;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Leaderboards.Models;
using Newtonsoft.Json;

public class LeaderboardManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
    }

    [SerializeField] GameManager gameManager;
    public GameObject leaderBoardUI;
    [SerializeField] private Transform leaderboardsItem;
    [SerializeField] private Transform leaderboardsContentParent;

    public async void InitializeLeaderboards()
    {
        await UnityServices.InitializeAsync();
    }

    public async void AddScoreToLeaderboard(int score)
    {
        Debug.Log(gameManager.leaderboardID);
        try
        {
            var playerScore = await LeaderboardsService.Instance.AddPlayerScoreAsync(gameManager.leaderboardID, score);
            Debug.Log("Score added to leaderboard successfully. " + JsonConvert.SerializeObject(playerScore));
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
            var playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync(gameManager.leaderboardID);
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


    public void OpenLeaderBoardMenuAsync()
    {
        ClearLeaderboardsItems();
        FetchLeaderboardsData();
        leaderBoardUI.SetActive(true);
    }

    public void CloseLeaderBoardMenu()
    {
        leaderBoardUI.SetActive(false);
    }

    public void ClearLeaderboardsItems()
    {
        foreach (Transform transform in leaderboardsContentParent)
        {
            if (transform != null && transform.gameObject != null)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    private async void FetchLeaderboardsData()
    {
        LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(gameManager.leaderboardID);

        foreach (LeaderboardEntry leaderboardEntry in leaderboardScoresPage.Results)
        {
            Transform leaderboardItem = Instantiate(leaderboardsItem, leaderboardsContentParent);

            leaderboardItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = (leaderboardEntry.Rank + 1).ToString();
            leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = leaderboardEntry.PlayerName.ToString();
            leaderboardItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = leaderboardEntry.Score.ToString();
        }
    }

    public async void GetMyData()
    {
        var myPlayerData = await LeaderboardsService.Instance.GetPlayerScoreAsync(gameManager.leaderboardID);
        gameManager.statThiefName.text = myPlayerData.PlayerName.ToString();
        gameManager.statBestScore.text = myPlayerData.Score.ToString();
    }
}