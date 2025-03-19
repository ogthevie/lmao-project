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
        Destroy(gameManager.leftZone);
        Destroy(gameManager.rightZone);
    }

    public void HandRightGame()
    {
        gameManager.rightControl.SetActive(true);
        Destroy(gameManager.rightZone);
        Destroy(gameManager.leftZone);
    }

    #endregion
}
