using System.Collections;
using UnityEngine;

public class CompleteManager : MonoBehaviour
{
    [SerializeField] GameObject success;

    //Méthode native appelée lorsqu'un autre collider entre dans le trigger, ne nécessite pas de référencement
    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet entrant appartient à la couche 3 qui est celle du joueur
        if(other.gameObject.layer == 3)
        {
            StartCoroutine (CompleteLevel());
        }
    }

    // Coroutine pour gérer la complétion du niveau, revoir cette coroutine
    IEnumerator CompleteLevel()
    {
        Time.timeScale = 0;
        success.SetActive(true);
        yield return new WaitForSeconds (30f);
        Application.Quit();
    }
}
