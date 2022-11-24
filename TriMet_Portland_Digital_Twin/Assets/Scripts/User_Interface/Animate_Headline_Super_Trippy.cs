using System;

using UnityEngine;
using TMPro;

public class Animate_Headline_Super_Trippy : MonoBehaviour
{
    public static GameObject messageGameObject;

    public static Material messageMaterial;

    private TextMeshProUGUI textDisplayMessage;

    private float dilationValue = -1.0f;

    private float waitTime = 8.0f;
    private float timer = 0.0f;

    private string[] messageArray;

    bool increasing = true;

    int messageCounter = 0;
    int moduloMessageCounter = 0;

    public static int numberOfMessages = 0;

    DateTime localDateTime;
    string pacificDateTime;

    void Start()
    {
        // time stuff

        localDateTime = DateTime.Now;

        string pacificDateTime = localTimeToPacificTime(localDateTime);

        
        // message stuff

        messageArray = new string[10];

        numberOfMessages = 4;

        messageArray[0] = "The TriMet Network in Portland";
        messageArray[1] = "Live from TriMet's GTFS Realtime Feed";
        messageArray[2] = "The \"Trippy\" Edition";
        messageArray[3] = "Portland Local Time " + pacificDateTime;


        messageGameObject = GameObject.Find("Headline");

        textDisplayMessage = messageGameObject.GetComponent<TextMeshProUGUI>();
        textDisplayMessage.text = messageArray[messageCounter];

        messageMaterial = textDisplayMessage.fontSharedMaterial;

        messageMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -1.0f);
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer < waitTime)
        {
            if (increasing)
            {
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
            // update the current time

            localDateTime = DateTime.Now;

            pacificDateTime = localTimeToPacificTime(localDateTime);

            messageArray[3] = "Portland Local Time: " + pacificDateTime;


            // take care of dilation of font

            timer = 0;
            dilationValue = -1.0f;
            messageCounter++;

            moduloMessageCounter = messageCounter % numberOfMessages;

            messageMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilationValue);
            textDisplayMessage.text = messageArray[moduloMessageCounter];
        }
    }


    public static string localTimeToPacificTime(DateTime localTime)
    {
        string pacificZoneId = "Pacific Standard Time";

        TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById(pacificZoneId);

        DateTime pacificTime = TimeZoneInfo.ConvertTime(localTime, pacificZone);

        string pacificDateTime = pacificTime.ToString("HH:mm:ss" + " PST");

        return pacificDateTime;
    }
}
