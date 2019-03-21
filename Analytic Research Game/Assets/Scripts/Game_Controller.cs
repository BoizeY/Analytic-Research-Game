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
    void Start()
    {
        // Init the private variables
        canvas = GetComponent<Canvas>();
        gridLayout = GetComponent<GridLayoutGroup>();
        dataManager = GetComponent<Data_Manager>();
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
        dataManager.InitDataCollection();
    }

    public void EndRound()
    {
        // Tell the data manager to save this round's data
        dataManager.SaveRoundData(participantID, GetPaticipantGroupChar(), GetNotificationType());

        // Reload the scene for the next round OR end the test session
        if (roundID != 3)
        {
            // Reload the scene for the next round
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Tell the data manager to save the data to the participant's file
            dataManager.ExportData(participantID,  GetPaticipantGroupChar());

            // Move to the thank you scene
            SceneManager.LoadScene("ThankYou");
        }

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

    private char GetPaticipantGroupChar()
    {
        // Convert the group ID into a char and return it
        switch (participantGroup)
        {
            case 0:
                return 'A';
            case 1:
                return 'B';
            case 2:
                return 'C';
            default:
                return 'D';
        }
    }

    private NotificationType GetNotificationType()
    {
        // Get what noticiation type this player is interacting with from the playOrders array defined at the top
        return (NotificationType)playOrders[participantGroup, roundID];
    }
}
