using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Data_Point
{
    public Data_Point(int _numBuckets, float _errorPercentage)
    {
        errorPercentage = _errorPercentage;
        numBuckets = _numBuckets + 1; //+1 so it starts at 1 instead of 0
    }

    public override string ToString()
    {
        string ret = numBuckets.ToString() + ",";

        if (!float.IsNaN(errorPercentage))
            ret += errorPercentage.ToString();

        return ret;
    }

    public float errorPercentage;
    public int numBuckets;
}

public class Data_Participant
{
    public Data_Participant()
    {
        stageEarlyData = new List<Data_Point>();
        stageLateData = new List<Data_Point>();

        avgEarlyData = new List<Data_Point>();
        avgLateData = new List<Data_Point>();
    }

    public void AddData(float _errorValue)
    {
        if (_errorValue >= 0)
            stageLateData.Add(new Data_Point(numBuckets, _errorValue));
        else
            stageEarlyData.Add(new Data_Point(numBuckets, _errorValue));
    }

    public void NextBucket()
    {
        // Calculate the averages for the early and late data
        float earlyAvg = CalcAverage(true);
        float lateAvg = CalcAverage(false);

        // Add the averages to the data lists
        avgEarlyData.Add(new Data_Point(numBuckets, earlyAvg));
        avgLateData.Add(new Data_Point(numBuckets, lateAvg));

        // Dump the stage data
        stageEarlyData.Clear();
        stageLateData.Clear();

        // Move to the next bucket
        numBuckets++;
    }

    public float CalcAverage(bool _useEarlyData)
    {
        List<Data_Point> dataList = (_useEarlyData) ? stageEarlyData : stageLateData;

        if (dataList.Count == 0)
            return float.NaN;

        float sum = 0.0f;

        foreach (Data_Point d in dataList)
            sum += d.errorPercentage;

        return sum / dataList.Count;
    }

    // Gets dumped after every bucket stage (new bucket is added). Will be converted into an average
    public List<Data_Point> stageEarlyData;
    public List<Data_Point> stageLateData;

    // Holds the average for each bucket stage. These lists are the ones that are actually output into the csv
    public List<Data_Point> avgEarlyData;
    public List<Data_Point> avgLateData;

    private int numBuckets = 0;
}

public class Data_Manager : MonoBehaviour
{
    //--- Private Variables ---//
    private Game_Controller gameController;
    private Data_Participant participantData;
    private string outputPath;



    //--- Unity Functions ---//
    private void Start()
    {
        // Init the private variables
        gameController = GetComponent<Game_Controller>();
        participantData = new Data_Participant();
        outputPath = Application.persistentDataPath + "/Data/";
    }



    //--- Methods ---//
    public void AddDataPoint(float _errorValue)
    {
        // Add the data point to the participant data
        participantData.AddData(_errorValue);
    }

    public void NextBucket()
    {
        // Tell the participant data to calculate the early and late averages for the last bucket count and move on to the next bucket count
        participantData.NextBucket();
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

        // Output all of the data points into the file
        for (int i = 0; i < gameController.GetNumberOfBuckets(); i++)
            streamWriter.WriteLine(participantData.avgEarlyData[i].ToString() + ",,," + participantData.avgLateData[i].ToString());

        // Close the file for safety
        streamWriter.Close();
        filestream.Close();
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
}
