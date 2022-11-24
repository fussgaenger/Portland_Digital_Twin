/**************************************************
*                                                 *
* Project          TriMet_Portland_Digital_Twin   *
* Supervisor       Christoph Traun                *
* Author           Winfried Schwan                *
* Participant ID   107023                         *
* Filename         Escape_Key_Handler.cs          *
* Version          1.0                            *
* Summary          This script is responsible     *
*                  for exiting the application    *
*                  when the ESC key is hit        *
*                                                 *
* Created          2022-11-09 12:00:00            *
* Last modified    2022-11-17 13:30:00            *
*                                                 *
***************************************************/

using UnityEngine;


public class Escape_Key_Handler : MonoBehaviour
{
    /*************************************************
    *                                                *
    * Method name     Update                         *
    * Arguments       none                           *
    * Return value    none                           *
    * Summary         Update will be executed before *
    *                 a frame gets rendered to the   *
    *                 screen                         *
    *                                                *
    *                 If the method detects a hit    *
    *                 ESC key the app will be left   *
    *                                                *
    **************************************************/

    void Update()
    {
        //
        // Check for Escape key before rendering a new 
        // frame
        // Leave application if Escape key is pressed
        //

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

/*************************************************
* End of file Escape_Key_Handler.cs              *
**************************************************/
