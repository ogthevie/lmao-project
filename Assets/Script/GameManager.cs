using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    MaskManager maskManager;
    public AudioSource gameAudioSource;
    public TextMeshProUGUI chronoVisual, primeScoreVisual, tentativeNb;
    public float timer;
    public GameObject mainMenu, transitionScreen, leftButton, rightButton, leftControl, rightControl;
    public AudioClip [] audioClips = new AudioClip[3];
    public int score, primeScore;
    public bool canPlay;

    void Awake()
    {
        gameAudioSource = GetComponent<AudioSource>();
        maskManager = FindFirstObjectByType<MaskManager>();
        //ClearAllSaves();
        LoadTentative();
    }

    void Start()
    {
        gameAudioSource.PlayOneShot(audioClips[0]);
    }
    
    void Update()
    {
        if(!canPlay) return;

        float delta = Time.deltaTime;
        if(!maskManager.isdead) HandleTimer(delta);
        //Debug.Log(timer);
    }

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

    public void HandLeftGame()
    {
        leftControl.SetActive(true);
        Destroy(leftButton);
        Destroy(rightControl);
        Destroy(rightButton);
        StartCoroutine(HandleTransition());
    }

    public void HandRightGame()
    {
        rightControl.SetActive(true);
        Destroy(rightButton);
        Destroy(leftButton);
        Destroy(leftControl);
        StartCoroutine(HandleTransition());
    }

    public void HandleLetsPlay()
    {
        StartCoroutine(HandleTransition());
        maskManager.joystick = FindFirstObjectByType<FixedJoystick>();
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
        ReloadMainTheme();
    }

    public void ReloadMainTheme()
    {
        gameAudioSource.Play();
        gameAudioSource.loop = true;
    }

    public void SaveTentative()
    {
        GameData gameData = new GameData
        {
            scoreData = primeScore
        };

        string thiefJson = JsonUtility.ToJson(gameData);
        string filepath = Application.persistentDataPath + "/tentativeData.json";
        System.IO.File.WriteAllText(filepath, thiefJson);
        Debug.Log(gameData.scoreData);
    }

    public void LoadTentative()
    {
        string filePath = Application.persistentDataPath + "/tentativeData.json";
        if(System.IO.File.Exists(filePath))
        {
            string thiefJson = System.IO.File.ReadAllText(filePath);
            GameData gameData = JsonUtility.FromJson<GameData>(thiefJson);
            
            primeScore = gameData.scoreData;
        }
        primeScoreVisual.text = primeScore.ToString();

    }

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


    class GameData
    {
        public int scoreData;
    }
}
