using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkersProcessingScript : MonoBehaviour
{
    public static List<Dictionary<string, string>> ProcessMarkers(string filePath)
    {
        List<Dictionary<string, string>> beatMap = new List<Dictionary<string, string>>();

        // Check if the file exists
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the header line
                reader.ReadLine();

                int squareIndex = 0;

                // Read and process each line in the file
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split('\t');
                    string typeOfNote = fields[0];
                    string timeStamp = timeToSeconds(fields[1]);

                    // For noteSquareEnd
                    if (typeOfNote == "SE")
                    {
                        beatMap[squareIndex].Add("timeStampRelease", timeStamp);
                        continue;
                    }
                    
                    Dictionary<string, string> note = new Dictionary<string, string>()
                    {
                        { "typeOfNote", typeOfNote },
                        { "timeStamp", timeStamp }
                    };

                    beatMap.Add(note);

                    // Update squareIndex so that SE can reference the current note
                    if (typeOfNote == "S")
                    {
                        squareIndex = beatMap.Count - 1;
                    }    
                }
            }
        }
        else
        {
            Debug.Log("File not found!");
        }

        return beatMap;
    }

    private static string timeToSeconds(string time)
    {
        // Split the string at the colon
        string[] parts = time.Split(':');
        // Changes the timing based on the player offset
        int offset = PlayerPrefs.GetInt("GlobalOffset");
        // Parse minutes and seconds parts to float
        float minutes = float.Parse(parts[0]);
        float seconds = float.Parse(parts[1]) + offset/1000;

        // Convert to seconds and add
        float totalTime = minutes * 60 + seconds;

        return totalTime.ToString();
    }
}
