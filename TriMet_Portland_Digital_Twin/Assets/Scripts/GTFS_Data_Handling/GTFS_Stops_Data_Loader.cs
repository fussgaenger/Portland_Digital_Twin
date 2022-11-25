/*************************************************
*                                                *
* Project          TriMet_Portland_Digital_Twin  *
* Supervisor       Christoph Traun               *
* Author           Winfried Schwan               *
* Participant ID   107023                        *
* Filename         GTFS_Stops_Data_Loader.cs     *
* Version          1.01                          *
* Summary          This script reads the locally *
*                  stored static GTFS file       *
*                  stops.txt, a CSV file that    *
*                  contains stop ids and names   *
*                                                *
*                  Stops ids and names are then  *
*                  exposed as a Dictionary for   *
*                  consumption by other Unity    *
*                  scripts                       *
*                                                *
* Created          2022-07-26 08:15:00           *
* Last modified    2022-11-25 07:55:00           *
*                                                *
**************************************************/

using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class GTFS_Stops_Data_Loader : MonoBehaviour
{
    //
    // Initializing the class variables...
    //

    //
    // Path to static GTFS file stops.txt that contains
    // stop ids and stop names in a csv format
    //

    private static string gtfsStopsFilePath = @"C:\tmp\gtfs_stops\stops.txt";


    //
    // set up a Dictionary that holds all the stop names
    // indexed by the stop id
    //

    public static Dictionary<int, string> gtfsStopsDictionary;


    /*************************************************
    *                                                *
    * Method name     Start                          *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         Start will be executed once    *
    *                 the scene is loaded            *
    *                                                *
    *                 It reads the static GTFS file  *
    *                 stops.txt and pushes stops ids *
    *                 and names into a Dictionary    *
    *                                                *
    **************************************************/

    void Start()
    {
        gtfsStopsDictionary = new Dictionary<int, string>();


        //
        // read the stops.txt file and extract 
        // stop id (field 0) and stop name (field 2)
        //

        using (var gtfsStopsReader = new StreamReader(gtfsStopsFilePath))
        {
            while (!gtfsStopsReader.EndOfStream)
            {
                string stopsFileLine = gtfsStopsReader.ReadLine();
                string[] stopsFileLineFields = stopsFileLine.Split(',');
               

                //
                // check if first field contains really a number
                // thus exlcuding anything that contains a string
                // in the first field which happened on some occasions
                //

                int integerKey;

                bool isKeyNumerical = int.TryParse(stopsFileLineFields[0], out integerKey);

                if(isKeyNumerical)
                {
                    gtfsStopsDictionary.Add(integerKey, stopsFileLineFields[2]);
                }
            }
        }
    }


    /*************************************************
    *                                                *
    * Method name     getStopName                    *
    * Arguments       int stopId                     *
    * Return value    gtfsStopsDictionary[stopId]    *
    * Summary         This method returns the stop   *
    *                 name in plain language as a    *
    *                 string when it is called with  *
    *                 the stop id from other scripts *
    *                                                *
    **************************************************/

    public static string getStopName(int stopId)
    {
        return gtfsStopsDictionary[stopId];
    }
}

/*************************************************
* End of file GTFS_Stops_Data_Loader.cs          *
**************************************************/
