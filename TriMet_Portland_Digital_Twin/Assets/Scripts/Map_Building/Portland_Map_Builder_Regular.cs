/***************************************************
*                                                  *
* Project          TriMet_Portland_Digital_Twin    *
* Author           ESRI                            *
* Customizing      Winfried Schwan                 *
* Filename         Portland_Map_Builder_Regular.cs *
* Version          1.0                             *
* Summary          This script is based on ESRI    *
*                  sample code that has been       *
*                  provided under an Apache        *
*                  license, please see license     *
*                  details in the comments below   *
*                                                  *
*                  The code has been customized to *
*                  meet the project requirements   *
*                                                  *
*                  This includes setting up a      *
*                  local scene of the Portland     *
*                  area with a hillshade layer and *
*                  transformed *.shp files which   *
*                  had been prepared in ArcGIS Pro *
*                  and describe the TriMet network *
*                  The *.shp files had been trans- *
*                  formed into scene layer         *
*                  packages that are loaded        *
*                  locally from disk               *
*                                                  *
* Created          2022-08-02 09:30:00             *
* Last modified    2023-03-27 11:00:00             *
*                                                  *
****************************************************/

// Copyright 2022 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//

// ArcGISMapsSDK

using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Samples.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;

using Esri.GameEngine.Extent;
using Esri.GameEngine.Elevation;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Layers;
using Esri.GameEngine.Map;

using Esri.Unity;

using UnityEditor;
using UnityEngine;

using System;
using System.IO;


// This sample code demonstrates the essential API calls to set up an ArcGISMap
// It covers generation and initialization for the necessary ArcGISMapsSDK game objects and components

// Render-In-Editor Mode
// The ExecuteAlways attribute allows a script to run both in editor and during play mode
// You can disable the run-in-editor mode functions by commenting out this attribute and reloading the scene
// NOTE: Hot reloading changes to an editor script doesn't always work. You'll need to restart the scene if you want your code changes to take effect
// You could write an editor script to reload the scene for you, but that's beyond the scope of this sample script
// See the Unity Hot Reloading documentation to learn more about hot reloading: https://docs.unity3d.com/Manual/script-Serialization.html

[ExecuteAlways]
public class Portland_Map_Builder_Regular : MonoBehaviour
{
	private ArcGISMapComponent arcGISMapComponent;
	private ArcGISCameraComponent cameraComponent;

	//
	// WS Customizing
	//
	// Defined ESRI developer key
	//

	public string APIKey = "Enter API Key here";


	//
	// WS Customizing
	//
	// set up positions for map and camera
	//

	private ArcGISPoint originCoordinates = new ArcGISPoint(-122.6883204, 45.4639347, 0, ArcGISSpatialReference.WGS84());

	private ArcGISPoint cameraCoordinates = new ArcGISPoint(-122.679775, 45.057815, 84750.982019, ArcGISSpatialReference.WGS84());


	// This sample event is used in conjunction with a Sample3DAttributes component
	// It passes a layer to a listener to process its attributes
	// The Sample3DAttributes component is not required, so you are free to remove this event and its invocations in both scripts
	// See ArcGISMapsSDK/Samples/Scripts/3DAttributesSample/Sample3DAttributesComponent.cs for more info

	public delegate void SetLayerAttributesEventHandler(ArcGIS3DObjectSceneLayer layer);

	private void Start()
	{
		CreateArcGISMapComponent();
		CreateArcGISCamera();
		CreateViewStateLoggingComponent();
		CreateArcGISMap();
	}

	// The ArcGISMap component is responsible for setting the origin of the map
	// All geographically located objects need to be a parent of this object

	private void CreateArcGISMapComponent()
	{
		arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();

		if (!arcGISMapComponent)
		{
			GameObject arcGISMapGameObject = new GameObject("ArcGISMap");
			arcGISMapComponent = arcGISMapGameObject.AddComponent<ArcGISMapComponent>();
		}

		arcGISMapComponent.OriginPosition = originCoordinates;
		arcGISMapComponent.MapType = ArcGISMapType.Local;

		// To change the Map Type in editor, you can change the Map Type property of the Map component
		// When you change the Map Type, this event will trigger a call to rebuild the map
		// We only want to subscribe to this event once after the necessary game objects are added to the scene

		arcGISMapComponent.MapTypeChanged += new ArcGISMapComponent.MapTypeChangedEventHandler(CreateArcGISMap);
	}

	// ArcGIS Camera and Location components are added to a Camera game object
	// to enable map rendering, player movement and tile loading

	private void CreateArcGISCamera()
	{
		cameraComponent = Camera.main.gameObject.GetComponent<ArcGISCameraComponent>();

		if (!cameraComponent)
		{
			GameObject cameraGameObject = Camera.main.gameObject;

			// The Camera game object needs to be a child of the Map View game object
			// in order for it to be correctly placed in the world

			cameraGameObject.transform.SetParent(arcGISMapComponent.transform, false);

			// We need to add an ArcGISCamera component

			cameraComponent = cameraGameObject.AddComponent<ArcGISCameraComponent>();

			// The Camera Controller component provides player movement to the Camera game object

			cameraGameObject.AddComponent<ArcGISCameraControllerComponent>();

			// The Rebase component adjusts the world origin to account for 32 bit
			// floating point precision issues as the camera moves around the scene

			cameraGameObject.AddComponent<ArcGISRebaseComponent>();
		}

		ArcGISLocationComponent cameraLocationComponent = cameraComponent.GetComponent<ArcGISLocationComponent>();


		if (!cameraLocationComponent)
		{
			// We need to add an ArcGISLocation component...

			cameraLocationComponent = cameraComponent.gameObject.AddComponent<ArcGISLocationComponent>();

			// ...and update its position and rotation in geographic coordinates

	        cameraLocationComponent.Position = cameraCoordinates;

			// rotate the camera as needed

			cameraLocationComponent.Rotation = new ArcGISRotation(359.727499, 32.388506, 359.999979);
		}
	}


