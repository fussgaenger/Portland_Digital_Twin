   M S B A M L   `   `   `  ���� K  GGTFS_Data_Loader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:TriMetDigitalTwinTriMetDigitalTwin  "    TriMetDigitalTwin.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     1localclr-namespace:TriMetDigitalTwin     -     �Name$  
GTFSWindow�� � Title$ GTFS Realtime Data Loader��$	��557��$	��659�� � Icon$ download_icon.png��.��� $��0,0,2,0q� J� RenderTransformOrigin$ 	0.47,0.63&� � RowDefinitions �� $
��240* �.+��� -    ��Name$ NYC_Map��$
��Left=�$	��865��$	��Top=�$	��649��$��nyc_transparent.jpg��$��
0,1,0,-336q��� $��GTFS Realtime Cockpit��$
��Left=�$��73��$��	10,11,0,0q�$	��Top=�$	��624��$��2q�$��14(��� -    ��Name$ StartProcessingGTFSData��$#��Start Processing GTFS Data...��$
��Left=�$��	23,38,0,0q�$	��Top=�$	��200�� �� 	IsDefault . $��33��$��14(��� -   $ StopProcessingGTFSData��$"��Stop Processing GTFS Data...��$
��Left=�$��
244,38,0,0q�$	��Top=�$	��200�� . $��33��$��14(��� $��Status:��$
��Left=�$��
483,41,0,0q�$	��Top=�$��51��$��14(�$��32���� -    ��Name$ StatusLabel��$
��Idle��$
��Left=�$��
529,41,0,0q�$	��Top=�$��14(��� $��Latest Query Results��$
��Left=�$��73��$��	10,88,0,0q�$	��Top=�$	��624��$��2q�$��14(��� -   $ QueryResultLabel��$
��None��$
��Left=�$��
20,116,0,0q�$	��Top=�$	��584��$��14(��� $��Connection Test��$
��Left=�$��73��$��
10,194,0,0q�$	��Top=�$	��624��$��2q�$��14(��� $��URL GTFS Feed��$
��Left=�$��
21,221,0,0q�$	��Top=�$	��104��$��14(��� $
��Left=�$��24��$��132,225,0,0q�	 �� TextWrapping$
	 Wrap=�$0��*http://developer.trimet.org/ws/v2/vehicles��$	��Top=�$	��285��$��14(��� -   $ CheckConnectionHealth��$ ��Check Connection Health...��$
��Left=�$��435,219,0,0q�$	��Top=�$	��186�� . $��33��$��14(��� $��Query Time Interval��$
��Left=�$��73��$��
10,271,0,0q�$	��Top=�$	��624��$��2q�$��14(��� $8��2The latest GTFS realtime data will be pulled every��$
��Left=�$��
20,297,0,0q�$	��Top=�$	��317��$��14(��� -   
 ��Name$
 QueryInterval��$
��Left=�$��24��$��339,300,0,0q�$
	 Wrap=�$��10��$	��Top=�$��51��$��14(��� $��seconds��$
��Left=�$��397,297,0,0q�$	��Top=�$��82��$��14(��� $��Folder Configuration��$
��Left=�$	��158��$��
10,350,0,0q�$	��Top=�$	��624��$��2q�$��14(��� $��Working Folder��$
��Left=�$��
20,370,0,0q�$	��Top=�$	��104��$��14(��� -	   $
 WorkingFolder��$
��Left=�$��24��$��132,374,0,0q�$
	 Wrap=�$��C:\tmp\gtfs_working\��$	��Top=�$	��186��$��14(��� -
   $ OpenWorkingFolder��$��Open Working Folder...��$
��Left=�$��132,413,0,0q�$	��Top=�$	��186�� . $��33��$��14(��� -   $ PurgeWorkingFolder��$��Purge Working Folder...��$
��Left=�$��132,459,0,0q�$	��Top=�$	��186�� . $��33��$��14(��� $��Archive Folder��$
��Left=�$��331,370,0,0q�$	��Top=�$	��104��$��14(��� -   $
 ArchiveFolder��$
��Left=�$��24��$��435,374,0,0q�$
	 Wrap=�$��C:\tmp\gtfs_archive\��$	��Top=�$	��186��$��14(��� -   $ OpenArchiveFolder��$��Open Archive Folder...��$
��Left=�$��433,413,0,0q�$	��Top=�$	��186�� . $��33��$��14(��� -   $ PurgeArchiveFolder_Copy1��$��Purge Archive Folder...��$
��Left=�$��433,459,0,0q�$	��Top=�$	��186�� . $��33��$��14(� � UseLayoutRounding . 