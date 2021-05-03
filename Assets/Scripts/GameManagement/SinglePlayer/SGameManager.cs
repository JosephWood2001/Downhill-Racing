using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SGameManager : GameManager
{
    [DisplayWithoutEdit]
    public int numberOfRacers = 8;
    [DisplayWithoutEdit]
    public RacerId[] racers; //player is always index 0
    public GameObject playerInputSender;
    public GameObject computerInputSender;

    public GameObject scoreboardPrefab;

    [DisplayWithoutEdit]
    public MapCatolog.Cup cup;
    [DisplayWithoutEdit]
    public int courseOfCupId = -1;
    [DisplayWithoutEdit]
    public int difficulty;

    void Start()
    {
        racers = new RacerId[numberOfRacers];
        Instantiate(scoreboardPrefab, GameObject.FindGameObjectWithTag("GameTypeManagers").transform);
    }

    public override void SetPlayer(GameObject kart)
    {
        racers[0] = new RacerId("player", kart, playerInputSender, true);
    }

    public void PopulateWithCPU()
    {
        GameObject[] karts = GameObject.FindGameObjectWithTag("KartCatolog").GetComponent<KartCatolog>().karts;
        for (int i = 1; i < numberOfRacers; i++)
        {
            racers[i] = new RacerId("CPU" + i, karts[Random.Range(0, karts.Length)],computerInputSender);
        }
    }

    public void BeginGame()
    {
        PopulateWithCPU();
        Camera.main.GetComponent<MainMenuCameraController>().enabled = false;
        Camera.main.GetComponent<CameraController>().enabled = true;
        SceneManager.UnloadSceneAsync("MainMenu");
        LoadNextMap();
    }

    //return false if this is the last map in cup
    public override bool LoadNextMap()
    {
        courseOfCupId++;
        if (courseOfCupId < cup.courses.Length)
        {
            SceneManager.LoadScene(cup.courses[courseOfCupId], LoadSceneMode.Additive);
            if (courseOfCupId > 0)
            {
                SceneManager.UnloadSceneAsync(loadedCourse);
            }
            loadedCourse = cup.courses[courseOfCupId];
            StartCoroutine("WaitForStartLineLoad");
        }
        else
        {
            //the cup is finished
            if (courseOfCupId > 0)
            {
                SceneManager.UnloadSceneAsync(loadedCourse);
            }
            StartCoroutine("WaitForResultsLoad",racers);
            return false;
        }
            
        
        
        
        

        return true;
    }

    public override void SeasonComplete()
    {
        SceneManager.UnloadSceneAsync("Results");
        SceneManager.LoadSceneAsync("MainMenu",LoadSceneMode.Additive);
        StartCoroutine("WaitToSetMenuToMapSelect");
        
    }

    IEnumerator WaitToSetMenuToMapSelect()
    {
        while (GameObject.FindGameObjectWithTag("MapSelectPos") == null)
        {
            yield return null;
        }
        Camera.main.GetComponent<CameraController>().enabled = false;
        Camera.main.GetComponent<MainMenuCameraController>().enabled = true;
        GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().CurrentMenu = 4;
        
    }

    IEnumerator WaitForResultsLoad(RacerId[] racers)
    {
        SceneManager.LoadScene("Results", LoadSceneMode.Additive);
        while (GameObject.FindGameObjectWithTag("ResultsManager") == null)
        {
            yield return null;
        }

        // Do anything after proper scene has been loaded
        GameObject.FindGameObjectWithTag("ResultsManager").GetComponent<ResultsManager>().PlayResults(racers);

    }

    IEnumerator WaitForStartLineLoad()
    {
        while (GameObject.FindGameObjectWithTag("StartLine") == null)
        {
            yield return null;
        }

        // Do anything after proper scene has been loaded
        GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>().InitializeRace(racers);

    }

    public override void RaceComplete()
    {
        GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<SScoreboard>().DisplayScoreboardMenu(racers);
        GameObject.FindGameObjectWithTag("PlayerHud").GetComponent<PlayerHud>().content.SetActive(false);
    }
}
