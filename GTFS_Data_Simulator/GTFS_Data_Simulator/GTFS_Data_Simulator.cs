/*************************************************
*                                                *
* Project          TriMet_Portland_Digital_Twin  *
* Supervisor       Christoph Traun               *
* Author           Winfried Schwan               *
* Participant ID   107023                        *
* Filename         GTFS_Data_Simulator.cs        *
* Version          1.01                          *
* Summary          This script simulates a GTFS  *
*                  feed by copying previously    *
*                  captured GTFS files from      *
*                  one local folder to another   *
*                  to mimick incoming live data  *
*                                                *
*                  This script is meant to be    *
*                  a "safety net" when no        *
*                  internet connections is       *
*                  available to capture a "live" *
*                  GTFS feed                     *
*                                                *
* Created at       2022-09-11 12:00:00           *
* Last modified    2022-28-11 08:00:00           *
*                                                *
**************************************************/

using System;
using System.IO;
using System.Threading;


namespace TriMetDigitalTwin
{
    class TriMetDataSimulator
    {
        /*************************************************
        * Set up class variables                         *
        **************************************************/

        const string SOURCE_FOLDER      = @"C:\tmp\gtfs_data_offline\";
        const string DESTINATION_FOLDER = @"C:\tmp\gtfs_working\";

        static string gtfsOfflineDataFolderPath;
        static string gtfsWorkingFolderPath;

        static int copyIntervalInMilliseconds  = 10000;


        /*************************************************
        * Main class                                     *
        **************************************************/

        static void Main(string[] args)
        {
            //
            // print welcome message
            //

            printWelcomeMessage();


            //
            // check number of command line arguments if any
            //

            int numberOfCommandLineArguments = checkCommandLineArguments(args);


            //
            // If number of command line arguemtns is 2
            // we assume that custom input/output folders
            // should be used otherwise we take the 
            // default values for input/output folders
            //

            if(numberOfCommandLineArguments == 2)
            {
                gtfsOfflineDataFolderPath = args[0];
                gtfsWorkingFolderPath     = args[1];
            }
            else
            {
                gtfsOfflineDataFolderPath = SOURCE_FOLDER;
                gtfsWorkingFolderPath     = DESTINATION_FOLDER;
            }


            //
            // Read all GTFS files in input folder
            //

            string[] sortedGtfsFiles = readAndSortGtfsFiles();


            //
            // Let's see how many GTFS files we've got
            //

            int numberofGtfsFiles = sortedGtfsFiles.Length;


            //
            // Loop through file name array and copy one file
            // to working folder every 10 seconds
            //

            for(int i = 0; i < numberofGtfsFiles; i++)
            {
                string gtfsFileName = Path.GetFileName(sortedGtfsFiles[i]);

                Console.WriteLine("Copying file \"" + gtfsFileName + 
                                  "\" from \""      + gtfsOfflineDataFolderPath + 
                                  "\" to \""        + gtfsWorkingFolderPath + 
                                  "\"");

                File.Copy(sortedGtfsFiles[i], Path.Combine(gtfsWorkingFolderPath, gtfsFileName), true);


                //
                // Wait 10 seconds until the next file is copied
                // mimicking the query interval with the live GTFS feed
                //

                Thread.Sleep(copyIntervalInMilliseconds);


                //
                // check if any key has been pressed at console window
                // if so break out from loop and exit program
                //

                if (Console.KeyAvailable)
                {
                    printExitMessage();
                    break;
                }
            }
        }


        /*************************************************
        *                                                *
        * Method name     printWelcomeMessage            *
        * Arguments       none                           *
        * Return value    none                           *
        * Summary         Prints a welcome message that  *
        *                 indicates that program has     *
        *                 successfully started           *
        *                                                *
        **************************************************/

        static void printWelcomeMessage()
        {
            Console.WriteLine("****************************************");
            Console.WriteLine("*                                      *");
            Console.WriteLine("*   Welcome to TriMet Data Simulator   *");
            Console.WriteLine("*                                      *");
            Console.WriteLine("****************************************");
        }


        /*************************************************
        *                                                *
        * Method name     printExitMessage               *
        * Arguments       none                           *
        * Return value    none                           *
        * Summary         Prints an exit message that    *
        *                 indicates that program has     *
        *                 been terminated                *
        *                                                *
        **************************************************/

        static void printExitMessage()
        {
            Console.WriteLine();
            Console.WriteLine("****************************************");
            Console.WriteLine("*                                      *");
            Console.WriteLine("*   Leaving TriMet Data Simulator...   *");
            Console.WriteLine("*                                      *");
            Console.WriteLine("****************************************");
            Console.WriteLine();
        }


        /*************************************************
        *                                                *
        * Method name     checkCommandLineArguments      *
        * Arguments       string[] arguments             *
        * Return value    int numberOfArguments          *
        * Summary         Checks if there are any        *
        *                 command line arguments and     *
        *                 if so how many                 *
        *                                                *
        **************************************************/

        static int checkCommandLineArguments(string[] arguments)
        {
            if (arguments.Length == 0)
            {
                Console.WriteLine("No command line arguments found.");
            }
            else
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    Console.WriteLine("Command line argument " + i + ": " + arguments[i]);
                }
            } 

            Console.WriteLine();

            int numberOfArguments = arguments.Length;

            return numberOfArguments;
        }


        /*************************************************
        *                                                *
        * Method name     readAndSortGtfsFiles           *
        * Arguments       none                           *
        * Return value    string[] gtfsOfflineFiles      *
        * Summary         Checks given folder and        *
        *                 collects all GTFS file names   *
        *                 in folder to prepare for file  *
        *                 copy                           *
        *                                                *
        **************************************************/

        static string[] readAndSortGtfsFiles()
        {
            string[] gtfsOfflineFiles = Directory.GetFiles(gtfsOfflineDataFolderPath, "*.json");

            Array.Sort(gtfsOfflineFiles);

            return gtfsOfflineFiles;
        }
    }
}

/*************************************************
* End of file GTFS_Data_Simulator.cs             *
**************************************************/
