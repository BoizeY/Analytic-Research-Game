using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BucketController : MonoBehaviour
{

    public GameObject bucketWater;
    public Button empty;
    public float waterFillTimer;
    public Text timeRelativeToFill;
    public Text timePassFill;
    public float canEmptyPercentage;

    public GameObject container;

    Image bucketWaterSprite;
    float timer;
    float tillFull;

    private Data_Manager dataManager;

    private void Awake()
    {
        dataManager = GameObject.FindObjectOfType<Data_Manager>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {

        bucketWaterSprite = bucketWater.GetComponent<Image>();
        empty.onClick.AddListener(emptyWater);
        timer = 0.0f;

        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime/ waterFillTimer;
        // time relative till what is at fill
        tillFull = timer* waterFillTimer;
        bucketWaterSprite.fillAmount = timer;

        timeRelativeToFill.text = (tillFull - waterFillTimer).ToString();
    }

    void emptyWater()
    {
        // Pass the calculated error percentage to the data manager
        float fillAmount = (tillFull - waterFillTimer);
        dataManager.AddDataPoint(new Data_Point(fillAmount));

        timePassFill.text = fillAmount.ToString();
            timer = 0.0f;
            bucketWaterSprite.fillAmount = 0.0f;
    }


    void setGameOff(bool set)
    {
        container.SetActive(set);
    }
}
