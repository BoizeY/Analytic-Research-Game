using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Data_Point
{
    public Data_Point(float _errorPercentage)
    {
        time = Time.timeSinceLevelLoad;
        errorPercentage = Mathf.Abs(_errorPercentage);
    }

    public override string ToString()
    {
        return time.ToString() + "," + errorPercentage.ToString() + "\n";
    }

    public float time;
    public float errorPercentage;
}

public class Data_Manager : MonoBehaviour
{
    //--- Private Variables ---//
    private List<Data_Point> data;
    private string outputPath;



    //--- Unity Functions ---//
    private void Start()
    {
        // Init the private variables
        data = new List<Data_Point>();
        outputPath = Application.persistentDataPath + "/Data/";
    }



    //--- Methods ---//
    public void AddDataPoint(Data_Point _dataPoint)
    {
        // Add the new data point to the list
        data.Add(_dataPoint);
    }

    public void ExportData()
    {
        // We are going to use the current timestamp as the filename
        string currentTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string filePath = outputPath + currentTimestamp + ".csv";

        // Open the file
        FileStream filestream = File.Open(filePath, FileMode.CreateNew);
        StreamWriter streamWriter = new StreamWriter(filestream);

        // Output all of the data points into the file
        foreach (Data_Point p in data)
            streamWriter.Write(p.ToString());

        // Close the file for safety
        streamWriter.Close();
        filestream.Close();
    }
}
