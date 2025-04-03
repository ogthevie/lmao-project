using UnityEngine;
using System.Collections.Generic;

public class FirstStageManager : StageManager
{
    [SerializeField] List <GameObject> walls = new List<GameObject>(); 
    [SerializeField] Color dynamicWall = new Color(0.90f, 0.09f, 0.05f);
    [SerializeField] Color staticWall = new Color(0.04f, 0.1f, 0.30f);

    protected override void Awake()
    {
        stageIndex = 0;
        AddMazeChildrenToWalls();
        base.Awake();
    }

    protected override void Update()
    {
        if(gameManager.timer >= 50 && !wallOnMove) WallMove();
        if(gameManager.timer >= 120 && !wallOnFire) WallFire();
    }

    void AddMazeChildrenToWalls()
    {

        bool isFirstChild = true;
        foreach (Transform child in this.transform)
        {
            if (isFirstChild)
            {
                isFirstChild = false;
                continue;
            }
            walls.Add(child.gameObject);
        }
    }

    public void WallMove()
    {
        gameManager.phase.SetActive(true);
        stageAudiosource.Play();
        // Activer les oscillations pour tous les murs
        foreach (GameObject wall in walls)
        {
            var oscillationManager = wall.GetComponent<OscillationManager>();
            if (oscillationManager != null)
            {
                oscillationManager.StartOscillating();
            }
        }
        wallOnMove = true;
    }

    public void ResetWall()
    {
        foreach (GameObject wall in walls)
        {
            var material = wall.GetComponent<MeshRenderer>().materials[0];
            material.SetColor("_EmissionColor", staticWall);
            OscillationManager oscillationManager = wall.GetComponent<OscillationManager>();
            oscillationManager.StopOscillating();
        }
        wallOnMove = false;
        wallOnFire = false;
    }

    public void WallFire()
    {
        foreach (GameObject wall in walls)
        {
            var material = wall.GetComponent<MeshRenderer>().materials[0];
            material.SetColor("_EmissionColor", dynamicWall);
        }
        wallOnFire = true;
    }

}
