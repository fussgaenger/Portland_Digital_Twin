/**************************************************
*                                                 *
* Project          TriMet_Portland_Digital_Twin   *
* Supervisor       Christoph Traun                *
* Author           Winfried Schwan                *
* Participant ID   107023                         *
* Filename         Display_Vehicle_Information.cs *
* Version          1.0                            *
* Summary          This script is responsible     *
*                  for displaying vehicle info    *
*                  at the vehicle information     *
*                  panel                          *
*                                                 *
* Created          2022-08-09 07:30:00            *
* Last modified    2022-08-17 13:30:00            *
*                                                 *
***************************************************/

using System;
using System.Collections.Generic;

using UnityEngine;
using TMPro;


public class Display_Vehicle_Information : MonoBehaviour
{
    //
    // Initializing the class variables...
    //

    //
    // access the List with information on all current
    // vehicles operating
    //

    public static List<GTFS_Data_Processor.Vehicle> vehicleList;

    
    //
    // set up the game objects that are part
    // of the vehicle information panel
    //

    public static GameObject messageGameObject01;
    public static GameObject messageGameObject02;
    public static GameObject messageGameObject03;
    public static GameObject messageGameObject04;


    //
    // set up TextMesh Pro objects to be able
    // change the texts in vehicle information
    // panel
    //

    private TextMeshProUGUI textDisplayMessage01;
    private TextMeshProUGUI textDisplayMessage02;
    private TextMeshProUGUI textDisplayMessage03;
    private TextMeshProUGUI textDisplayMessage04;


    /*************************************************
    *                                                *
    * Method name     Start                          *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         Start will be executed once    *
    *                 the scene is loaded            *
    *                                                *
    *                 Collect the text objects as    *
    *                 part of the vehicle            *
    *                 information panel for on the   *
    *                 fly text changing              *
    *                                                *
    **************************************************/

    void Start()
    {
        //
        // find game objects in project hierarchy
        // that will contain the vehicle information later on
        //

        messageGameObject01 = GameObject.Find("Body_Dynamic_Information_01");
        messageGameObject02 = GameObject.Find("Body_Dynamic_Information_02");
        messageGameObject03 = GameObject.Find("Body_Dynamic_Information_03");
        messageGameObject04 = GameObject.Find("Body_Dynamic_Information_04");

        //
        // get the TextMesh Pro objects to enable
        // text manipulation in panel
        //

        textDisplayMessage01 = messageGameObject01.GetComponent<TextMeshProUGUI>();
        textDisplayMessage02 = messageGameObject02.GetComponent<TextMeshProUGUI>();
        textDisplayMessage03 = messageGameObject03.GetComponent<TextMeshProUGUI>();
        textDisplayMessage04 = messageGameObject04.GetComponent<TextMeshProUGUI>();
    }


    /*************************************************
    *                                                *
    * Method name     OnMouseOver                    *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         This event will be triggered   *
    *                 during runtime when the mouse  *
    *                 pointer hovers over a vehicle  *
    *                 While hovering several pieces  *
    *                 of vehicle information are     *
    *                 pulled form List vehicleList   *
    *                 and displayed in the vehicle   *
    *                 information panel              *
    *                                                *
    *                 1) Sign message of vehicle     *
    *                 2) Name of next stop           *
    *                 3) Name of previous stop       *
    *                 4) Vehicle type                *
    *                 5) Longitude of vehicle        *
    *                 6) Latitude of vehicle         *
    *                                                *
    **************************************************/

    void OnMouseOver()
    {
        int vehicleIndex = GetComponent<VehicleIndex>().vehicleIndex;

        if (vehicleIndex >= 0)
        {
            textDisplayMessage01.text = GTFS_Data_Processor.vehicleList[vehicleIndex].signMessage;

            textDisplayMessage02.text = GTFS_Stops_Data_Loader.getStopName(
                                           Int32.Parse(GTFS_Data_Processor.vehicleList[vehicleIndex].nextLocID));

            textDisplayMessage03.text = GTFS_Stops_Data_Loader.getStopName(
                                           Int32.Parse(GTFS_Data_Processor.vehicleList[vehicleIndex].lastLocID));

            textDisplayMessage04.text = GTFS_Data_Processor.vehicleList[vehicleIndex].type.ToUpper() +
                                        "<br>" +
                                        GTFS_Data_Processor.vehicleList[vehicleIndex].longitude +
                                        "<br>" +
                                        GTFS_Data_Processor.vehicleList[vehicleIndex].latitude;
        }
    }

    /*************************************************
    *                                                *
    * Method name     OnMouseExit                    *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         This event will be fired       *
    *                 during runtime if the mouse    *
    *                 pointer stops hovering over a  *
    *                 vehicle. As soon as this       *
    *                 happens all the text in the    *
    *                 vehicle information panel is   *
    *                 removed                        *
    *                                                *
    **************************************************/

    void OnMouseExit()
    {
        textDisplayMessage01.text = "";
        textDisplayMessage02.text = "";
        textDisplayMessage03.text = "";
        textDisplayMessage04.text = "";
    }
}

/*************************************************
* End of file Display_Vehicle_Information.cs     *
**************************************************/
