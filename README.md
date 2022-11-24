# Portland_Digital_Twin
 Digital Twin of the Portland Public Transport Network

This project sets out to create a Digital Twin of the Portland TriMet Public Transportation network. It leverages the General Transport Feed Specification (GTFS) to get vehicle location data straight from TriMet. Real-time vehicle data are pulled from TriMet's feed to be visualized in a Unity project. The Unity project is enhanced by the ESRI Maps SDK for Unity to enable using latitude/longitude data.

Two applications need to work at the same time that is GTFS Data Loader (retrieves GTFS Realtime data) and Portland Digital Twin (Unity project consuming GTFS Realtime data and taking care of visuals).

![Portland Digital Twin](/Screenshots/Portland_Digital_Twin.png)
