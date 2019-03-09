using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Controls")]
    public int numRows;
    public int numCols;
    public float timeBetweenGameActivations;

    [Header("Game List")]
    public GameObject[] gamePrefabs;



    //--- Static Variables ---//
    public static int participantID = 0;
    public static int participantGroup = 0;
    public static int roundID = 0;



    //--- Private Variables ---//
    private Canvas canvas;
    private GridLayoutGroup gridLayout;
    private List<GameObject> remainingBuckets;
    private Data_Manager dataManager;
    private float timeSinceLastActivation;
    private bool doneSpawning;
    private bool roundStarted;
    private readonly int[,] playOrders = { { 0, 1, 3, 2 }, { 1, 2, 0, 3 }, { 2, 3, 1, 0 }, { 3, 0, 2, 1 } }; //{ "ABDC", "BCAD", "CDBA", "DACB" };
    private readonly float[] fillTimes = { 2.0f, 5.0f, 8.0f, 10.0f };



    //--- Unity Functions ---//
    private void Awake()
    {
        // TEMP:
        SetParticipantID(6);
    }

    void Start()
    {
        // Init the private variables
        canvas = GetComponent<Canvas>();
        gridLayout = GetComponent<GridLayoutGroup>();
        dataManager = GetComponent<Data_Manager>();
        timeSinceLastActivation = 0.0f;
        doneSpawning = false;
        roundStarted = false;
    }

    void Update()
    {
        // Do nothing until the game has actually begun
        if (roundStarted)
        {
            // Countdown to the next game spawn
            timeSinceLastActivation += Time.deltaTime;

            // Spawn the next game if the time has come
            if (timeSinceLastActivation >= timeBetweenGameActivations && !doneSpawning)
            {
                // Enable the next game
                ActivateNextBucket();

                // Reset the timer
                timeSinceLastActivation = 0.0f;
            }
        }
    }



    //--- Methods ---//
    public void StartRound()
    {
        // Set the bucket filltime
        int fillValID = playOrders[participantGroup, roundID];
        BucketController.fillDuration = fillTimes[fillValID];

        // Start the round
        roundStarted = true;
        SpawnGames();
        ActivateNextBucket();
    }

    public void ActivateNextBucket()
    {
        // If no more buckets to add, wait a few more seconds and then end the round
        if (remainingBuckets.Count == 0)
        {
            // Don't spawn any more buckets. When one more spawn cycle has passed, export the csv data
            doneSpawning = true;
            Invoke("EndRound", timeBetweenGameActivations);
            return;
        }

        // Determine the game that is going to be enabled
        int gameIndex = Random.Range(0, remainingBuckets.Count);

        // Enable the game controls in the scene
        remainingBuckets[gameIndex].transform.GetChild(0).gameObject.SetActive(true);

        // Remove the now active game from the list of deactivated ones
        remainingBuckets.RemoveAt(gameIndex);

        // Tell the data manager that the next game has activated
        dataManager.NextBucket();
    }

    public void EndRound()
    {
        // Output the data to the file
        dataManager.ExportData(participantID, participantGroup, (int)BucketController.fillDuration);

        // Reload the scene for the next round OR end the test session
        if (roundID != 3)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene("ThankYou");

        // Onto the next round
        roundID++;
    }

    public static void SetParticipantID(int _id)
    {
        participantID = _id;
        participantGroup = (_id % 4); 
    }



    //--- Utility Functions ---//
    private void SpawnGames()
    {
        // Set up the array to store all of the games
        remainingBuckets = new List<GameObject>();

        // Need to determine the width and height of each game based on the canvas size and the number of rows/cols
        float gameWidth = canvas.pixelRect.width / (float)numCols;
        float gameHeight = canvas.pixelRect.height / (float)numRows;

        // Apply the width and height to the grid controller
        gridLayout.cellSize = new Vector2(gameWidth, gameHeight);

        // Spawn all of the deactivated games into the scene and then add them to the list
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                int gameTypeIndex = Random.Range(0, gamePrefabs.Length);
                GameObject game = Instantiate(gamePrefabs[gameTypeIndex], this.transform);
                remainingBuckets.Add(game);
            }
        }
    }



    //--- Getters ---//
    public int GetNumberOfBuckets()
    {
        // We store the number of games left so we need the opposite of that to determine how many buckets there are
        return (numCols * numRows) - remainingBuckets.Count;
    }
}
