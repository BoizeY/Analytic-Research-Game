using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Data_Manager : MonoBehaviour
{
    //--- Public Variables ---//
    public float timeBetweenSamples;



    //--- Private Variables ---//
    private BucketController[] buckets;
    private List<float> roundErrorRates;
    private float timeSinceLastSample;



    //--- Static Variables ---//
    private static List<Data_Frame> fullGameData;



    //--- Unity Functions ---//
    private void Update()
    {
        // If the buckets exist, collect data on them
        if (buckets != null)
        {
            // Sample the data every X seconds
            timeSinceLastSample += Time.deltaTime;
            if (timeSinceLastSample >= timeBetweenSamples)
                CollectDataSample();
        }
    }



    //--- Methods ---//
    public void InitDataCollection()
    {
        // Init the timer
        timeSinceLastSample = 0.0f;

        // Find all of the buckets in the scene and keep track of them
        buckets = GameObject.FindObjectsOfType<BucketController>();

        // Init the list
        roundErrorRates = new List<float>();
    }

    public void CollectDataSample()
    {
        // These will be used to store a sum of all of the errors in the buckets and then average that across the number of 'error' buckets
        float errorSum = 0.0f;
        int numErrors = 0;

        // Loop through all of the buckets and collect the data from them
        foreach (BucketController bucket in buckets)
        {
            // If the bucket is above the fill line, it is in an error state
            if (bucket.GetIsInErrorState())
            {
                // Collect the error amount from the bucket
                errorSum += bucket.GetErrorAmount();
                numErrors++;
            }
        }

        // Calculate the average error. If there are no errors, then set it to 0 so we don't divide by 0
        float averageError = (numErrors > 0) ? errorSum / (float)numErrors : 0.0f;

        // Store the average error in the data list
        roundErrorRates.Add(averageError);

        // Reset the countdown to the next error sample
        timeSinceLastSample = 0.0f;
    }

    public void SaveRoundData(int _participantID, char _participantGroup, NotificationType _notType)
    {
        // Calculate one more data point sample
        CollectDataSample();

        // Init the persistent data array if it isn't already
        if (fullGameData == null) fullGameData = new List<Data_Frame>();

        // Store the data in the persistent data array
        fullGameData.Add(new Data_Frame(_participantID, _participantGroup, _notType, CalculateRoundErrorAvg()));

        // Clear the round data since this round is over
        roundErrorRates.Clear();
    }

    public void ExportData(int _participantID, char _participantGroup, string _participantAge, string _participantGender)
    {
        // Create the filename in the format ID#-Group.csv (ex: P01-B.csv)
        string filePath = Application.persistentDataPath + "/Data/" +
            ((_participantID < 10) ? "P0" : "P") +
            _participantID.ToString() +
            "-" +
            _participantGroup +
            ".csv";

        // Sort the output list so it's always the same order
        var sortedData = fullGameData.OrderBy(x => (int)x.notificationType).ToArray();

        // Open the file
        FileStream filestream = File.Open(filePath, FileMode.OpenOrCreate);
        StreamWriter streamWriter = new StreamWriter(filestream);

        // Output the personal information about the participant
        streamWriter.WriteLine("Age," + _participantAge + ",Gender," + _participantGender);

        // Output the headers for the rest of the data set
        streamWriter.WriteLine("ParticipantID,ParticipantGroup,NotificationType,AvgErrorValue");

        // Write the persistent data array to the file
        for (int i = 0; i < fullGameData.Count; i++)
            streamWriter.WriteLine(sortedData[i].ToString());

        // Close the file
        streamWriter.Close();
        filestream.Close();

        // Clear the persistent data array to finish up
        fullGameData.Clear();
    }



    //--- Utility Functions ---//
    float CalculateRoundErrorAvg()
    {
        // Store the sum of the errors
        float sum = 0.0f;

        // Sum up all of the individual data points across the entire round
        for (int i = 0; i < roundErrorRates.Count; i++)
            sum += roundErrorRates[i];

        // Return the average of all of the error rates
        return sum / (float)roundErrorRates.Count;
    }
}
