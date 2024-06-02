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

                    // Example: Processing "C-100" into typeOfNote == "C" and accuracyWindow == "100"
                    string noteHyphenAccuracyWindow = fields[0];
                    string[] parts = noteHyphenAccuracyWindow.Split('-'); // Split the string at the hyphen
                    string typeOfNote = parts[0];
                    string accuracyWindow = (float.Parse(parts[1]) / 1000f).ToString(); // Convert ms to s, then to string for storing

                    string timeStamp = timeToSeconds(fields[1]);

                    // Check if formatting of note is correct
                    List<string> acceptedNotes = new List<string> { "C", "S", "SE", "T" };
                    List<float> acceptedAccuracyWindows = new List<float> { 0.025f, 0.05f, 0.075f, 0.1f, 0.125f };
                    if ( !acceptedNotes.Contains(typeOfNote) || !acceptedAccuracyWindows.Contains(float.Parse(accuracyWindow)))
                    {
                        Debug.Log("Invalid note format!");
                        return new List<Dictionary<string, string>>(); // Returns an empty beatmap
                    }

                    // For noteSquareEnd
                    if (typeOfNote == "SE")
                    {
                        beatMap[squareIndex].Add("timeStampRelease", timeStamp);
                        continue;
                    }
                    
                    Dictionary<string, string> note = new Dictionary<string, string>()
                    {
                        { "typeOfNote", typeOfNote },
                        { "accuracyWindow", accuracyWindow }, // In the case of noteSquareEnd, it will always adopt the accuracy window of noteSquareStart
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
            Debug.Log("File not found! No beatmap will be loaded.");
        }

        return beatMap;
    }

    private static string timeToSeconds(string time)
    {
        // Split the string at the colon
        string[] parts = time.Split(':');

        // Parse minutes and seconds parts to float
        float minutes = float.Parse(parts[0]);
        float seconds = float.Parse(parts[1]);

        // Convert to seconds and add
        float totalTime = minutes * 60 + seconds;

        // Buffer for better syncing
        float totalTimeWithBuffer = totalTime + 0.265f;

        return totalTimeWithBuffer.ToString();
    }
}