	private void CreateViewStateLoggingComponent()
	{
		ArcGISViewStateLoggingComponent viewStateComponent = arcGISMapComponent.GetComponent<ArcGISViewStateLoggingComponent>();

		if (!viewStateComponent)
		{
			viewStateComponent = arcGISMapComponent.gameObject.AddComponent<ArcGISViewStateLoggingComponent>();
		}
	}


	// This function creates the actual ArcGISMap object that will use your data to create a map
	// This is the only function from this script that will get called again when the map type changes

	public void CreateArcGISMap()
	{
		if (APIKey == "")
		{
			Debug.LogError("An API Key must be set on the SampleAPIMapCreator for content to load");
		}

		// Create the Map Document
		// You need to create a new ArcGISMap whenever you change the map type

		ArcGISMap arcGISMap = new ArcGISMap(arcGISMapComponent.MapType);


		//
		// WS Customizing
		//
		// Using elevation as a basemap replacement
		// because the ESRI imagery that is available
		// as basemap does not meet a more stylized look
		//
		// Create the elevation layer
		//

		arcGISMap.Elevation = new ArcGISMapElevation(new ArcGISImageElevationSource("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer", "Elevation", ""));


		//
		// WS Customizing
		//
		// Create ArcGIS layers based on *.slpk filed prepared in ArcGIS Pro
		// then assign materials and add layers to map
		//

		ArcGISImageLayer hillshadeLayer = new ArcGISImageLayer("https://ibasemaps-api.arcgis.com/arcgis/rest/services/Elevation/World_Hillshade_Dark/MapServer/", "Hillshade Layer", 1.0f, true, APIKey);
		arcGISMap.Layers.Add(hillshadeLayer);

		//
		// Add scene layer package describing the TriMet service boundaries
		// (polygon feature)
		//

		string tmBoundariesSlpkPath = Path.Combine(Application.streamingAssetsPath, "tm_boundary.slpk");

		ArcGIS3DObjectSceneLayer tmBoundariesLayer = new ArcGIS3DObjectSceneLayer(tmBoundariesSlpkPath, "TriMet Boundary Layer", 1.0f, true, "");
		tmBoundariesLayer.MaterialReference = new Material(Resources.Load<Material>("Materials/DarkGreyMat_Regular"));
		arcGISMap.Layers.Add(tmBoundariesLayer);


		//
		// Add scene layer package describing all lines of the TriMet
		// network (line feature)
		//

		string tmAllLinesSlpkPath = Path.Combine(Application.streamingAssetsPath, "tm_routes_ALL.slpk");

		ArcGIS3DObjectSceneLayer tmAllLinesLayer = new ArcGIS3DObjectSceneLayer(tmAllLinesSlpkPath, "TriMet All Routes Layer", 1.0f, true, "");
		tmAllLinesLayer.MaterialReference = new Material(Resources.Load<Material>("Materials/BlueMat"));
		arcGISMap.Layers.Add(tmAllLinesLayer);


		//
		// Add scene layer package describing MAX routes of the TriMet
		// network only (line feature)
		//

		string tmMaxLinesSlpkPath = Path.Combine(Application.streamingAssetsPath, "tm_routes_MAX.slpk");

		ArcGIS3DObjectSceneLayer tmMaxLinesLayer = new ArcGIS3DObjectSceneLayer(tmMaxLinesSlpkPath, "TriMet MAX Routes Layer", 1.0f, true, "");
		tmMaxLinesLayer.MaterialReference = new Material(Resources.Load<Material>("Materials/RedMat"));
		arcGISMap.Layers.Add(tmMaxLinesLayer);


		//
		// Add scene layer package describing MAX stops of the TriMet
		// network (point feature)
		//

		string tmMaxStopsSlpkPath = Path.Combine(Application.streamingAssetsPath, "tm_stops_MAX.slpk");

		ArcGIS3DObjectSceneLayer tmMaxStopsLayer = new ArcGIS3DObjectSceneLayer(tmMaxStopsSlpkPath, "TriMet MAX Stops Layer", 1.0f, true, "");
		tmMaxStopsLayer.MaterialReference = new Material(Resources.Load<Material>("Materials/BlackMat"));
		arcGISMap.Layers.Add(tmMaxStopsLayer);


		// If the map type is local, we will create a rectangle extent and attach it to the map's clipping area

		if (arcGISMap.MapType == ArcGISMapType.Local)
		{
			ArcGISPoint extentCenter = new ArcGISPoint(-122.713204, 45.4639347, 0, ArcGISSpatialReference.WGS84());

			ArcGISExtentRectangle extent = new ArcGISExtentRectangle(extentCenter, 400000, 400000);

			try
			{
				arcGISMap.ClippingArea = extent;
			}
			catch (Exception exception)
			{
				Debug.Log(exception.Message);
			}
		}

		// We have completed setup and are ready to assign the ArcGISMap object to the View

		arcGISMapComponent.View.Map = arcGISMap;

		

#if UNITY_EDITOR
		// The editor camera is moved to the position of the Camera game object when the map type is changed in editor

		if (!Application.isPlaying && SceneView.lastActiveSceneView != null)
		{
			SceneView.lastActiveSceneView.pivot = cameraComponent.transform.position;
			SceneView.lastActiveSceneView.rotation = cameraComponent.transform.rotation;
		}
#endif
	}
}

/*************************************************
* End of file Portland_Map_Builder_Regular.cs    *
**************************************************/

