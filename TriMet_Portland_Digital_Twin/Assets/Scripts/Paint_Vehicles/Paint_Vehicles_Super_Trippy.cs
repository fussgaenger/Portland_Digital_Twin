using System;
using System.Collections.Generic;
using System.Globalization;

using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;

using UnityEngine;


public class Paint_Vehicles_Super_Trippy : MonoBehaviour
{
    public static List<GTFS_Data_Processor.Vehicle> vehicleList;

    public static GameObject[] prefabs = new GameObject[500];

    public static GameObject arcGISMap;

    public static GameObject redSpherePrefab;
    public static GameObject yellowSpherePrefab;

    public static GameObject prefabsGameObject;

    public static int queryInterval = 10;

    void Start()
    {
        arcGISMap = GameObject.Find("ArcGISMap");

        redSpherePrefab    = (GameObject)Resources.Load("Prefabs/RedSphere_Super_Trippy");
        yellowSpherePrefab = (GameObject)Resources.Load("Prefabs/YellowSphere_Super_Trippy");

        prefabsGameObject = GameObject.Find("Prefabs");


        InvokeRepeating("paintVehicles", 0, queryInterval);
    }


    void paintVehicles()
    {
        vehicleList = GTFS_Data_Processor.vehicleList;

        ArcGISPoint vehiclePosition;

        for (int i = 0; i < vehicleList.Count; i++)
        {
            string vehicleLongitudeString = vehicleList[i].longitude;
            string vehicleLatitudeString = vehicleList[i].latitude;

            double vehicleLongitude = Double.Parse(vehicleLongitudeString, CultureInfo.GetCultureInfo("en-US"));
            double vehicleLatitude  = Double.Parse(vehicleLatitudeString, CultureInfo.GetCultureInfo("en-US"));

 
            if (vehicleList[i].type.Equals("rail") && vehicleList[i].signMessageLong.Contains("MAX"))
            {
                prefabs[i] = Instantiate<GameObject>(yellowSpherePrefab, arcGISMap.transform);

                prefabs[i].transform.parent = prefabsGameObject.transform;

                prefabs[i].GetComponent<VehicleIndex>().vehicleIndex = i;

                prefabs[i].SetActive(true);

                // clean up instantiated prefabs after x seconds

                Destroy(prefabs[i].gameObject, queryInterval);

                vehiclePosition = new ArcGISPoint(vehicleLongitude, vehicleLatitude, 4000, new ArcGISSpatialReference(4326));
            }
            else
            {
                prefabs[i] = Instantiate<GameObject>(redSpherePrefab, arcGISMap.transform);

                prefabs[i].transform.parent = prefabsGameObject.transform;

                prefabs[i].GetComponent<VehicleIndex>().vehicleIndex = i;

                prefabs[i].SetActive(true);

                // clean up instantiated prefabs after x seconds

                Destroy(prefabs[i].gameObject, queryInterval);

                vehiclePosition = new ArcGISPoint(vehicleLongitude, vehicleLatitude, 3000, new ArcGISSpatialReference(4326));
            }

            ArcGISLocationComponent locationComponent = prefabs[i].GetComponent<ArcGISLocationComponent>();

            locationComponent.enabled = true;

            locationComponent.Position = vehiclePosition;
        }
    }
}
