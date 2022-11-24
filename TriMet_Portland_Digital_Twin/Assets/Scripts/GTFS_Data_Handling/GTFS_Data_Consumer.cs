using System.Collections.Generic;
using UnityEngine;


public class GTFS_Data_Consumer : MonoBehaviour
{
    public static bool debugMode = false;


    public static List<GTFS_Data_Processor.Vehicle> vehicleList;

    void Start()
    {
        // update vehicle list every 10 seconds

        InvokeRepeating("consumeVehicleList", 0.0f, 10.0f);
    }

    void consumeVehicleList()
    {
        vehicleList = GTFS_Data_Processor.vehicleList;

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
                Debug.Log("");
            }
        }
    }
}
