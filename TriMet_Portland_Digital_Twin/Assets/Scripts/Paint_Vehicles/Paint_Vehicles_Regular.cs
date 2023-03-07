/***************************************************
*                                                  *
* Project          TriMet_Portland_Digital_Twin    *
* Author           ESRI                            *
* Customizing      Winfried Schwan                 *
* Filename         Paint_Vehicles_Regular.cs       *
* Version          1.0                             *
* Summary          This script is responsible      *
*                  for painting the vehicles       *
*                  in the scene                    *
*                                                  *
*                  Yellow and red sphere prefabs   *
*                  symbolize vehicle positions     *
*                  for MAX and bus vehicles        *
*                  respectively                    *
*                                                  *
*                  Part of the prefab is the       *
*                  ArcGISLocationComponent that    *
*                  expects longitude/latitude      *
*                  values to place the vehicles    *
*                  correctly in the scene          *
*                                                  *
* Created          2022-08-10 15:30:00             *
* Last modified    2023-03-27 11:00:00             *
*                                                  *
****************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;

using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;

using UnityEngine;


public class Paint_Vehicles_Regular : MonoBehaviour
{
    //
    // Initializing the class variables...
    //

    public static List<GTFS_Data_Processor.Vehicle> vehicleList;

    public static GameObject[] prefabs = new GameObject[500];

    public static GameObject arcGISMap;

    public static GameObject redSpherePrefab;
    public static GameObject yellowSpherePrefab;

    public static GameObject prefabsGameObject;

    public static int queryInterval = 10;


    /*************************************************
    *                                                *
    * Method name     Start                          *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         Start will be executed once    *
    *                 the scene is loaded            *
    *                                                *
    *                 Initializes the various        *
    *                 prefabs used for drawing the   *
    *                 vehicles on the map            *
    *                 Takes also care of updating    *
    *                 vehicle positions every 10     *
    *                 seconds                        *
    *                                                *
    **************************************************/

    void Start()
    {
        arcGISMap = GameObject.Find("ArcGISMap");

        redSpherePrefab    = (GameObject)Resources.Load("Prefabs/RedSphere_Regular");
        yellowSpherePrefab = (GameObject)Resources.Load("Prefabs/YellowSphere_Regular");

        prefabsGameObject = GameObject.Find("Prefabs");


        //
        // call function paintVehicles every 10 seconds
        // according to value of queryInterval
        //

        InvokeRepeating("paintVehicles", 0, queryInterval);
    }


    /*************************************************
    *                                                *
    * Method name     paintVehicles                  *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         Consumes the vehicle List      *
    *                 that is built by               *
    *                 GTFS_Data_Processor script     *
    *                 Extracts longitude and         *
    *                 latitude to use it for         *
    *                 vehicle placement on the map   *
    *                 Different prefabs are used     *
    *                 for MAX vehicles and buses     *
    *                                                *
    **************************************************/

    void paintVehicles()
    {
        // 
        // pull vehicle data from List
        //

        vehicleList = GTFS_Data_Processor.vehicleList;

        ArcGISPoint vehiclePosition;


        //
        // loop through List of vehicles
        //

        for (int i = 0; i < vehicleList.Count; i++)
        {
            //
            // get vehicle longitude and latitude from List
            //

            string vehicleLongitudeString = vehicleList[i].longitude;
            string vehicleLatitudeString = vehicleList[i].latitude;

            double vehicleLongitude = Double.Parse(vehicleLongitudeString, CultureInfo.GetCultureInfo("en-US"));
            double vehicleLatitude  = Double.Parse(vehicleLatitudeString, CultureInfo.GetCultureInfo("en-US"));


            //
            // if we deal with a MAX vehicle use the yellow sphere prefab
            // assign a vehicle index for later reference
            //

            if (vehicleList[i].type.Equals("rail") && vehicleList[i].signMessageLong.Contains("MAX"))
            {
                //
                // instantiate the yellow sphere prefab for MAX vehicles
                //

                prefabs[i] = Instantiate<GameObject>(yellowSpherePrefab, arcGISMap.transform);


                // 
                // make the Prefabs game object the parent of all instantiated prefabs
                //

                prefabs[i].transform.parent = prefabsGameObject.transform;


                //
                // assign a vehicle index for later reference
                //

                prefabs[i].GetComponent<VehicleIndex>().vehicleIndex = i;


                //
                // make yellow sphere prefab visible
                //

                prefabs[i].SetActive(true);


                //
                // let instantiated prefabs disappear after x seconds
                //

                Destroy(prefabs[i].gameObject, queryInterval);


                //
                // assign longitude and latitude to vehicle
                //

                vehiclePosition = new ArcGISPoint(vehicleLongitude, vehicleLatitude, 4000, new ArcGISSpatialReference(4326));
            }
            else
            {
                //
                // instantiate the red sphere prefab for buses and other vehicles except MAX
                //

                prefabs[i] = Instantiate<GameObject>(redSpherePrefab, arcGISMap.transform);


                // 
                // make the Prefabs game object the parent of all instantiated prefabs
                //

                prefabs[i].transform.parent = prefabsGameObject.transform;


                //
                // assign a vehicle index for later reference
                //

                prefabs[i].GetComponent<VehicleIndex>().vehicleIndex = i;


                //
                // make red sphere prefab visible
                //

                prefabs[i].SetActive(true);


                //
                // let instantiated prefabs disappear after x seconds
                //

                Destroy(prefabs[i].gameObject, queryInterval);


                //
                // assign longitude and latitude to vehicle
                //

                vehiclePosition = new ArcGISPoint(vehicleLongitude, vehicleLatitude, 3000, new ArcGISSpatialReference(4326));
            }

            //
            // get the location component of prefab
            // 

            ArcGISLocationComponent locationComponent = prefabs[i].GetComponent<ArcGISLocationComponent>();

            locationComponent.enabled = true;


            //
            // assgin the vehicle position to location component
            //

            locationComponent.Position = vehiclePosition;
        }
    }
}

/*******************************************
* End of file Paint_Vehicles_Regular.cs    *
********************************************/

