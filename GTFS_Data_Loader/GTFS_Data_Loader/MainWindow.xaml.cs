/*************************************************
*                                                *
* Project          GTFS Data Loader              *
* Supervisor       Christoph Traun               *
* Author           Winfried Schwan               *
* Participant ID   107023                        *
* Filename         MainWindow.xaml.cs            *
* Version          1.0                           *
* Summary          This script collects GTFS     *
*                  realtime data from the        *
*                  Portland TriMet GTFS feed and *
*                  stores all data in local      *
*                  folders for further           *
*                  processing                    *
*                                                *
* Created          2022-06-26 15:00:00           *
* Last modified    2022-08-16 19:20:00           *
*                                                *
**************************************************/

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Net;
using System.Diagnostics;

using RestSharp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TriMetDigitalTwin
{
    public partial class MainWindow : Window
    {
        /*************************************************
        * Initialize class variables                     *
        **************************************************/

        static public string triMetWebServiceUrl = "http://developer.trimet.org/ws/v2/vehicles";

        static public string archiveFolderPath;
        static public string workingFolderPath;

        static public string triMetDeveloperKey = "***REMOVED***";

        static public int queryIntervalInMilliseconds = 10000;

        static public RestClient connectionTestRestClient;
        static public RestClient triMetRestClient;

        static public RestRequest request;
        static public RestResponse response;

        static public DateTime currentDateTime;

        static public string vehiclesSummary;

        static DispatcherTimer timer;


        public MainWindow()
        {
            InitializeComponent();

            //
            // set up working and archive folders
            // to hold incoming GTFS data
            //

            archiveFolderPath = ArchiveFolder.Text;
            workingFolderPath = WorkingFolder.Text;


            //
            // set up the timer that is controlling
            // the time interval between requests
            //

            timer = new DispatcherTimer();


            //
            // get time interval from UI
            // and pass QueryResultLabel to Timer_Tick method
            //

            timer.Interval = TimeSpan.FromSeconds(Convert.ToInt32(QueryInterval.Text));
            timer.Tick += (s, args) => Timer_Tick(QueryResultLabel);
        }


        /*************************************************
        *                                                *
        * Method name     StartProcessingGTFSData_Click  *
        * Arguments       object sender                  *
        *                 RoutedEventArgs e              *
        * Return value    none                           *
        * Summary         Triggers an event as soon as   *
        *                 user clicks the button "Start  *
        *                 Processing GTFS Data". Event   *
        *                 intializes REST client and     *
        *                 sets up a timer that controls  *
        *                 the data processing thread     *
        *                                                *
        **************************************************/

        private void StartProcessingGTFSData_Click(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "Processing";


            //
            // set up the REST client for accessing the GTFS feed
            //

            triMetRestClient = initializeRestClient();


            //
            // start timer and adapt time interval if necessary
            //

            timer.Interval = TimeSpan.FromSeconds(Convert.ToInt32(QueryInterval.Text));
            timer.Start();
        }


        /*************************************************
        *                                                *
        * Method name     StopProcessingGTFSData_Click   *
        * Arguments       object sender                  *
        *                 RoutedEventArgs e              *
        * Return value    none                           *
        * Summary         Stops the timer and updates    *
        *                 status message                 *
        *                                                *
        **************************************************/

        private void StopProcessingGTFSData_Click(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "Idle";
            timer.Stop();
        }


        /*************************************************
        *                                                *
        * Method name     Timer_Tick                     *
        * Arguments       Label result                   *
        * Return value    none                           *
        * Summary         Associated  timer event that   *
        *                 will be called every x seconds *
        *                 to get latest vehicle          *
        *                 information including location *
        *                                                *
        **************************************************/

        public static void Timer_Tick(Label result)
        {
            getAndSaveVehicleData();

            result.Content = vehiclesSummary;
        }


        /*************************************************
        *                                                *
        * Method name     ConnectionCheck_Click          *
        * Arguments       object sender                  *
        *                 RoutedEventArgs e              *
        * Return value    none                           *
        * Summary         Checks if connection to GTFS   *
        *                 is alive                       *
        *                                                *
        **************************************************/

        private void ConnectionCheck_Click(object sender, RoutedEventArgs e)
        {
            connectionTestRestClient = initializeRestClient();

            //
            // build request to check if TriMet GTFS feed is working
            //

            buildRequest();


            //
            // fire request and hope for an OK response from TriMet server
            //

            try
            {
                response = connectionTestRestClient.Get(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show("   GTFS Feed online...", "GTFS Data Realtime Loader", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("   GTFS Feed offline...", "GTFS Data Realtime Loader", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("   Exception caught... " + ex.Message, "GTFS Data Realtime Loader", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            connectionTestRestClient.Dispose();
        }


        /*************************************************
        *                                                *
        * Method name     QueryInterval_Changed          *
        * Arguments       object sender                  *
        *                 TextChangedEventArgs e         *
        * Return value    none                           *
        * Summary         This event is fired as soon as *
        *                 user is changing the query     *
        *                 interval in UI (which is by    *
        *                 default set to 10 seconds)     *
        *                                                *
        **************************************************/

        private void QueryInterval_Changed(object sender, TextChangedEventArgs e)
        {
            queryIntervalInMilliseconds = Convert.ToInt32(QueryInterval.Text) * 1000;
        }


        /*************************************************
        *                                                *
        * Method name     WorkingFolder_Changed          *
        * Arguments       object sender                  *
        *                 TextChangedEventArgs e         *
        * Return value    none                           *
        * Summary         This event is fired as soon as *
        *                 user is changing the working   *
        *                 folder in UI (which is by      *
        *                 default set to                 *
        *                 D:\tmp\gtfs\working)           *
        *                                                *
        **************************************************/

        private void WorkingFolder_Changed(object sender, TextChangedEventArgs e)
        {
            workingFolderPath = WorkingFolder.Text;
        }


        /*************************************************
        *                                                *
        * Method name     ArchiveFolder_Changed          *
        * Arguments       object sender                  *
        *                 TextChangedEventArgs e         *
        * Return value    none                           *
        * Summary         This event is fired as soon as *
        *                 user is changing the archive   *
        *                 folder in UI (which is by      *
        *                 default set to                 *
        *                 D:\tmp\gtfs\archive)           *
        *                                                *
        **************************************************/

        private void ArchiveFolder_Changed(object sender, TextChangedEventArgs e)
        {
            archiveFolderPath = ArchiveFolder.Text;
        }


        /*************************************************
        *                                                *
        * Method name     OpenWorkingFolder_Click        *
        * Arguments       object sender                  *
        *                 RoutedEventArgs e              *
        * Return value    none                           *
        * Summary         Runs Windows Explorer to       *
        *                 take a look at the working     *
        *                 folder                         *
        *                                                *
        **************************************************/

        private void OpenWorkingFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", workingFolderPath);
        }


        /*************************************************
        *                                                *
        * Method name     OpenArchiveFolder_Click        *
        * Arguments       object sender                  *
        *                 RoutedEventArgs e              *
        * Return value    none                           *
        * Summary         Runs Windows Explorer to       *
        *                 take a look at the archive     *
        *                 folder                         *
        *                                                *
        **************************************************/

        private void OpenArchiveFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", archiveFolderPath);
        }


        /*************************************************
        *                                                *
        * Method name     PurgeArchiveFolder_Click       *
        * Arguments       object sender                  *
        *                 RoutedEventArgs e              *
        * Return value    none                           *
        * Summary         Deletes all files in arhive    *
        *                 folder and notifies user       *
        *                 once deletion of files is      *
        *                 complete                       *
        *                                                *
        **************************************************/

        private void PurgeArchiveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(archiveFolderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(archiveFolderPath);
                FileInfo[] files = directoryInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }

            Thread.Sleep(2000);

            MessageBox.Show("   Archive Folder purged...", "GTFS Data Loader", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        /*************************************************
        *                                                *
        * Method name     PurgeWorkingFolder_Click       *
        * Arguments       object sender                  *
        *                 RoutedEventArgs e              *
        * Return value    none                           *
        * Summary         Deletes all files in working   *
        *                 folder and notifies user       *
        *                 once deletion of files is      *
        *                 complete                       *
        *                                                *
        **************************************************/

        private void PurgeWorkingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(workingFolderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(workingFolderPath);
                FileInfo[] files = directoryInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }

            Thread.Sleep(2000);

            MessageBox.Show("   Working Folder purged...", "GTFS Data Loader", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        /*************************************************
        *                                                *
        * Method name     initializeRestClient           *
        * Arguments       none                           *
        * Return value    RestClient                     *
        * Summary         Initializes the REST client    *
        *                 to prepare for access to       *
        *                 GTFS realtime feed             *
        *                                                *
        **************************************************/

        public static RestClient initializeRestClient()
        {
            RestClientOptions options = new RestClientOptions()
            {
                ThrowOnAnyError = true,
                MaxTimeout = 10000
            };

            RestClient client = new RestClient(options);

            return client;
        }


        /*************************************************
        *                                                *
        * Method name     getAndSaveVehicleData          *
        * Arguments       none                           *
        * Return value    none                           *
        * Summary         Sets up REST request to get    *
        *                 vehicle data from GTFS         *
        *                 feed, send request, format     *
        *                 REST response and write to     *
        *                 working folder                 *
        *                                                *
        **************************************************/

        public static void getAndSaveVehicleData()
        {
            //
            // build request to get vehicle positions from TriMet server
            //

            buildRequest();


            //
            // fire request to get vehicle positions from TriMet GTFS feed
            //

            try
            {
                response = triMetRestClient.Get(request);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot access TriMet web services, exiting now...");
                Console.WriteLine("Exception: " + e.Message);
            }


            //
            // put prettily formatted JSON data in string formattedVehicleList
            //

            object vecicleList = JsonConvert.DeserializeObject(response.Content);
            string formattedVehicleList = JsonConvert.SerializeObject(vecicleList, Formatting.Indented);

 
            //
            // build the timestamp to be added to JSON file name
            //

            string timeStampJsonFile = buildTimeStampForJsonFile();


            //
            // write formatted GTFS to file in "archive" folder along with timestamp
            //

            writeGtfsFileToArchiveFolder(formattedVehicleList, timeStampJsonFile);


            //
            // write formatted GTFS to file in "working" folder along with timestamp
            //

            writeGtfsFileToWorkingFolder(formattedVehicleList, timeStampJsonFile);


            //
            // write file with .complete extension to indicate that saving of JSON file
            // has been completed in "working" folder
            //

            writeGtfsCompleteFileToWorkingFolder(timeStampJsonFile);


            //
            // get the number of vehicles in operation from latest service call
            //

            dynamic allVehicles = JsonConvert.DeserializeObject(formattedVehicleList);

            int numberOfVehicles = 0;

            numberOfVehicles = getNumberOfVehicles(formattedVehicleList);

 
            //
            // get the query's time and date according to received data
            //

            string queryTimeStamp = getQueryTimeStamp(formattedVehicleList);

 
            //
            // output each vehicle's properties
            //

            printVehicleProperties(allVehicles, numberOfVehicles);


            //
            // get current local time CET
            //

            string currentDateTime = MainWindow.currentDateTime.ToString("dd/MM/yyyy HH:mm:ss");

            Console.WriteLine();
            Console.WriteLine(numberOfVehicles + " TriMet vehicle location(s) found on " + currentDateTime + " CET");

            string pstDateTime = localTimeToPacificTime(MainWindow.currentDateTime);

            vehiclesSummary = numberOfVehicles + " TriMet vehicle location(s) found on " + currentDateTime + " CET"
                                     + " / " + pstDateTime + " PST";
        }


        /*************************************************
        *                                                *
        * Method name     buildRequest                   *
        * Arguments       none                           *
        * Return value    none                           *
        * Summary         Build REST request to get data *
        *                 GTFS live feed                 *
        *                                                *
        **************************************************/

        private static void buildRequest()
        {
            request = new RestRequest(triMetWebServiceUrl);
            response = new RestResponse();

            request.AddHeader("User-Agent", "ESRI Unity prototype app");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Connection", "keep-alive");

            request.AddParameter("appID", "***REMOVED***");
        }


        /*************************************************
        *                                                *
        * Method name     getNumberOfVehicles            *
        * Arguments       string vehicleList             *
        * Return value    int numberOfVehicles           *
        * Summary         Get currentnumber of vehicles  *
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
                Console.WriteLine("No vehicles operational");
                Console.WriteLine(exception.Message);

                numberOfVehicles = 0;
            }

            return numberOfVehicles;
        }


        /*************************************************
        *                                                *
        * Method name     getQueryTimeStamp              *
        * Arguments       string vehicleList             *
        * Return value    string queryTimeStamp          *
        * Summary         Get timestamp from GTFS data   *
        *                 and convert time format from   *
        *                 UNIX epoch time to local       *
        *                 Portland time                  *
        *                                                *
        **************************************************/

        public static string getQueryTimeStamp(string vehicleList)
        {
            dynamic allVehicles = JsonConvert.DeserializeObject(vehicleList);

            string queryTime;

            queryTime = allVehicles.resultSet.queryTime;

            string queryTimeStamp = epochTimeToPacificTime(queryTime);

            return queryTimeStamp;
        }


        /*************************************************
        *                                                *
        * Method name     epochTimeToPacificTime         *
        * Arguments       string epochTime               *
        * Return value    string portlandDateTime        *
        * Summary         Get UTC time and date from     *
        *                 UNIX epoch time and transform  *
        *                 to "Pacific Standard Time"     *
        *                 which happens to be the time   *
        *                 where the city of Portland,    *
        *                 Oregon is located in the US    *
        *                                                *
        *                 All Portland time values are   *
        *                 in milliseconds since UNIX     *
        *                 epoch                          *
        *                                                *
        *                  Portland local time is UTC-7  *
        *                                                *
        **************************************************/

        public static string epochTimeToPacificTime(string epochTime)
        {
            //
            // cut the last three digits, we need seconds not milliseconds
            //

            string tmpTimeStamp = epochTime.Substring(0, epochTime.Length - 3);


            //
            // change type from string to long
            //

            long timeStamp = Convert.ToInt64(tmpTimeStamp);


            //
            // unix timestamp is seconds past epoch
            //

            DateTime triMetUtcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            triMetUtcDateTime = triMetUtcDateTime.AddSeconds(timeStamp);

            DateTime utcTime = triMetUtcDateTime;


            //
            // prepare for getting "Pacific Standard Time"
            //

            string pacificZoneId = "Pacific Standard Time";

            TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById(pacificZoneId);

            DateTime pacificTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pacificZone);

            string portlandDateTime = string.Format("{0:u}", pacificTime);

            return portlandDateTime;
        }


        /*************************************************
        *                                                *
        * Method name     buildTimeStampForJsonFile      *
        * Arguments       none                           *
        * Return value    string formattedDateTime       *
        * Summary         Build a time stamp with        *
        *                 current time to be used for    *
        *                 naming of working JSON file    *
        *                                                *
        **************************************************/

        public static string buildTimeStampForJsonFile()
        {
            currentDateTime = DateTime.Now;

            string formattedDateTime = currentDateTime.ToString("_yyyy_MM_dd_HH_mm_ss");

            return formattedDateTime;
        }


        /*************************************************
        *                                                *
        * Method name     writeGtfsFileToArchiveFolder   *
        * Arguments       string formattedVehicleList    *
        *                 string timeStampJsonFile       *
        * Return value    none                           *
        * Summary         Writes JSON file to archive    *
        *                 folder                         *
        *                                                *
        **************************************************/

        private static void writeGtfsFileToArchiveFolder(string formattedVehicleList, string timeStampJsonFile)
        {
            //
            // write formatted JSON to file along with timestamp
            //

            File.WriteAllText(archiveFolderPath + "vehicle_positions" + timeStampJsonFile + ".json", formattedVehicleList);
        }


        /*************************************************
        *                                                *
        * Method name     writeGtfsFileToWorkingFolder   *
        * Arguments       string formattedVehicleList    *
        *                 string timeStampJsonFile       *
        * Return value    none                           *
        * Summary         Writes JSON file to working    *
        *                 folder                         *
        *                                                *
        **************************************************/

        private static void writeGtfsFileToWorkingFolder(string formattedVehicleList, string timeStampJsonFile)
        {
            //
            // write formatted JSON to file along with timestamp
            //

            File.WriteAllText(workingFolderPath + "vehicle_positions" + timeStampJsonFile + ".json", formattedVehicleList);
        }


        /*******************************************************
        *                                                      *
        * Method name     writeGtfsCompleteFileToWorkingFolder *
        * Arguments       string timeStampJsonFile             *
        * Return value    none                                 *
        * Summary         Writes .complete file to working     *
        *                 folder as a signal that writing of   *
        *                 file has finished                    *
        *                                                      *
        ********************************************************/

        private static void writeGtfsCompleteFileToWorkingFolder(string timeStampJsonFile)
        {
            File.WriteAllText(workingFolderPath + "vehicle_positions" + timeStampJsonFile + ".complete", "");
        }


        /*************************************************
        *                                                *
        * Method name     printVehicleProperties         *
        * Arguments       dynamic allVehicles            *
        *                 int numberOfVehicles           *
        * Return value    none                           *
        * Summary         Loops through all captured     *
        *                 vehicles and outputs vehicle   *
        *                 at console (for debugging      *
        *                 purposes)                      *
        *                                                *
        **************************************************/

        public static void printVehicleProperties(dynamic allVehicles, int numberOfVehicles)
        {
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
            string vehicleId;
            string time;
            string offRoute;

            string portlandDateExpires;
            string portlandServiceDate;
            string portlandTime;


            //
            // loop through all vehicles
            // and output properties
            //

            for (int i = 0; i < numberOfVehicles; i++)
            {
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
                vehicleId       = allVehicles.resultSet.vehicle[i].vehicleId;
                time            = allVehicles.resultSet.vehicle[i].time;
                offRoute        = allVehicles.resultSet.vehicle[i].offRoute;


                Console.WriteLine("expires:               " + expires);
                Console.WriteLine("signMessage:           " + signMessage);
                Console.WriteLine("serviceDate:           " + serviceDate);
                Console.WriteLine("loadPercentage:        " + loadPercentage);

                Console.WriteLine("latitude:              " + latitude);
                Console.WriteLine("nextStopSeq:           " + nextStopSeq);
                Console.WriteLine("source:                " + source);
                Console.WriteLine("type:                  " + type);

                Console.WriteLine("blockID:               " + blockID);
                Console.WriteLine("signMessageLong:       " + signMessageLong);
                Console.WriteLine("lastLocID:             " + lastLocID);
                Console.WriteLine("nextLocID:             " + nextLocID);

                Console.WriteLine("locationInScheduleDay: " + locationInScheduleDay);
                Console.WriteLine("newTrip:               " + newTrip);
                Console.WriteLine("longitude:             " + longitude);
                Console.WriteLine("direction:             " + direction);

                Console.WriteLine("inCongestion:          " + inCongestion);
                Console.WriteLine("routeNumber:           " + routeNumber);
                Console.WriteLine("bearing:               " + bearing);
                Console.WriteLine("garage:                " + garage);

                Console.WriteLine("tripID:                " + tripID);
                Console.WriteLine("delay:                 " + delay);
                Console.WriteLine("extraBlockID:          " + extraBlockID);
                Console.WriteLine("messageCode:           " + messageCode);

                Console.WriteLine("lastStopSeq:           " + lastStopSeq);
                Console.WriteLine("vehicleId:             " + vehicleId);
                Console.WriteLine("time:                  " + time);
                Console.WriteLine("offRoute:              " + offRoute);


                portlandDateExpires = epochTimeToPacificTime(expires);
                portlandServiceDate = epochTimeToPacificTime(serviceDate);
                portlandTime = epochTimeToPacificTime(time);

                Console.WriteLine();
                Console.WriteLine("portlandDateExpires PDT: " + portlandDateExpires);
                Console.WriteLine("portlandServiceDate PDT: " + portlandServiceDate);
                Console.WriteLine("portlandTime PDT: " + portlandTime);
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

            string pacificDateTime = pacificTime.ToString("dd/MM/yyyy HH:mm:ss");

            return pacificDateTime;
        }
    }
}

/*************************************************
* End of file MainWindow.xaml.cs                 *
**************************************************/
