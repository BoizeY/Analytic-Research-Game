using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BucketController : MonoBehaviour
{
    // How long until the bucket reaches the fill line in the center of the bucket (in seconds)
    public static float fillDuration; 
    // The number of seconds into the current filling
    private float currentFillTime;
    public GameObject bucketWater;
    public Button empty;
    public GameObject container;


    private Image bucketWaterSprite;
    private Data_Manager dataManager;
    private bool isPaused;
    private float currentPauseTime;
    private float pauseDuration;

    private void Awake()
    {
        dataManager = GameObject.FindObjectOfType<Data_Manager>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        // Init the sprite and the button listener
        bucketWaterSprite = bucketWater.GetComponent<Image>();
        empty.onClick.AddListener(emptyWater);

        // Reset the fill time when the bucket is spawned
        currentFillTime = 0.0f;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            // Increase the fill amount
            currentFillTime += Time.deltaTime;

            // Update the sprite
            bucketWaterSprite.fillAmount = (currentFillTime / fillDuration) * 0.5f;
        }
        else
        {
            // Increase the pause timer
            currentPauseTime += Time.deltaTime;

            // If the pause is over, we should go back to filling up
            if (currentPauseTime >= pauseDuration)
                isPaused = false;
        }
    }

    void emptyWater()
    {
        // Calculate the normalized fill amount
        float fillNormalized = (currentFillTime >= fillDuration) ? (currentFillTime - fillDuration) / fillDuration : 1.0f - (currentFillTime/fillDuration); 

        // Pass the data point into the data manager
        dataManager.AddDataPoint(fillNormalized);

        // Reset the fill amount
        currentFillTime = 0.0f;
        bucketWaterSprite.fillAmount = 0.0f;

        // Now, the bucket should be paused for a bit before refilling to prevent the dominant strategy
        isPaused = true;
        pauseDuration = Random.Range(0.0f, 1.0f);
        currentPauseTime = 0.0f;
    }
    void setGameOff(bool set)
    {
        container.SetActive(set);
    }

    void ResetPause()
    {

    }
}
