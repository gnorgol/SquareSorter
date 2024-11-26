using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    info,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    private GameState gameState = GameState.MainMenu;
    public int level = 1;
    public int score = 0;
    public float Timer = 60;
    [SerializeField]
    private float TimeLeft;
    public float MaxSizeSquare = 5;
    public float MinSizeSquare = 1;
    public float intervalSquareSize = 0.3f;
    public int nb_square = 4;
    [SerializeField]
    private bool ChoseBiggestFirst = false;

    public GameObject squarePrefab;

    private List<GameObject> instanciateSquares = new List<GameObject>();

    [SerializeField]
    private Camera Camera;

    //random color
    private Color[] colors = { Color.yellow, Color.red, Color.green, Color.blue };
    //different color for the background
    private Color[] background_colors = { Color.black, Color.grey, Color.cyan };

    public GameState GameState { get => gameState; set => gameState = value; }

    public Button startButton;
    public Button restartButton;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }
    public void Start()
    {
        startButton.gameObject.SetActive(true);
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(StartGame);
        restartButton.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        // Réinitialiser les variables de jeu
        level = 1;
        score = 0;
        Timer = 60;
        TimeLeft = Timer;
        gameState = GameState.Playing;
        ChoseBiggestFirst = Random.Range(0, 2) == 0;

        // Désactiver et réactiver les boutons
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // Supprimer les carrés existants
        foreach (var square in instanciateSquares)
        {
            Destroy(square);
        }
        instanciateSquares.Clear();

        // Déclencher les événements
        if (ChoseBiggestFirst)
        {
            EventManager.instance.TriggerInfo("Big to Small");
        }
        else
        {
            EventManager.instance.TriggerInfo("Small to Big");
        }
        RandomizeBackgroundColor();
        SpawnSquare();
        EventManager.instance.TriggerTimer(TimeLeft);
        EventManager.instance.TriggerScore(score);
        EventManager.instance.TriggerLevel(level);
        EventManager.instance.TriggerGameStart();
    }


    public void Update()
    {
        if (gameState == GameState.Playing)
        {
            TimeLeft -= Time.deltaTime;
            EventManager.instance.TriggerTimer(TimeLeft);
            if (TimeLeft <= 0)
            {
                GameOver();
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Square")
                    {
                        bool isChoseTheRightOne = false;
                        //check if the player clicked on the right square beetwen all squares instanciate
                        float biggestSize = instanciateSquares[0].transform.localScale.x;
                        float smallestSize = instanciateSquares[0].transform.localScale.x;
                        for (int i = 0; i < instanciateSquares.Count; i++)
                        {
                            if (biggestSize < instanciateSquares[i].transform.localScale.x)
                            {
                                biggestSize = instanciateSquares[i].transform.localScale.x;
                            }
                            if (smallestSize > instanciateSquares[i].transform.localScale.x)
                            {
                                smallestSize = instanciateSquares[i].transform.localScale.x;
                            }
                        }
                        if (ChoseBiggestFirst)
                        {
                            if (biggestSize == hit.collider.gameObject.transform.localScale.x)
                            {
                                isChoseTheRightOne = true;
                            }
                        }
                        else
                        {
                            if (smallestSize == hit.collider.gameObject.transform.localScale.x)
                            {
                                isChoseTheRightOne = true;
                            }
                        }

                        if (!isChoseTheRightOne)
                        {
                            GameOver();
                        }
                        else { 
                            instanciateSquares.Remove(hit.collider.gameObject);
                            Destroy(hit.collider.gameObject);
                            score += 1;
                            EventManager.instance.TriggerScore(score);
                            if (instanciateSquares.Count == 0)
                            {
                                NextLevelSetup();
                            }
                        }
                    }
                }
            }
        }
    }

    private void GameOver()
    {
        gameState = GameState.GameOver;
        restartButton.gameObject.SetActive(true);
        EventManager.instance.TriggerGameOver();
    }

    private void NextLevelSetup()
    {
        level += 1;
        TimeLeft = Timer;
        EventManager.instance.TriggerLevel(level);
        ChoseBiggestFirst = Random.Range(0, 2) == 0;
        if (ChoseBiggestFirst)
        {
            EventManager.instance.TriggerInfo("Big to Small");
        }
        else
        {
            EventManager.instance.TriggerInfo("Small to Big");
        }
        SpawnSquare();
        RandomizeBackgroundColor();
    }

    public void SpawnSquare()
    {
        float sizeCurrentSquare = MinSizeSquare;
        for (int i = 0; i < nb_square; i++)
        {
            GameObject square = GameObject.Instantiate(squarePrefab);
            //random position inside the camera view
            Vector3 position = new Vector3(Random.Range(-Camera.orthographicSize + square.transform.localScale.x / 2, Camera.orthographicSize - square.transform.localScale.x / 2), Random.Range(-Camera.orthographicSize + square.transform.localScale.y / 2, Camera.orthographicSize - square.transform.localScale.y / 2), 0);
            //check if the position is not too close to another square
            for (int j = 0; j < instanciateSquares.Count; j++)
            {
                if (Vector3.Distance(instanciateSquares[j].transform.position, position) < MaxSizeSquare)
                {
                    position = new Vector3(Random.Range(-Camera.orthographicSize + square.transform.localScale.x / 2, Camera.orthographicSize - square.transform.localScale.x / 2), Random.Range(-Camera.orthographicSize + square.transform.localScale.y / 2, Camera.orthographicSize - square.transform.localScale.y / 2), 0);
                    j = 0;
                }
            }
            square.transform.position = position;

            square.GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Length)];
            square.transform.localScale = new Vector3(sizeCurrentSquare, sizeCurrentSquare, sizeCurrentSquare);

            square.tag = "Square";
            instanciateSquares.Add(square);
            sizeCurrentSquare += intervalSquareSize;
        }
    }
    private void RandomizeBackgroundColor()
    {
        Camera.backgroundColor = background_colors[Random.Range(0, background_colors.Length)];
    }
}
