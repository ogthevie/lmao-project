using UnityEngine;
using Unity.Services.Leaderboards;
using TMPro;
using Unity.Services.Leaderboards.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
    [SerializeField] RectTransform leaderBoardTableUI;
    [SerializeField] TextMeshProUGUI myRank, myName, myID, myScore;

    public async Task AddScoreToLeaderboard(int score)
    {
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
            if(playerScore != null)
            {
                int temprank = playerScore.Rank + 1;
                gameManager.rankOnline.text = temprank.ToString();
                gameManager.primeScore = (int)playerScore.Score;
                gameManager.primeScoreVisual.text = playerScore.Score.ToString();
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la récupération du rang du joueur : " + e.Message);
        }
    }

    public async void SaveOnline()
    {
        try
        {
            await AddScoreToLeaderboard(gameManager.primeScore);
            var myPlayerData = await LeaderboardsService.Instance.GetPlayerScoreAsync(gameManager.leaderboardID);
            
            gameManager.dotroidPlayerName = myPlayerData.PlayerName.ToString();    
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la sauvegarde du joueur : " + e.Message);            
        }
    }

    public void OpenLeaderBoardMenuAsync()
    {
        ClearLeaderboardsItems();
        FetchLeaderboardsData();
        FetchMyLeaderboardData();
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
        var options = new GetScoresOptions { Limit = 100 };
        LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(gameManager.leaderboardID, options);

        int nbLines = leaderboardScoresPage.Results.Count + 1;
        if(leaderboardScoresPage.Results.Count > 7)
        {
            int height = 62 + ((nbLines+1) * 52);
            leaderBoardTableUI.sizeDelta = new Vector2 (0, height);
        } 

        foreach (LeaderboardEntry leaderboardEntry in leaderboardScoresPage.Results)
        {
            Transform leaderboardItem = Instantiate(leaderboardsItem, leaderboardsContentParent);

            leaderboardItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = (leaderboardEntry.Rank + 1).ToString();
            leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = RemoveAfterHash(leaderboardEntry.PlayerName.ToString());
            leaderboardItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = leaderboardEntry.PlayerId.ToString();
            leaderboardItem.GetChild(3).GetComponent<TextMeshProUGUI>().text = leaderboardEntry.Score.ToString();
        }
    }

    private async void FetchMyLeaderboardData()
    {
        var myPlayerData = await LeaderboardsService.Instance.GetPlayerScoreAsync(gameManager.leaderboardID);
        myRank.text = (myPlayerData.Rank + 1).ToString();
        myName.text = RemoveAfterHash(myPlayerData.PlayerName.ToString());
        myID.text =  myPlayerData.PlayerId.ToString();
        myScore.text = myPlayerData.Score.ToString();

    }

    string RemoveAfterHash(string input)
    {
        int hashIndex = input.IndexOf('#');
        
        if (hashIndex == -1)
            return input;
        
        return input[..hashIndex];
    }

    public async void GetMyData()
    {
        var myPlayerData = await LeaderboardsService.Instance.GetPlayerScoreAsync(gameManager.leaderboardID);
        gameManager.statDotroidName.text = RemoveAfterHash(myPlayerData.PlayerName.ToString());
        gameManager.statBestScore.text = myPlayerData.Score.ToString();
    }
}