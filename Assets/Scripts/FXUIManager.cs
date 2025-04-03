using UnityEngine;

public class FXUIManager : MonoBehaviour
{
    public GameObject horusFx;

    #region Horus
    public void HorusEnter()
    {
        horusFx.SetActive(true);
        horusFx.GetComponent<Animation>().Play("HorusEnter");
    }

    public void HorusExit()
    {
        horusFx.GetComponent<Animation>().Play("HorusExit");
    }
    #endregion
}
