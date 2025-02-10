using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();    
    }

    private void Start()
    {
        gameManager.gameAudioSource.PlayOneShot(gameManager.audioClips[0]);    
    }

    #region choix de la main préférentiel pour jouer

    public void HandLeftGame()
    {
        gameManager.leftControl.SetActive(true);
        Destroy(gameManager.leftButton);
        Destroy(gameManager.rightControl);
        Destroy(gameManager.rightButton);
        StartCoroutine(gameManager.HandleTransition());
        gameManager.UpdateRanked();
    }

    public void HandRightGame()
    {
        gameManager.rightControl.SetActive(true);
        Destroy(gameManager.rightButton);
        Destroy(gameManager.leftButton);
        Destroy(gameManager.leftControl);
        StartCoroutine(gameManager.HandleTransition());
        gameManager.UpdateRanked();
    }

    #endregion
}
