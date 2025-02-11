using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region variables
    MaskManager maskManager;
    LeaderboardManager leaderboardManager;
    public AudioSource gameAudioSource;

    // Références aux éléments UI pour afficher le chronomètre et le score
    public TextMeshProUGUI chronoVisual, primeScoreVisual, rankOnline;

    public TextMeshProUGUI statThiefName, statCallSign, statNumberOfRuns, statBestScore;

    // Références aux différents éléments UI
    public GameObject mainMenu, transitionScreen, leftButton, rightButton, leftControl, rightControl, pauseMenu, maze, setNameMenu, statBoard;

    // Tableau des clips audio utilisés dans le jeu
    public AudioClip [] audioClips = new AudioClip[3];
    [SerializeField] List <GameObject> walls = new List<GameObject>();

    // Variables pour le score et le score prime
    public int score, primeScore, runs;
    // Timer pour suivre le temps écoulé
    public float timer;
    // Indicateur pour savoir si le jeu peut être joué
    public bool canPlay, OnPause, wallOnFire, wallOnMove, isControllerConnected;
    [SerializeField] Color dynamicWall = new Color(0.90f, 0.09f, 0.05f);
    [SerializeField] Color staticWall = new Color(0.04f, 0.1f, 0.30f);
    public string thiefPlayerName;

    #endregion

    void Awake()
    {
        gameAudioSource = GetComponent<AudioSource>();
        maskManager = FindFirstObjectByType<MaskManager>();
        leaderboardManager = GetComponent<LeaderboardManager>();
        //ClearAllSaves();
        LoadGame();
        AddMazeChildrenToWalls();
        if(!string.IsNullOrEmpty(thiefPlayerName))
        {
            Destroy(setNameMenu);
            mainMenu.SetActive(true);
        }
    }
    
    void Update()
    {
        if(!canPlay) return;

        float delta = Time.deltaTime;
        if(!maskManager.isdead) HandleTimer(delta);
        //Debug.Log(timer);
    }

    /*private void FixedUpdate()
    {
        CheckForGamePad();    
    }*/

    // Gère le chronomètre du jeu
    void HandleTimer(float delta)
    {
        if(maskManager.canCount)
        {
            timer += 1 * delta;
            score = (int)timer;
            if(primeScore < score) primeScore = score;
            chronoVisual.text = Mathf.Ceil(timer).ToString();


            if(!wallOnMove && score >= 50f) 
            {
                wallOnMove = true;
                WallMoveTheme();
            }
            if(!wallOnFire && score >= 200f) HandleWallOnFire();

        }
    }

    void AddMazeChildrenToWalls()
    {
        if (maze != null)
        {
            bool isFirstChild = true;
            foreach (Transform child in maze.transform)
            {
                if (isFirstChild)
                {
                    isFirstChild = false;
                    continue;
                }
                walls.Add(child.gameObject);
            }
        }
    }

    public void HandleLetsPlay()
    {
        StartCoroutine(HandleTransition());
        maskManager.joystick = FindFirstObjectByType<FixedJoystick>();
    }



    public IEnumerator HandleTransition()
    {
        UpdateRanked();
        gameAudioSource.PlayOneShot(audioClips[1]);
        yield return new WaitForSeconds(1f);
        transitionScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        mainMenu.SetActive(false);
        yield return new WaitForSeconds(1.7f);
        transitionScreen.SetActive(false);
        canPlay = true;
        LoadGame();
        //ReloadMainTheme();
    }

    public void WallMoveTheme()
    {
        gameAudioSource.Play();
        gameAudioSource.loop = true;
    }

    public void ResetWall()
    {
        foreach (GameObject wall in walls)
        {
            var material = wall.GetComponent<MeshRenderer>().materials[0];
            material.SetColor("_EmissionColor", staticWall);
            Vector3 initPosition = wall.GetComponent<OscillationManager>().basePosition;
            wall.transform.position = initPosition;
        }
        wallOnFire = wallOnMove = false;
    }

    public void HandleWallOnFire()
    {
        foreach (GameObject wall in walls)
        {
            var material = wall.GetComponent<MeshRenderer>().materials[0];
            material.SetColor("_EmissionColor", dynamicWall);
        }
        wallOnFire = true;
    }

    public void HandlePauseMenu()
    {
        if(!OnPause)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            OnPause = true;
        } 
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            leaderboardManager.leaderBoardUI.SetActive(false);
            statBoard.SetActive(false);
            OnPause = false;
        } 
    }

    #region Gestion des sauvegardes

    public void SaveGame()
    {
        GameData gameData = new GameData
        {
            scoreData = primeScore,
            nameData = thiefPlayerName,
            runData = runs,
            
        };

        string thiefJson = JsonUtility.ToJson(gameData);
        string filepath = Application.persistentDataPath + "/tentativeData.json";
        System.IO.File.WriteAllText(filepath, thiefJson);
        leaderboardManager.AddScoreToLeaderboard(thiefPlayerName, primeScore);
    }

    // Charge le jeu depuis les sauvegardes
    public void LoadGame()
    {
        string filePath = Application.persistentDataPath + "/tentativeData.json";
        if(System.IO.File.Exists(filePath))
        {
            string thiefJson = System.IO.File.ReadAllText(filePath);
            GameData gameData = JsonUtility.FromJson<GameData>(thiefJson);
            
            primeScore = gameData.scoreData;
            thiefPlayerName = gameData.nameData;
            runs = gameData.runData;
            Debug.Log("Donnee chargees");
        }
    }

    public void UpdateRanked()
    {
        leaderboardManager.GetPlayerRank(thiefPlayerName);
    }

    public void OpenStatsBoard()
    {
        FetchStatBoardData();
        statBoard.SetActive(true);
    }

    public void CloseStatBoard()
    {
        statBoard.SetActive(false);
    }

    public void FetchStatBoardData()
    {
        leaderboardManager.GetMyData();
        statNumberOfRuns.text = runs.ToString();
        statThiefName.text = thiefPlayerName;

    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Arrête le jeu dans la version Editor
        #else
        Application.Quit(); // Arrête le jeu dans la version buildé du jeu (je ne sais pas si c'est compréhensible :)
        #endif
    }


    // Efface toutes les sauvegardes
    public void ClearAllSaves()
    {
        // Chemin du dossier de sauvegarde
        string saveFolderPath = Application.persistentDataPath;

        try
        {
            // Récupère tous les fichiers JSON dans le dossier de sauvegarde
            string[] saveFiles = System.IO.Directory.GetFiles(saveFolderPath, "*.json");

            // Supprime chaque fichier
            foreach (string file in saveFiles)
            {
                System.IO.File.Delete(file);
                Debug.Log("Fichier supprimé : " + file);
            }

            Debug.Log("Toutes les sauvegardes ont été effacées.");
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors de la suppression des sauvegardes : " + e.Message);
        }
    }
    /*private void CheckForGamePad()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if(joystickNames.Length == 0)
        {
            //Debug.Log("Manette déconnectée");
            //needGamepad.enabled = true;
            //Time.timeScale = 0;
            isControllerConnected = false;            
        }

        if (joystickNames.Length > 0 && !string.IsNullOrEmpty(joystickNames[0]))
        {
            if (!isControllerConnected)
            {
                Debug.Log("Manette connectée : " + joystickNames[0]);
                isControllerConnected = true;
            }
        }
        else
        {
            if (isControllerConnected)
            {
                //Debug.Log("Manette déconnectée");
                isControllerConnected = false;
            }
        }
    }*/


    class GameData
    {
        public int scoreData, runData;
        public string nameData;
    }

    #endregion
}
