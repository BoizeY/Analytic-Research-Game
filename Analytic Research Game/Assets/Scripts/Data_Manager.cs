using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Data_Participant
{
    //--- Public Variables ---//
    [Header("Early Data")]
    public List<Vector2> stageEarlyData;    // All of the early clicks within the current bucket stage
    public List<Vector2> avgEarlyData;      // The average early clicks for each of the previous bucket stages (actually gets output to the file)

    [Header("Early Data")]
    public List<Vector2> avgLateData;       // The averages of all of the buckets on the screen for every single click (actually gets output to the file)


    //--- Private Variables ---//
    private int numActiveBuckets; // Number of buckets currently active on the screen


    //--- Constructor ---//
    public Data_Participant()
    {
        // Init the data
        stageEarlyData = new List<Vector2>();    
        avgEarlyData = new List<Vector2>();     
        avgLateData = new List<Vector2>();
        numActiveBuckets = 0;
    }


    //--- Methods ---//
    public void AddEarlyData(int _numBuckets, float _errorValue)
    {
        stageEarlyData.Add(new Vector2((float)_numBuckets, _errorValue));
    }

    public void NextBucket()
    {
        // Calculate the averages for the early data from this past stage
        float earlyAvg = CalcEarlyAverage();

        // Add the average to the data list
        avgEarlyData.Add(new Vector2(numActiveBuckets, earlyAvg));

        // Dump the stage data
        stageEarlyData.Clear();

        // Move to the next bucket
        numActiveBuckets++;
    }

    public void StoreLateAverage(float _timeElapsed, float _averageOfLateBuckets)
    {
        avgLateData.Add(new Vector2(_timeElapsed, _averageOfLateBuckets));
    }

    public float CalcEarlyAverage()
    {
        if (stageEarlyData.Count == 0)
            return float.NaN;

        float sum = 0.0f;

        foreach (Vector2 d in stageEarlyData)
            sum += d.y;

        return sum / stageEarlyData.Count;
    }
}

public class Data_Manager : MonoBehaviour
{
    //--- Private Variables ---//
    private Game_Controller gameController;
    private Data_Participant participantData;
    private List<BucketController> activeBuckets;
    private string outputPath;
    private float elapsedTime;


    private float dataOutputInterval; // How many seconds between data output
    private float currentDataOutputInterval;
    private int numTotalBuckets; // The number of buckets that will eventually be on screen at once
    private List<List<Vector3>> bucketData;
    private bool isCollectingData;



    //--- Unity Functions ---//
    private void Start()
    {
        // Init the private variables
        gameController = GetComponent<Game_Controller>();
        participantData = new Data_Participant();
        outputPath = Application.persistentDataPath + "/Data/";
        activeBuckets = new List<BucketController>();
        elapsedTime = 0.0f;

        dataOutputInterval = 1.0f;
        currentDataOutputInterval = 0.0f;
        bucketData = new List<List<Vector3>>();
        isCollectingData = false;
        for (int i = 0; i < numTotalBuckets; i++)
            bucketData.Add(new List<Vector3>());
    }

    private void Update()
    {
        if (isCollectingData)
        {
            // Keep track of how long this round has lasted so far
            elapsedTime += Time.deltaTime;

            currentDataOutputInterval += Time.deltaTime;
            if (currentDataOutputInterval >= dataOutputInterval)
            {
                currentDataOutputInterval = 0.0f;
                CollectBucketData();
            }
        }
    }


    //--- Methods ---//
    public void AddEarlyDataPoint(float _errorValue)
    {
        // Add the data point to the participant data. Only call this when clicking a bucket early
        participantData.AddEarlyData(activeBuckets.Count, _errorValue);
    }

    public void AddAveragedLateData()
    {
        // Add a data point that is averaged from all of the late buckets in the scene
        float sum = 0.0f;
        float numLateBuckets = 0.0f;

        for (int i = 0; i < activeBuckets.Count; i++)
        {
            float fillNormalized = activeBuckets[i].GetFillNormalized();

            if (fillNormalized > 0.0f)
            {
                sum += fillNormalized;
                numLateBuckets++;
            }
        }

        float average = sum / numLateBuckets;

        participantData.StoreLateAverage(elapsedTime, average);
    }

    public void NextBucket(BucketController _bucket)
    {
        // Add the bucket to the list
        activeBuckets.Add(_bucket);

        // Tell the participant data to calculate the early and late averages for the last bucket count and move on to the next bucket count
        participantData.NextBucket();

        // Add the averaged late data to the list
        AddAveragedLateData();
    }

    public void ExportData(int _participantID, int _groupID, int _fillTime)
    {
        // Create the filename in the format ID#-Group-BucketFillTime (ex: P01-B-5s.csv)
        string filePath = outputPath +
            ((_participantID < 10) ? "P0" : "P") +
            _participantID.ToString() +
            "-" +
            GetPlayerGroupChar(_groupID) +
            "-" +
            _fillTime.ToString() + "s" +
            ".csv";

        // Open the file
        FileStream filestream = File.Open(filePath, FileMode.OpenOrCreate);
        StreamWriter streamWriter = new StreamWriter(filestream);

        //// Output all of the early data points into the file
        //for (int i = 0; i < participantData.avgEarlyData.Count; i++)
        //    streamWriter.WriteLine(Vec2ToString(participantData.avgEarlyData[i]));

        //// Output a bit of space
        //streamWriter.WriteLine(",");

        //// Output all of the late data points into the file
        //for (int i = 0; i < participantData.avgLateData.Count; i++)
        //    streamWriter.WriteLine(Vec2ToString(participantData.avgLateData[i]));

        // Output all of the data points into the file
        //for (int i = 0; i < bucketData.Count; i++)
        //{
        //    // Output all of the buckets side by side
        //    for (int j = 0; j < bucketData[i].Count; j++)
        //    {
        //        streamWriter.Write(Vec3ToString(bucketData[i][j]) + ",,");
        //    }

        //    streamWriter.Write("\n");
        //}

        for (int j = 0; j < bucketData[0].Count; j++)
        {
            for (int i = 0; i < bucketData.Count; i++)
            {
                streamWriter.Write(Vec3ToString(bucketData[i][j]) + ",,");
            }

            streamWriter.Write("\n");
        }

        // Close the file for safety
        streamWriter.Close();
        filestream.Close();
    }

    public void CollectBucketData()
    {
        // Add data for all of the active buckets
        for (int i = 0; i < activeBuckets.Count; i++)
            bucketData[i].Add(new Vector3(elapsedTime, activeBuckets[i].GetFillAmount_Seconds(), activeBuckets[i].GetFillAmount_Normalized()));

        // Add blank data for all of the other buckets
        for (int i = activeBuckets.Count; i < numTotalBuckets; i++)
            bucketData[i].Add(new Vector3(elapsedTime, float.NaN, float.NaN));
    }

    public void EnableDataCollection()
    {
        isCollectingData = true;
    }



    //--- Utility Functions ---//
    char GetPlayerGroupChar(int _groupID)
    {
        switch (_groupID)
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

    string Vec3ToString(Vector3 _v)
    {
        // Output in the format [x,y,z]. For NaN, output a blank
        string ret = _v.x.ToString() + ",";
        ret += (float.IsNaN(_v.y)) ? "" : _v.y.ToString();
        ret += ",";
        ret += (float.IsNaN(_v.z)) ? "" : _v.z.ToString();

        // Return the string
        return ret;
    }

    public void SetTotalNumBuckets(int _n)
    {
        numTotalBuckets = _n;
    }
}
