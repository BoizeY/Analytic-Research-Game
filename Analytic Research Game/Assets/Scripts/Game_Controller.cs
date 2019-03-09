﻿using System.Collections.Generic;
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



    //--- Private Variables ---//
    private Canvas canvas;
    private GridLayoutGroup gridLayout;
    private List<GameObject> remainingGames;
    private Data_Manager dataManager;
    private float timeSinceLastActivation;
    private bool doneSpawning;



    //--- Unity Functions ---//
    void Start()
    {
        // Init the private variables
        canvas = GetComponent<Canvas>();
        gridLayout = GetComponent<GridLayoutGroup>();
        dataManager = GetComponent<Data_Manager>();
        SpawnGames();
        timeSinceLastActivation = timeBetweenGameActivations;
        doneSpawning = false;
    }

    void Update()
    {
        // Countdown to the next game spawn
        timeSinceLastActivation += Time.deltaTime;

        // Spawn the next game if the time has come
        if (timeSinceLastActivation >= timeBetweenGameActivations && !doneSpawning)
        {
            // Enable the next game
            ActivateNextGame();

            // Reset the timer
            timeSinceLastActivation = 0.0f;
        }
    }



    //--- Methods ---//
    public void ActivateNextGame()
    {
        // If no more games to add, wait a few more seconds and then end the game
        if (remainingGames.Count == 0)
        {
            doneSpawning = true;
            Invoke("EndGame", timeBetweenGameActivations);
            return;
        }

        // Determine the game that is going to be enabled
        int gameIndex = Random.Range(0, remainingGames.Count);

        // Enable the game controls in the scene
        // TODO: Grab the script and call the function
        // TEMP: Manually enable the first child
        remainingGames[gameIndex].transform.GetChild(0).gameObject.SetActive(true);

        // Remove the now active game from the list of deactivated ones
        remainingGames.RemoveAt(gameIndex);
    }

    public void EndGame()
    {
        // Output the data to the file
        dataManager.ExportData();

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    //--- Utility Functions ---//
    private void SpawnGames()
    {
        // Set up the array to store all of the games
        remainingGames = new List<GameObject>();

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
                remainingGames.Add(game);
            }
        }
    }
}