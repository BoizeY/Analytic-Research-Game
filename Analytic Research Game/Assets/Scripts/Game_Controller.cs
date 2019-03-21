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
    public float roundDuration;
    public Vector2 fillTimeRange;

    [Header("Game List")]
    public GameObject bucketPrefab;



    //--- Static Variables ---//
    public static int participantID = 0;
    public static int participantGroup = 0;
    public static int roundID = 0;



    //--- Private Variables ---//
    private Canvas canvas;
    private GridLayoutGroup gridLayout;
    private Data_Manager dataManager;
    private bool roundStarted;
    private float roundTimer;
    private readonly int[,] playOrders = { { 0, 1, 3, 2 }, { 1, 2, 0, 3 }, { 2, 3, 1, 0 }, { 3, 0, 2, 1 } }; //{ "ABDC", "BCAD", "CDBA", "DACB" };



    //--- Unity Functions ---//
    private void Awake()
    {
        // Init the data manager 
        dataManager = GetComponent<Data_Manager>();
        dataManager.SetTotalNumBuckets(numRows * numCols);
    }

    void Start()
    {
        // Init the private variables
        canvas = GetComponent<Canvas>();
        gridLayout = GetComponent<GridLayoutGroup>();
        roundStarted = false;
        roundTimer = 0.0f;
    }

    void Update()
    {
        // Do nothing until the game has actually begun
        if (roundStarted)
        {
            // Keep track of how long this round has been going on
            roundTimer += Time.deltaTime;

            // If the round is over, move on
            if (roundTimer >= roundDuration)
                EndRound();
        }
    }



    //--- Methods ---//
    public void StartRound()
    {
        // Start the round
        roundStarted = true;
        SpawnGames();
        dataManager.EnableDataCollection();
    }

    public void EndRound()
    {
        // Output the data to the file
        //dataManager.ExportData(participantID, participantGroup, (int)BucketController.fillDuration);

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
        // The participants will move through the groups from 1 - 4 and then loop back around
        participantID = _id;
        participantGroup = (_id % 4); 
    }



    //--- Utility Functions ---//
    private void SpawnGames()
    {
        // Need to determine the width and height of each buckets based on the canvas size and the number of rows/cols
        float gameWidth = canvas.pixelRect.width / (float)numCols;
        float gameHeight = canvas.pixelRect.height / (float)numRows;

        // Apply the width and height to the grid controller
        gridLayout.cellSize = new Vector2(gameWidth, gameHeight);

        // Spawn all of the buckets into the scene and tell them to randomize their fill time
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                // Spawn the bucket
                BucketController bucket = Instantiate(bucketPrefab, this.transform).GetComponentInChildren<BucketController>();

                // Tell the bucket to randomize its fill time between the range given in the inspector
                bucket.SetFillDurationRange(fillTimeRange);
            }
        }
    }
}
