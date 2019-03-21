using UnityEngine;
using UnityEngine.UI;


public enum NotificationType
{
    NONE,
    ANIMATION,
    ICON,
    COLOUR,
    COUNT


}


public class BucketController : MonoBehaviour
{
    public NotificationType notificationStyle;
    // The number of seconds into the current filling
    private Vector2 fillDurationRange;
    private float fillDuration;
    private float currentFillTime;
    public GameObject bucketWater;
    public Button empty;
    public GameObject container;
    public GameObject NotificationIcon;
    public GameObject bucketWaterRed;


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
        empty.gameObject.SetActive(false);

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
        Notification();

    }

    void emptyWater()
    {
        // Calculate the normalized fill amount
        float fillNormalized = GetFillNormalized();

        // Pass the early data point into the data manager if the click was early
        //if (fillNormalized < 0.0f)
        //   dataManager.AddEarlyDataPoint(fillNormalized);

        // Every click, we average out the values for ALL of the buckets on the screen
        //dataManager.AddAveragedLateData();

        // Reset the fill amount
        ResetFillTime();
        bucketWaterSprite.fillAmount = 0.0f;

        // Now, the bucket should be paused for a bit before refilling to prevent the dominant strategy
        isPaused = true;
        pauseDuration = Random.Range(0.0f, 1.0f);
        currentPauseTime = 0.0f;
        //disable button
        empty.gameObject.SetActive(false);
        gameObject.GetComponent<Animator>().SetBool("Shake", false);
        NotificationIcon.SetActive(false);
        bucketWaterSprite = bucketWater.GetComponent<Image>(); 
        //also reset the fill for the red bucket
        bucketWaterSprite.fillAmount = 0.0f;
    }
    void setGameOff(bool set)
    {
        container.SetActive(set);
    }

    void ResetPause()
    {

    }

    public float GetFillNormalized()
    {
        return (currentFillTime >= fillDuration) ? (currentFillTime - fillDuration) / fillDuration : -(1.0f - (currentFillTime / fillDuration));
    }

    public float GetFillAmount_Seconds()
    {
        return currentFillTime;
    }

    public float GetFillAmount_Normalized()
    {
        return currentFillTime / fillDuration;
    }

    public void ResetFillTime()
    {
        // Randomly select the fill duration within the range
        fillDuration = Random.Range(fillDurationRange.x, fillDurationRange.y);
        currentFillTime = 0.0f;
    }

    public void SetFillDurationRange(Vector2 _range)
    {
        // Set the fill range
        this.fillDurationRange = _range;

        // Now, init the fill time to something within that range
        ResetFillTime();
    }

    public bool GetIsInErrorState()
    {
        // The bucket is in 'error' if it is above half (ie: above its fill line)
        return currentFillTime >= (fillDuration * 0.5f);
    }

    public float GetErrorAmount()
    {
        // Return the normalized amount that the bucket is filled ABOVE the fill line
        float halfFull = fillDuration * 0.5f;
        return (currentFillTime - halfFull);
    }

    public void Notification()
    {

        if (bucketWaterSprite.fillAmount > 0.5)
        {
            //make the empty bucket button active;
            empty.gameObject.SetActive(true);

            if (notificationStyle == NotificationType.NONE)
            {

            }
            else if (notificationStyle == NotificationType.ANIMATION)
            {
                gameObject.GetComponent<Animator>().SetBool("Shake", true);
                
            }
            else if (notificationStyle == NotificationType.ICON)
            {
                NotificationIcon.SetActive(true);
            }
            else if (notificationStyle == NotificationType.COLOUR)
            {
                bucketWaterSprite = bucketWaterRed.GetComponent<Image>();
            }
        }
    }

}


