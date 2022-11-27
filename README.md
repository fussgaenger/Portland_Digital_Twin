# Portland Digital Twin
 Digital Twin of the Portland Public Transport Network
 
![Portland Digital Twin](/Screenshots/Portland_Digital_Twin.png)

This is a random screenshot captured from the Digital Twin "Trippy" Edition. For a more lifelike experience check out the repo's `Videos` folder.

This project sets out to create a Digital Twin of the Portland TriMet Public Transportation network. It leverages the General Transport Feed Specification (GTFS) to get vehicle location data straight from TriMet. Real-time vehicle data are pulled from TriMet's feed to be visualized in a Unity project. The Unity project is enhanced by the ESRI Maps SDK for Unity to enable using latitude/longitude data.

Two applications need to work at the same time that is GTFS Data Loader (retrieves GTFS Realtime data) and Portland Digital Twin (Unity project consuming GTFS Realtime data and taking care of visuals).

The `Binaries` folder contains prebuilt applications.

The GTFS Data Loader can be run by executing `GTFS_Data_Loader.exe` in folder `GTFS_Data_Loader`

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
 
__The fine print on versions...__

Although I expect this app to work with newer software versions I cannot say for sure because this hadn't been tested. So here are some details on the software used in my environment:

1. Unity Editor _(Version 2021.3.4f1 LTS from 2021)_
2. Microsoft Visual Studio Community 2019 _(Version 16.11.16 from 2021)_
3. ESRI ArcGIS Pro _(Version 3.02 from 2022)_
4. ESRI Maps SDK for Unity _(Version 1.0 from 2022)_

