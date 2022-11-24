# Portland_Digital_Twin
 Digital Twin of the Portland Public Transport Network

This project sets out to create a Digital Twin of the Portland TriMet Public Transportation network. It leverages the General Transport Feed Specification (GTFS) to get vehicle location data straight from TriMet. Real-time vehicle data are pulled from TriMet's feed to be visualized in a Unity project. The Unity project is enhanced by the ESRI Maps SDK for Unity to enable using latitude/longitude data.

Two applications need to work at the same time that is GTFS Data Loader (retrieves GTFS Realtime data) and Portland Digital Twin (Unity project consuming GTFS Realtime data and taking care of visuals).

The `Binaries` folder contains prebuilt applications.

The GTFS Data Loader can be run by executing `GTFS_Data_Loader.exe` in folder `GTFS_Data_Loader`

Different versions of the Portland Digital Twin can be run by executing `TriMet_Portland_DigitalTwin.exe` in folders 

1. `Digital_Twin_Vanilla_Fullscreen`
2. `Digital_Twin_Vanilla_Windowed`
3. `Digital_Twin_Trippy_Fullscreen`
4. `Digital_Twin_Trippy_Windowed`

![Portland Digital Twin](/Screenshots/Portland_Digital_Twin.png)
