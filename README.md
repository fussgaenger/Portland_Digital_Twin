# Portland Digital Twin
 Digital Twin of the Portland Public Transport Network
 
![Portland Digital Twin](/Screenshots/Portland_Digital_Twin.png)

This is a random screenshot captured from the Digital Twin "Trippy" Edition. For a more lifelike experience check out the repo's `Videos` folder.

This project sets out to create a kind of Digital Twin of the Portland TriMet Public Transportation network. It leverages the General Transport Feed Specification (GTFS) to get vehicle location data straight from TriMet. Real-time vehicle data are pulled from TriMet's feed to be visualized in a Unity project. The Unity project is enhanced by the ESRI Maps SDK for Unity to enable using spatial information as in latitude/longitude data.

Two applications need to be run at the same time.

1. GTFS Data Loader _(retrieves GTFS Realtime data)_
2. Portland Digital Twin _(Unity project consuming GTFS Realtime data and taking care of visuals)_

The GTFS Data Loader can be run by executing `GTFS_Data_Loader.exe` from folder `GTFS_Data_Loader`

Different versions of the Portland Digital Twin can be run by executing `TriMet_Portland_DigitalTwin.exe` in folders 

1. `Digital_Twin_Vanilla_Fullscreen`
2. `Digital_Twin_Vanilla_Windowed`
3. `Digital_Twin_Trippy_Fullscreen`
4. `Digital_Twin_Trippy_Windowed`

 <br />
 
__Important notes on setup__

Please note that a couple of folders needs to be set up before running the application. Please create the following folders:

`C:\tmp\gtfs_working`
`C:\tmp\gtfs_archive`
`C:\tmp\gtfs_processed`
`C:\tmp\gtfs_stops`

Please make also sure that the file `stops.txt` that can be found at the top level of this repo is copied to `C:\tmp\gtfs_stops`. This file contains the stop names of the network that are needed to feed the info panel in the app.

All Digital_Twin_* applications can be left at any point in time by hitting the ESC key. This might be especially helpful when running the full screen applications. 

<br />

__Developer Key Alert!__

Please note that a developer key issued by ESRI is needed to access various assets like basemaps from the ESRI universe. You can easily apply for an ESRI developer key by hitting https://developers.arcgis.com/. Please make sure that the gathered key is used to update the following files at folder `Portland_Digital_Twin/TriMet_Portland_Digital_Twin/Assets/Scripts/Map_Building/`:

`Portland_Map_Builder_Regular.cs`

Line 84

`Portland_Map_Builder_Super_Trippy.cs`

Line 44

Another key alias "Application ID" is needed from TrimMet. Again hit https://developer.trimet.org/appid/registration/ to apply for an Application ID. For this one you need to navigate to folder `Portland_Digital_Twin/GTFS_Data_Loader/GTFS_Data_Loader/` and modify file 

`MainWindow.xaml.cs`

Line 51

After updating these files you should be good to go...

<br />
 
__The fine print on versions finally...__

Although I expect this app to work with newer software versions I cannot say for sure because this hadn't been tested. So here are some details on the software versions used in my environment:

1. Unity Editor _(Version 2021.3.4f1 LTS from 2021)_
2. Microsoft Visual Studio Community 2019 _(Version 16.11.16 from 2021)_
3. ESRI ArcGIS Pro _(Version 3.02 from 2022)_
4. ESRI Maps SDK for Unity _(Version 1.0 from 2022)_

