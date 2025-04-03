using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Services.Authentication;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{
    #region variables
    public string leaderboardID;
    MaskManager maskManager;
    public LeaderboardManager leaderboardManager;
    public CredentialsManager credentialsManager;
    [SerializeField] FXUIManager fXUIManager;
    public AudioSource gameAudioSource;

    // Références aux éléments UI pour afficher le chronomètre et le score
    public TextMeshProUGUI chronoVisual, primeScoreVisual, rankOnline, statDotroidName, statNumberOfRuns, statBestScore, nameErrorText;

    // Références aux différents éléments UI
    public GameObject mainMenu, transitionScreen, leftZone, rightZone, switchLeft, switchRight, leftControl, rightControl, pauseGame, statBoard, settingsMenu, confirmDeleteMenu, updateNameMenu, phase, online, offline, blockLeaderBoard, blockProfileSettings, profileUIText, onlineUIText;
    [SerializeField] Button lbutton, rbutton;

    // Tableau des clips audio utilisés dans le jeu
    public AudioClip [] audioClips = new AudioClip[3];

    // Variables pour le score et le score prime
    public int score, primeScore, runs;
    // Timer pour suivre le temps écoulé
    public float timer;
    // Indicateur pour savoir si le jeu peut être joué
    public bool canPlay, OnPause;
    public string dotroidPlayerName;

    #endregion

    void Awake()
    {        
        //#if UNITY_EDITOR
        //leaderboardID = "Dotroid_Leaderboard_Dev";
        //#else
        leaderboardID = "Dotroid_Leaderboard"; 
        //#endif

        gameAudioSource = GetComponent<AudioSource>();
        maskManager = FindFirstObjectByType<MaskManager>();
        leaderboardManager = FindFirstObjectByType<LeaderboardManager>();
        fXUIManager = FindFirstObjectByType<FXUIManager>();
        //ClearAllSaves();
    }
    void Start()
    {
        LoadGame();
        if (!string.IsNullOrEmpty(dotroidPlayerName) && credentialsManager.unityLogin != null)
        {   
            credentialsManager.unityLogin.SetActive(false);
            mainMenu.SetActive(true);
        } 
    }
    void Update()
    {
        if(!canPlay) return;

        float delta = Time.deltaTime;
        if(!maskManager.isdead) HandleTimer(delta);
    }

    // Gère le chronomètre du jeu
    void HandleTimer(float delta)
    {
        if(maskManager.canCount)
        {
            timer += 1 * delta;
            score = (int)timer;
            if(primeScore < score) primeScore = score;
            chronoVisual.text = Mathf.Ceil(timer).ToString();
        }
    }

    public void HandleLetsPlay()
    {
        StartCoroutine(HandleTransition());
        maskManager.joystick = FindFirstObjectByType<FloatingJoystick>();
    }



    public IEnumerator HandleTransition()
    {
        gameAudioSource.PlayOneShot(audioClips[1]);
        yield return new WaitForSeconds(1f);
        transitionScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        mainMenu.SetActive(false);
        yield return new WaitForSeconds(1.7f);
        transitionScreen.SetActive(false);
        canPlay = true;
        maskManager.dotroidThemeAudioSource.enabled = true;
        LoadGame();
        UpdateRanked();
    }


    #region Gestion des sauvegardes

    public void SaveGame()
    {
        GameData gameData = new GameData
        {
            scoreData = primeScore,
            nameData = dotroidPlayerName,
            runData = runs,
            
        };

        string thiefJson = JsonUtility.ToJson(gameData);
        string filepath = Application.persistentDataPath + "/tentativeData.json";
        System.IO.File.WriteAllText(filepath, thiefJson);

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
            dotroidPlayerName = gameData.nameData;
            runs = gameData.runData;
            #if UNITY_EDITOR
            Debug.Log("Donnee chargees, ton callsign est: " + dotroidPlayerName);
            #endif
        }
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
                #if UNITY_EDITOR
                Debug.Log("Fichier supprimé : " + file);
                #endif
            }

            #if UNITY_EDITOR
            Debug.Log("Toutes les sauvegardes ont été effacées.");
            #endif
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors de la suppression des sauvegardes : " + e.Message);
        }
    }

    #region PauseMenu

    public void ToggleOnlineMode()
    {
        online.SetActive(true);
        blockLeaderBoard.SetActive(false);
        blockProfileSettings.SetActive(false);
        profileUIText.SetActive(true);
        onlineUIText.SetActive(true);
        offline.SetActive(false);
    }

    public void ToggleOfflineMode()
    {
        offline.SetActive(true);
        blockLeaderBoard.SetActive(true);
        blockProfileSettings.SetActive(true);
        profileUIText.SetActive(false);
        onlineUIText.SetActive(false);
        online.SetActive(false);
    }

    public void HandlePauseMenu()
    {
        if(!OnPause)
        {
            Time.timeScale = 0;
            pauseGame.SetActive(true);
            HandleStateSwitchControlButton();
            OnPause = true;
        } 
        else
        {
            Time.timeScale = 1;
            pauseGame.SetActive(false);
            leaderboardManager.leaderBoardUI.SetActive(false);
            statBoard.SetActive(false);
            OnPause = false;
        } 
    }
    public void OpenPlayerNameMenu()
    {
        AssignPlayerName();

        updateNameMenu.SetActive(true);
    }

    public void ClosePlayerNameMenu()
    {
        updateNameMenu.SetActive(false);
        nameErrorText.text = "";
    }

    public void OpenDeleteMenu()
    {
        settingsMenu.SetActive(true);
    }

    public void CloseDeleteMenu()
    {
        settingsMenu.SetActive(false);
        CloseDeleteConfirmationMenu();
    }

    public void OpenDeleteConfirmationMenu()
    {
        confirmDeleteMenu.SetActive(true);
    }

    public void CloseDeleteConfirmationMenu()
    {
        confirmDeleteMenu.SetActive(false);
    }

    public void UpdateRanked()
    {
        leaderboardManager.GetPlayerRank(dotroidPlayerName);
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
        statDotroidName.text = dotroidPlayerName;
    }

    public void HandleSwitchControl()
    {
        if(leftControl.activeSelf)
        {
            SkillManager skillManagerleft = FindFirstObjectByType<SkillManager>();
            if(skillManagerleft.isHorusActive)
            {
                skillManagerleft.isHorusActive = false;
                skillManagerleft.mainCamera.orthographicSize = 0.4f;
                skillManagerleft.horusButton.interactable = false;
                skillManagerleft.chain.SetActive(true);
                fXUIManager.HorusExit();
            }

            rightControl.SetActive(true);
            maskManager.joystick = rightControl.transform.GetChild(0).GetComponent<FloatingJoystick>();
            leftControl.SetActive(false);
            
        }
        else if(rightControl.activeSelf)
        {
            SkillManager skillManagerRight = FindFirstObjectByType<SkillManager>();
            if(skillManagerRight.isHorusActive)
            {
                skillManagerRight.isHorusActive = false;
                skillManagerRight.mainCamera.orthographicSize = 0.4f;
                skillManagerRight.horusButton.interactable = false;
                skillManagerRight.chain.SetActive(true);
                fXUIManager.HorusExit();
            }

            leftControl.SetActive(true);
            maskManager.joystick = leftControl.transform.GetChild(0).GetComponent<FloatingJoystick>();
            rightControl.SetActive(false);
        }
    }
    public void HandleStateSwitchControlButton()
    {
        if(leftControl.activeSelf)
        {
            switchLeft.SetActive(true);
            switchRight.SetActive(false);
        }
        else if(rightControl.activeSelf)
        {
            switchRight.SetActive(true);
            switchLeft.SetActive(false);
        }
    }

    #endregion

    public async void DeleteUnityAccount()
    {
        await AuthenticationService.Instance.DeleteAccountAsync();
        CloseDeleteConfirmationMenu();
        CloseDeleteMenu();
        ClearAllSaves();
        ExitGame();
    }

    #region SFX buttons
    public void playOpenSound()
    {
        gameAudioSource.PlayOneShot(audioClips[4]);
    }

    public void ClickSound()
    {
        gameAudioSource.PlayOneShot(audioClips[5]);
    }

    public void CancelSound()
    {
        gameAudioSource.PlayOneShot(audioClips[8]);
    }
    #endregion

    async void AssignPlayerName()
    {
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();

        credentialsManager.nameInput.text = RemoveAfterHash(playerName);
    }

    public async void UpdatePlayerName()
    {
        if (!HasCorrectCharacterCount(credentialsManager.nameInput.text))
        {
            nameErrorText.text = "Minimum 3 characters & maximum 15 characters allowed.";
        }
        if (ContainsSpaces(credentialsManager.nameInput.text))
        {
            nameErrorText.text = "No spaces allowed !";
        }
        else if (ContainsPunctuation(credentialsManager.nameInput.text))
        {
            nameErrorText.text = "No punctuation is allowed !";
        }

        if (!ContainsSpaces(credentialsManager.nameInput.text) && !ContainsPunctuation(credentialsManager.nameInput.text) && HasCorrectCharacterCount(credentialsManager.nameInput.text))
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(credentialsManager.nameInput.text);
                nameErrorText.text = "PlayerName updated successfully.";
            }
            catch (Exception ex)
            {
                Debug.LogError("Error updating PlayerName: " + ex);
            }
        }
    }

    bool HasCorrectCharacterCount(string input)
    {
        Debug.Log("Input is " + input + " and size is " + input.Length);
        return input.Length >= 3 && input.Length <= 15;
    }

    bool ContainsSpaces(string input)
    {
        return input.Contains(" ");
    }

    bool ContainsPunctuation(string input)
    {
        return Regex.IsMatch(input, @"[#.,!?;:'""(){}[\]\\/-]"); 
    }

    string RemoveAfterHash(string input)
    {
        int hashIndex = input.IndexOf('#');
        
        if (hashIndex == -1)
            return input;
        
        return input[..hashIndex];
    }

    class GameData
    {
        public int scoreData, runData;
        public string nameData;
    }

    #endregion
}
