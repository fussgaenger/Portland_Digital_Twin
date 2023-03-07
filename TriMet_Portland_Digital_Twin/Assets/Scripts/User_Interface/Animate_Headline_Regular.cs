/*************************************************
*                                                *
* Project          TriMet_Portland_Digital_Twin  *
* Author           Winfried Schwan               *
* Filename         Animate_Headline_Regular.cs   *
* Version          1.0                           *
* Summary          This script is responsible    *
*                  for displaying the scene      *
*                  titles that gives some        *
*                  context on this scene         *
*                                                *
* Created          2022-07-26 08:15:00           *
* Last modified    2023-03-27 11:00:00           *
*                                                *
**************************************************/

using System;

using UnityEngine;
using TMPro;

public class Animate_Headline_Regular : MonoBehaviour
{
    //
    // Initializing the class variables...
    //

    public static GameObject messageGameObject;

    public static Material messageMaterial;

    private TextMeshProUGUI textDisplayMessage;

    private float dilationValue = -1.0f;

    //
    // Timer responsible for changing titles
    // after waitTime seconds
    //

    private float timer = 0.0f;
    private float waitTime = 8.0f;


    //
    // string array to hold the different messages
    //

    private string[] messageArray;

    bool increasing = true;

    int messageCounter = 0;
    int moduloMessageCounter = 0;

    public static int numberOfMessages = 0;

    //
    // Portland local time is also displayed
    // as one of the messages so these variables
    // will hold Cologne local time and Portland
    // local time
    //

    DateTime localDateTime;
    string pacificDateTime;


    /*************************************************
    *                                                *
    * Method name     Start                          *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         Start will be executed once    *
    *                 the scene is loaded            *
    *                                                *
    *                 The message array gets         *
    *                 initialized with message texts *
    *                 I use the dilation/thickness   *
    *                 property of the font to let    *
    *                 the characters fade in and out *
    *                                                *
    **************************************************/

    void Start()
    {
        //
        // get local Cologne time
        //

        localDateTime = DateTime.Now;

        //
        // build a string that holds local Portland time
        //

        string pacificDateTime = localTimeToPacificTime(localDateTime);

        
        //
        // set up the message array that will
        // contain the headlines
        //

        messageArray = new string[10];

        numberOfMessages = 4;

        messageArray[0] = "The TriMet Network in Portland";
        messageArray[1] = "Live from TriMet's GTFS Realtime Feed";
        messageArray[2] = "The \"Vanilla\" Edition";
        messageArray[3] = "Portland Local Time " + pacificDateTime;


        messageGameObject = GameObject.Find("Headline");

        textDisplayMessage = messageGameObject.GetComponent<TextMeshProUGUI>();
        textDisplayMessage.text = messageArray[messageCounter];

        messageMaterial = textDisplayMessage.fontSharedMaterial;

        //
        // initialize the dilation value with -0.1 that is invisible
        //

        messageMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -1.0f);
    }


    /*************************************************
     *                                                *
     * Method name     Update                         *
     * Arguments       none                           *
     * Return value    none                           *
     * Summary         Update is called once per      *
     *                 frame (as a rule of thumb      *
     *                 60 times per seconds)          *
     *                                                *
     *                 Message to be displayed is     *
     *                 changed every 8 seconds        *
     *                 Dilation value of font is      *
     *                 continously changed to achieve *
     *                 a fade in/fade out effect      *
     *                 Dilation value swings from     *
     *                 -0.3 to -1.0 and back again    *
     *                                                *
     **************************************************/

    void Update()
    {
        //
        // set up the timer to handle fade in/out duration
        //

        timer += Time.deltaTime;

        if (timer < waitTime)
        {
            if (increasing)
            {
                //
                // increase dilation value of font
                // using Time.deltaTime to make it more
                // GPU performance independent
                // (Time.deltaTime = seconds between frames)
                //

                dilationValue = messageMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);

                dilationValue += 0.18f * Time.deltaTime;

                messageMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilationValue);

                if (dilationValue >= -0.3)
                {
                    increasing = false;
                }
            }
            else
            {
                //
                // decrease dilation value of font
                // using Time.deltaTime to make it more
                // GPU performance independent
                // (Time.deltaTime = seconds between frames)
                //

                dilationValue = messageMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);

                dilationValue -= 0.18f * Time.deltaTime;

                messageMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilationValue);

                if (dilationValue <= -1.0)
                {
                    increasing = true;
                }
            }
        }
        else
        {
            //
            // update current local time
            //

            localDateTime = DateTime.Now;

            pacificDateTime = localTimeToPacificTime(localDateTime);

            messageArray[3] = "Portland Local Time: " + pacificDateTime;


            //
            // reset timer and change displayed title
            //

            timer = 0;
            dilationValue = -1.0f;
            messageCounter++;

            moduloMessageCounter = messageCounter % numberOfMessages;

            messageMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilationValue);
            textDisplayMessage.text = messageArray[moduloMessageCounter];
        }
    }


    /*************************************************
    *                                                *
    * Method name     localTimeToPacificTime         *
    * Arguments       DateTime localTime             *
    * Return value    string pacificDateTime         *
    * Summary         Converts Cologne local time    *
    *                 to Portland local time         *
    *                                                *
    **************************************************/

    public static string localTimeToPacificTime(DateTime localTime)
    {
        string pacificZoneId = "Pacific Standard Time";

        TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById(pacificZoneId);

        DateTime pacificTime = TimeZoneInfo.ConvertTime(localTime, pacificZone);

        string pacificDateTime = pacificTime.ToString("HH:mm:ss" + " PST");

        return pacificDateTime;
    }
}

/*************************************************
* End of file Animate_Headline_Regular.cs        *
**************************************************/

