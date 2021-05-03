using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CGameManager : GameManager
{
    [DisplayWithoutEdit]
    public int numberOfRacers = 8;
    public RacerId[] racers; //player is always index 0
    public GameObject playerInputSender;
    public GameObject computerInputSender;

    public GameObject scoreboardPrefab;

    /** 0: random
     *  1: player selected
     *  2: play through cup
     */
    [DisplayWithoutEdit]
    public int mapSelectMode = 0;
    [DisplayWithoutEdit]
    public MapCatolog.Cup cup;//only applicable in mapSelectMode 2
    public int courseOfCupId = -1;//only applicable in mapSelectMode 2
    [DisplayWithoutEdit]
    public int numOfSeasonRaces = 4;//not applicable in mapSelectMode 2
    private int racesLeft;

    void Start()
    {
        Instantiate(scoreboardPrefab, GameObject.FindGameObjectWithTag("GameTypeManagers").transform);
    }

    public void Initialize(int numberOfRacers, int mapSelectMode, int numOfSeasonRaces/*can be whatever if mapSelectMode is 2*/)
    {
        this.numberOfRacers = numberOfRacers;
        this.mapSelectMode = mapSelectMode;
        this.numOfSeasonRaces = numOfSeasonRaces;
        racesLeft = this.numOfSeasonRaces;
        racers = new RacerId[numberOfRacers];
    }

    public void PopulateWithCPU()
    {
        GameObject[] karts = GameObject.FindGameObjectWithTag("KartCatolog").GetComponent<KartCatolog>().karts;
        for (int i = 1; i < numberOfRacers; i++)
        {
            racers[i] = new RacerId("CPU" + i, karts[Random.Range(0, karts.Length)], computerInputSender);
        }
    }

    public void BeginSession()
    {
        PopulateWithCPU();

        if (mapSelectMode == 0)//if map select is random load blank menu AND begin season
        {
            GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().CurrentMenu = 0;
            BeginSeason();
        }else if (mapSelectMode == 1)//if map select is player selected load respective menu
        {
            GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().CurrentMenu = 0;
            LoadNextMap();
        }
        else if(mapSelectMode == 2)//if map select is selected cup load respective menu
        {
            GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().CurrentMenu = 8;
        }


    }

    void PlayerSelectedChooseCourse()
    {
        if (!loadedCourse.Equals(""))
        {
            SceneManager.UnloadSceneAsync(loadedCourse);

        }

        if (GameObject.FindGameObjectWithTag("MapSelectPos") == null)
        {
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
            StartCoroutine("WaitToSetMenuToPlayerSelected");
        }
        else
        {
            GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().CurrentMenu = 9;
        }
    }

    public void BeginSeason()
    {
        Camera.main.GetComponent<MainMenuCameraController>().enabled = false;
        Camera.main.GetComponent<CameraController>().enabled = true;
        SceneManager.UnloadSceneAsync("MainMenu");
        LoadNextMap();
    }


    public override bool LoadNextMap()
    {
        if (!loadedCourse.Equals(""))
        {
            SceneManager.UnloadSceneAsync(loadedCourse);
        }
        

        if (mapSelectMode == 0)//Random
        {
            if (racesLeft > 0)
            {
                string course = "";
                MapCatolog mapCatolog = GameObject.FindGameObjectWithTag("MapCatolog").GetComponent<MapCatolog>();
                int amountOfCourses = 0;
                for (int i = 0; i < mapCatolog.cups.Length; i++)
                {
                    amountOfCourses += mapCatolog.cups[i].courses.Length;
                }
                int courseIndex = Random.Range(0,amountOfCourses);

                for (int i = 0; i < mapCatolog.cups.Length; i++)
                {
                    if (courseIndex >= mapCatolog.cups[i].courses.Length)
                    {
                        courseIndex -= mapCatolog.cups[i].courses.Length;
                    }
                    else
                    {
                        course = mapCatolog.cups[i].courses[courseIndex];
                        break;
                    }
                }
                loadedCourse = course;
                SceneManager.LoadSceneAsync(course, LoadSceneMode.Additive);
                StartCoroutine("WaitForStartLineLoad");

                racesLeft--;
                return true;
            }
            else//No more races left, go to results
            {
                loadedCourse = "";
                StartCoroutine("WaitForResultsLoad", racers);
                return false;
            }
        }else if(mapSelectMode == 1)//player selected
        {
            if (racesLeft > 0)
            {
                PlayerSelectedChooseCourse();

                racesLeft--;
                return true;
            }
            else//No more races left, go to results
            {
                loadedCourse = "";
                StartCoroutine("WaitForResultsLoad", racers);
                return false;
            }
        }
        else if(mapSelectMode == 2)//selected cup
        {
            if (courseOfCupId < cup.courses.Length - 1)
            {
                courseOfCupId++;
                loadedCourse = cup.courses[courseOfCupId];
                SceneManager.LoadSceneAsync(loadedCourse, LoadSceneMode.Additive);
                StartCoroutine("WaitForStartLineLoad");
                return true;
            }
            else//No more races left, go to results
            {
                loadedCourse = "";
                StartCoroutine("WaitForResultsLoad", racers);
                return false;
            }
        }

        return true;
    }

    public void LoadPlayerSelected()
    {
        Camera.main.GetComponent<MainMenuCameraController>().enabled = false;
        Camera.main.GetComponent<CameraController>().enabled = true;
        SceneManager.UnloadSceneAsync("MainMenu");
        loadedCourse = cup.courses[courseOfCupId];
        SceneManager.LoadSceneAsync(loadedCourse, LoadSceneMode.Additive);
        StartCoroutine("WaitForStartLineLoad");
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

    public override void SetPlayer(GameObject kart)
    {
        racers[0] = new RacerId("player", kart, playerInputSender, true);
    }

    public override void RaceComplete()
    {
        GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<SScoreboard>().DisplayScoreboardMenu(racers);
        GameObject.FindGameObjectWithTag("PlayerHud").GetComponent<PlayerHud>().content.SetActive(false);
    }

    public override void SeasonComplete()
    {
        SceneManager.UnloadSceneAsync("Results");
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        StartCoroutine("WaitToSetMenuToCustomGameMenu");
    }

    IEnumerator WaitToSetMenuToCustomGameMenu()
    {
        while (GameObject.FindGameObjectWithTag("MapSelectPos") == null)
        {
            yield return null;
        }
        Camera.main.GetComponent<CameraController>().enabled = false;
        Camera.main.GetComponent<MainMenuCameraController>().enabled = true;
        GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().CurrentMenu = 6;

    }

    IEnumerator WaitToSetMenuToPlayerSelected()
    {
        while (GameObject.FindGameObjectWithTag("MapSelectPos") == null)
        {
            yield return null;
        }
        Camera.main.GetComponent<CameraController>().enabled = false;
        Camera.main.GetComponent<MainMenuCameraController>().enabled = true;
        GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().CurrentMenu = 9;

    }

}
