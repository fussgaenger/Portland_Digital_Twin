/*************************************************
*                                                *
* Project          TriMet_Portland_Digital_Twin  *
* Supervisor       Christoph Traun               *
* Author           Winfried Schwan               *
* Filename         GTFS_Data_Processor.cs        *
* Version          1.01                          *
* Summary          This script reads locally     *
*                  stored realtime data from the *
*                  Portland TriMet GTFS feed and *
*                  organizes vehicle data        *
*                  Vehicle data are then put in  *
*                  a List for consumption by     *
*                  Unity                         *
*                                                *
* Created          2022-07-22 15:00:00           *
* Last modified    2023-07-03 10:50:00           *
*                                                *
**************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;


public class GTFS_Data_Processor : MonoBehaviour
{
    //
    // Initializing the class variables...
    //

    static public bool debugMode = false;

    static public string workingFolderPath   = @"C:\tmp\gtfs_working\";
    static public string processedFolderPath = @"C:\tmp\gtfs_processed\";

    static public List<Vehicle> vehicleList = new List<Vehicle>();

    static public Thread gtfsProcessorThread;

    static public FileSystemWatcher watcher;


    /*************************************************
    *                                                *
    * Method name     Start                          *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         Start will be executed once    *
    *                 the scene is loaded            *
    *                                                *
    *                 Here it is used to launch the  *
    *                 that consumes GTFS data in the *
    *                 background                     *
    *                                                *
    **************************************************/

    void Start()
    {
        Debug.Log("Running gtfsProcessorThread...");

        gtfsProcessorThread = new Thread(gtfsProcessor);
        gtfsProcessorThread.Start();
    }


    /*************************************************
    *                                                *
    * Method name     OnApplicationQuit              *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         This method will be executed   *
    *                 when the application will be   *
    *                 shut down                      *
    *                                                *
    *                 Here it is used to kill the    *
    *                 thread that is responsible for *
    *                 processing GTFS data in the    *
    *                 background                     *
    *                                                *
    *                 The file system watcher is     *
    *                 also shut down here            *
    *                                                *
    **************************************************/

    void OnApplicationQuit()
    {
        Debug.Log("Killing gtfsProcessorThread...");

        gtfsProcessorThread.Abort();
        watcher.Dispose();
    }


    /*************************************************
    *                                                *
    * Method name     gtfsProcessor                  *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         This method is responsible for *
    *                 watching the working folder    *
    *                 where newly collected GTFS     *
    *                 data arrive                    *
    *                                                *
    *                 Every time a new file with the *
    *                 extension *.complete has been  *
    *                 created an "OnCreated" event   *
    *                 will be triggered              *
    *                                                *
    *                 This event is then captured in *
    *                 the "OnCreated" method and     *
    *                 GTFS processing begins         *
    *                                                *
    **************************************************/

    private static void gtfsProcessor()
    {
        if (debugMode)
        {
            Debug.Log("Initializing FileSystemWatcher targeting folder: " + workingFolderPath + "...");
        }

        watcher = new FileSystemWatcher(workingFolderPath);

        watcher.Created += OnCreated;

        watcher.Filter = "*.complete";
        watcher.IncludeSubdirectories = false;
        watcher.EnableRaisingEvents = true;
    }


    /*************************************************
    *                                                *
    * Method name     OnCreated                      *
    * Arguments       object Sender                  *
    *                 FileSystemEventArgs e          *
    * Return value    none                           *
    * Summary         This method is executed each   *
    *                 time a new GTFS file has been  *
    *                 found                          *
    *                                                *
    *                 It calls the method            *
    *                 responsible for GTFS parsing   *
    *                                                *
    **************************************************/

    public static void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (debugMode)
        {
            Debug.Log("Event FileCreated captured...");
        }

        processGtfsFiles(e);
    }


    /*************************************************
    *                                                *
    * Method name     processGtfsFiles               *
    * Arguments       FileSystemEventArgs e          *
    * Return value    none                           *
    * Summary         GTFS file name is built here   *
    *                 then file will be processed    *
    *                 and file will be moved from    *
    *                 "working" to "processed"       *
    *                 folder                         *
    *                                                *
    **************************************************/

    public static void processGtfsFiles(FileSystemEventArgs e)
    {
        if (debugMode)
        {
            Debug.Log("File " + e.Name + " has been created.");
        }


        //
        // get filename w/o extension
        //

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(e.Name);


        //
        // build json file name
        //

        string fileNameWithJsonExtension = fileNameWithoutExtension + ".json";


        //
        // delete *.complete file
        //

        deleteCompleteFile(e.FullPath);


        //
        // process json file
        //

        processJsonFile(workingFolderPath + fileNameWithJsonExtension);


        //
        // move json file to processed folder
        //

        moveJsonFileToProcessedFolder(fileNameWithJsonExtension);
    }


    /*************************************************
    *                                                *
    * Method name     deleteCompleteFile             *
    * Arguments       string completeFileToBeDeleted *
    * Return value    none                           *
    * Summary         Signaling file with complete   *
    *                 extension will be deleted      *
    *                 after processing the GTFS file *
    *                                                *
    **************************************************/

    public static void deleteCompleteFile(string completeFileToBeDeleted)
    {
        if (debugMode)
        {
            Debug.Log(completeFileToBeDeleted + " will be deleted");
        }

        File.Delete(completeFileToBeDeleted);
    }


    /*************************************************
    *                                                *
    * Method name     processJsonFile                *
    * Arguments       string jsonFileToBeProcessed   *
    * Return value    none                           *
    * Summary         Reads GTFS JSON file and       *
    *                 extracts information about     *
    *                 vehicles, finally put all      *
    *                 vehicles into a list           *
    *                                                *
    **************************************************/

    public static void processJsonFile(string jsonFileToBeProcessed)
    {
        if (debugMode)
        {
            Debug.Log(jsonFileToBeProcessed + " will be processed");
        }

        string vehicleJsonString;

        using (StreamReader streamReader = new StreamReader(jsonFileToBeProcessed))
        {
            vehicleJsonString = streamReader.ReadToEnd();
        }


        //
        // put vehicles in List
        //

        pushVehiclesInList(vehicleJsonString);
    }


    /*************************************************
    *                                                *
    * Method name     moveJsonFileToProcessedFolder  *
    * Arguments       string jsonFileToBeMoved       *
    * Return value    none                           *
    * Summary         After GTFS JSON processing has *
    *                 finished processed file will   *
    *                 moved to "processed" folder    *
    *                                                *
    **************************************************/

    public static void moveJsonFileToProcessedFolder(string jsonFileToBeMoved)
    {
        if (debugMode)
        {
            Debug.Log(jsonFileToBeMoved + " will be moved");
        }

        string sourceFileName = workingFolderPath + jsonFileToBeMoved;
        string destFileName = processedFolderPath + jsonFileToBeMoved;

        File.Move(sourceFileName, destFileName);
    }


    /*************************************************
    *                                                *
    * Method name     pushVehiclesInList             *
    * Arguments       string vehicleJsonString       *
    * Return value    none                           *
    * Summary         Build a list of type Vehicle   *
    *                 that contains all vehicle      *
    *                 information from latest GTFS   *
    *                 JSON file                      *
    *                                                *
    **************************************************/

    public static void pushVehiclesInList(string vehicleJsonString)
    {
        dynamic allVehicles = JsonConvert.DeserializeObject(vehicleJsonString);

        int numberOfVehicles = 0;


        //
        // find out how many vehicles are in operation
        // currently
        //

        numberOfVehicles = getNumberOfVehicles(vehicleJsonString);

        vehicleList.Clear();


        string expires;
        string signMessage;
        string serviceDate;
        string loadPercentage;

        string latitude;
        string nextStopSeq;
        string source;
        string type;

        string blockID;
        string signMessageLong;
        string lastLocID;
        string nextLocID;

        string locationInScheduleDay;
        string newTrip;
        string longitude;
        string direction;

        string inCongestion;
        string routeNumber;
        string bearing;
        string garage;

        string tripID;
        string delay;
        string extraBlockID;
        string messageCode;

        string lastStopSeq;
        string vehicleID;
        string time;
        string offRoute;


        for (int i = 0; i < numberOfVehicles; i++)
        {
            //
            // collect all vehicle properties
            //

            expires         = allVehicles.resultSet.vehicle[i].expires;
            signMessage     = allVehicles.resultSet.vehicle[i].signMessage;
            serviceDate     = allVehicles.resultSet.vehicle[i].serviceDate;
            loadPercentage  = allVehicles.resultSet.vehicle[i].loadPercentage;

            latitude        = allVehicles.resultSet.vehicle[i].latitude;
            nextStopSeq     = allVehicles.resultSet.vehicle[i].nextStopSeq;
            source          = allVehicles.resultSet.vehicle[i].source;
            type            = allVehicles.resultSet.vehicle[i].type;

            blockID         = allVehicles.resultSet.vehicle[i].blockID;
            signMessageLong = allVehicles.resultSet.vehicle[i].signMessageLong;
            lastLocID       = allVehicles.resultSet.vehicle[i].lastLocID;
            nextLocID       = allVehicles.resultSet.vehicle[i].nextLocID;

            locationInScheduleDay = allVehicles.resultSet.vehicle[i].locationInScheduleDay;
            newTrip         = allVehicles.resultSet.vehicle[i].newTrip;
            longitude       = allVehicles.resultSet.vehicle[i].longitude;
            direction       = allVehicles.resultSet.vehicle[i].direction;

            inCongestion    = allVehicles.resultSet.vehicle[i].inCongestion;
            routeNumber     = allVehicles.resultSet.vehicle[i].routeNumber;
            bearing         = allVehicles.resultSet.vehicle[i].bearing;
            garage          = allVehicles.resultSet.vehicle[i].garage;

            tripID          = allVehicles.resultSet.vehicle[i].tripID;
            delay           = allVehicles.resultSet.vehicle[i].delay;
            extraBlockID    = allVehicles.resultSet.vehicle[i].extraBlockID;
            messageCode     = allVehicles.resultSet.vehicle[i].messageCode;

            lastStopSeq     = allVehicles.resultSet.vehicle[i].lastStopSeq;
            vehicleID       = allVehicles.resultSet.vehicle[i].vehicleID;
            time            = allVehicles.resultSet.vehicle[i].time;
            offRoute        = allVehicles.resultSet.vehicle[i].offRoute;


            //
            // build a new, temporary Vehicle object
            //

            Vehicle tmpVehicle = new Vehicle();


            //
            // assign properties to Vehicle object
            //

            tmpVehicle.expires               = expires;
            tmpVehicle.signMessage           = signMessage;
            tmpVehicle.serviceDate           = serviceDate;
            tmpVehicle.loadPercentage        = loadPercentage;

            tmpVehicle.latitude              = latitude;
            tmpVehicle.nextStopSeq           = nextStopSeq;
            tmpVehicle.source                = source;
            tmpVehicle.type                  = type;

            tmpVehicle.blockID               = blockID;
            tmpVehicle.signMessageLong       = signMessageLong;
            tmpVehicle.lastLocID             = lastLocID;
            tmpVehicle.nextLocID             = nextLocID;

            tmpVehicle.locationInScheduleDay = locationInScheduleDay;
            tmpVehicle.newTrip               = newTrip;
            tmpVehicle.longitude             = longitude;
            tmpVehicle.direction             = direction;

            tmpVehicle.inCongestion          = inCongestion;
            tmpVehicle.routeNumber           = routeNumber;
            tmpVehicle.bearing               = bearing;
            tmpVehicle.garage                = garage;

            tmpVehicle.tripID                = tripID;
            tmpVehicle.delay                 = delay;
            tmpVehicle.extraBlockID          = extraBlockID;
            tmpVehicle.messageCode           = messageCode;

            tmpVehicle.lastStopSeq           = lastStopSeq;
            tmpVehicle.vehicleID             = vehicleID;
            tmpVehicle.time                  = time;
            tmpVehicle.offRoute              = offRoute;


            //
            // add newly created Vehicle object to list vehicleList
            //

            vehicleList.Add(tmpVehicle);
        }


        //
        // if debug flag is enabled then output 
        // vehicle properties to Unity console
        // for verification
        //

        if (debugMode)
        {
            for (int i = 0; i < vehicleList.Count; i++)
            {
                Debug.Log("expires: " + vehicleList[i].expires);
                Debug.Log("signMessage: " + vehicleList[i].signMessage);
                Debug.Log("serviceDate: " + vehicleList[i].serviceDate);
                Debug.Log("loadPercentage: " + vehicleList[i].loadPercentage);
                Debug.Log("latitude: " + vehicleList[i].latitude);
                Debug.Log("nextStopSeq: " + vehicleList[i].nextStopSeq);
                Debug.Log("source: " + vehicleList[i].source);
                Debug.Log("type: " + vehicleList[i].type);
                Debug.Log("blockID: " + vehicleList[i].blockID);
                Debug.Log("signMessageLong: " + vehicleList[i].signMessageLong);
                Debug.Log("lastLocID: " + vehicleList[i].lastLocID);
                Debug.Log("nextLocID: " + vehicleList[i].nextLocID);
                Debug.Log("locationInScheduleDay: " + vehicleList[i].locationInScheduleDay);
                Debug.Log("newTrip: " + vehicleList[i].newTrip);
                Debug.Log("longitude: " + vehicleList[i].longitude);
                Debug.Log("direction: " + vehicleList[i].direction);
                Debug.Log("inCongestion: " + vehicleList[i].inCongestion);
                Debug.Log("routeNumber: " + vehicleList[i].routeNumber);
                Debug.Log("bearing: " + vehicleList[i].bearing);
                Debug.Log("garage: " + vehicleList[i].garage);
                Debug.Log("tripID: " + vehicleList[i].tripID);
                Debug.Log("delay: " + vehicleList[i].delay);
                Debug.Log("extraBlockID: " + vehicleList[i].extraBlockID);
                Debug.Log("messageCode: " + vehicleList[i].messageCode);
                Debug.Log("lastStopSeq: " + vehicleList[i].lastStopSeq);
                Debug.Log("vehicleID: " + vehicleList[i].vehicleID);
                Debug.Log("time: " + vehicleList[i].time);
                Debug.Log("offRoute: " + vehicleList[i].offRoute);
            }
        }
    }


    /*************************************************
    *                                                *
    * Method name     getNumberOfVehicles            *
    * Arguments       string vehicleList             *
    * Return value    int numberOfVehicles           *
    * Summary         Gets nunber of vehicles        *
    *                 in operation                   *
    *                                                *
    **************************************************/

    public static int getNumberOfVehicles(string vehicleList)
    {
        dynamic allVehicles = JsonConvert.DeserializeObject(vehicleList);

        JArray vehicleArray;

        int numberOfVehicles = 0;

        try
        {
            vehicleArray = allVehicles.resultSet.vehicle;
            numberOfVehicles = vehicleArray.Count;
        }
        catch (Exception exception)
        {
            Debug.Log("No vehicles operational");
            Debug.Log(exception.Message);
        }

        return numberOfVehicles;
    }


    /*************************************************
    *                                                *
    * Method name     getVehicleList                 *
    * Arguments       string vehicleList             *
    * Return value    List<Vehicle> vehicleList      *
    * Summary         Exposes list vehicleList to    *
    *                 other scripts in Unity project *
    *                                                *
    **************************************************/

    public static List<Vehicle> getVehicleList()
    {
        return vehicleList;
    }


    /*************************************************
    *                                                *
    * Class name      Vehicle                        *
    * Summary         Defines a class Vehicle        *
    *                 according to the vehicle info  *
    *                 pulled from Portland TriMet    *
    *                 developer site                 *
    *                                                *
    *                 Comments to vehicle properties *
    *                 have been pulled directly from *
    *                 TriMet developer documentation *
    *                                                *
    **************************************************/

    public class Vehicle
    {
        // Time this vehicle's entry should be discarded
        // if no new position information is received from the vehicle

        public string expires { get; set; }

        // Vehicle's over head sign text message

        public string signMessage { get; set; }

        // Midnight of the service day the vehicle is performing service for

        public string serviceDate { get; set; }

        public string loadPercentage { get; set; }

        // Latitude of the vehicle

        public string latitude { get; set; }

        public string nextStopSeq { get; set; }

        public string source { get; set; }

        // Identifies the type of vehicle. Can be "bus" or "rail"

        public string type { get; set; }

        public string blockID { get; set; }

        // Vehicle's full over head sign text message

        public string signMessageLong { get; set; }

        public string lastLocID { get; set; }

        // Location ID (or stopID) of the next stop
        // this vehicle is scheduled to serve

        public string nextLocID { get; set; }

        // Number of seconds since midnight from the scheduleDate
        // that the vehicle is positioned at along its schedule

        public string locationInScheduleDay { get; set; }

        // Will be true when the trip the vehicle is serving is new
        // and was not part of the published schedule

        public string newTrip { get; set; }

        // Longitude of the vehicle

        public string longitude { get; set; }

        // Direction of the route the vehicle is servicing

        public string direction { get; set; }

        public string inCongestion { get; set; }

        // Route number the vehicle is servicing

        public string routeNumber { get; set; }

        // Bearing of the vehicle if available
        // 0 is north, 180 is south

        public string bearing { get; set; }
        public string garage { get; set; }

        // TripID the vehicle is servicing

        public string tripID { get; set; }

        // Delay of the vehicle along its schedule
        // Negative is late. Positive is early

        public string delay { get; set; }
        public string extraBlockID { get; set; }

        // Identifier for the over head sign message
        // This is used internally at TriMet and can be ignored

        public string messageCode { get; set; }

        public string lastStopSeq { get; set; }

        // Identifies the vehicle

        public string vehicleID { get; set; }

        // Time this position was initially recorded

        public string time { get; set; }

        public string offRoute { get; set; }
    }
}

/*************************************************
* End of file GTFS_Data_Processor.cs             *
**************************************************/

