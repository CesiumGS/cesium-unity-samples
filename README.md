[![Cesium for Unity Logo](Images~/Cesium_for_Unity-Logo-WhiteBGH.jpg)](https://cesium.com/)

# Cesium for Unity Samples

The Cesium for Unity Samples is a starter project to learn and explore the [Cesium for Unity](https://cesium.com/platform/cesium-for-unity?utm_source=github&utm_medium=github&utm_campaign=unity) plugin.

The scenes in this project will walk you through the plugin's features and demonstrate global-scale content, applications, and experiences in Unity 3D.

The source code for Cesium for Unity itself may be found in the [cesium-unity](https://github.com/CesiumGS/cesium-unity) repo.

![Aerometrex Photogrammetry of San Francisco in Cesium for Unity](Images~/san_francisco.jpg)
*<p align="center">Photogrammetry of San Francisco, California visualized in Unity, using Cesium for Unity.<br>Open the <b>03_CesiumSanFrancisco</b> scene in Cesium for Unity Samples to experience it yourself!</p>*

### :rocket: Get Started

1. **[Download Cesium for Unity Samples](https://github.com/CesiumGS/cesium-unity-samples/releases/latest)**.
2. Extract the `.zip` file into a suitable location on your computer.
3. If you have [Unity Hub](https://unity.com/unity-hub) installed, click the "Open" button under the "Projects" tab. Otherwise, open the Unity Editor directly and select "Open Project". Then, point it to the extracted directory. Unity will load the project and also download Cesium for Unity using the Package Manager.
4. In the Asset Browser, select and load `Assets -> Scenes -> 01_CesiumWorld`. You can also select other scenes as you browse and explore the samples.

Have questions? Ask them on the [community forum](https://community.cesium.com).

## :mountain: Level Descriptions

### :one: Level 1 - Cesium World

The sample levels begin with a scene in San Francisco. You'll see [Cesium World Terrain](https://cesium.com/platform/cesium-ion/content/cesium-world-terrain/) and [Cesium OSM Buildings](https://cesium.com/platform/cesium-ion/content/cesium-osm-buildings/) in this scene. This level is designed to familiarize you with the core Cesium game objects and components, like Cesium World Terrain and Cesium Georeference.

Be sure to sign into your Cesium ion account using the Cesium button in the toolbar.

### :two: Level 2 - Melbourne Photogrammetry

In this scene, explore high-resolution photogrammetry data of Melbourne. This data is streamed from Cesium ion with the 3D Tiles format, a spatial index for streaming massive 3D content over the web. 3D Tiles makes it possible to stream hundreds of gigabytes of data over the web using hierarchical level of detail, spatial subdivision, and runtime optimizations.

See how this scene was created with the [Adding Datasets tutorial](https://cesium.com/learn/unity/unity-datasets/).

### :three: Level 3 - San Francisco Photogrammetry with a Unity Character Controller

In this scene, explore even more high-resolution photogrammetry data of San Francisco using a third-person character controller. Whereas Melbourne allowed you to freely fly around and explore, in this scene you'll walk right up to the real-world data. When streaming content through Cesium for Unity, physics and gravity will continue to work as expected with your character controllers.

Grab some cool screenshots to share with us as you explore and customize the scenes.

![Photogrammetry of the Ferry Building in San Francisco, CA captured by [Aerometrex](https://aerometrex.com.au/) and visualized in Unity 3D using Cesium for Unity.](Images~/ferry_building.jpg)
*<p align="center">Photogrammetry of the Ferry Building in San Francisco, CA captured by [Aerometrex](https://aerometrex.com.au/).<br>Open <b>03_CesiumSanFrancisco</b> in the Cesium for Unity Samples to walk around the dataset.</p>*

### :four: Level 4 - Using Subscenes to Explore the World

With Cesium for Unity, you can build experiences in different locations around the world, without even changing scenes. In this level, we've added a few locations around the world that you can explore. Enter play mode and jump between locations by pressing the 1-4 keys on your keyboard. These levels are loaded in and georeferenced using the `CesiumSubScene` component.

You can also add your own locations, like your hometown or your favorite vacation spot. Check out the [Sub-scenes tutorial](https://cesium.com/learn/unity/unity-subscenes/) to learn how.

### :five: Level 5 - Metadata

Cesium for Unity enables you to access metadata encoded in your 3D Tiles datasets. In this scene, explore the buildings in New York City and see what information is included in the tileset about each building.

![Metadata of New York City buildings visualized.](Images~/metadata.jpg)
*<p align="center">Metadata of New York City buildings visualized.<br>Open <b>05_CesiumMetadata</b> in the Cesium for Unity Samples to explore the New York City buildings' metadata.</p>*

### :six: Level 6 - Point Clouds

Cesium for Unity supports rendering point cloud 3D Tilesets in addition to terrain and photogrammetry datasets. In this scene, you can explore several point cloud datasets around the world. Feel free to add your own point clouds as well.

![Point Cloud of Melbourne, Australia visualized using Cesium for Unity.](Images~/melbourne_point_cloud.jpg)
*<p align="center">Point Cloud of Melbourne, Australia visualized using Cesium for Unity.<br>Open <b>06_CesiumPointClouds</b> in the Cesium for Unity Samples to explore multiple point cloud datasets, including the Melbourne Point Cloud.</p>*

### :seven: Level 7 - Photorealistic 3D Tiles via Google Maps Platform

Explore the world through Photorealistic 3D Tiles streamed via Google Maps Platform. You can learn how to use Photorealistic 3D Tiles in your own 
projects in the [Getting Started with Photorealistic 3D Tiles tutorial](https://cesium.com/learn/unity/unity-photorealistic-3d-tiles).

![The Googleplex in Mountain View, California visualized with Photorealistic 3D Tiles in Cesium for Unity.](Images~/googleplex.jpeg)

*<p align="center">The Googleplex in Mountain View, California visualized with Photorealistic 3D Tiles in Cesium for Unity.<br>Open <b>07_CesiumGoogleMapsTiles</b> in the Cesium for Unity Samples to explore the world as Photorealistic 3D Tiles.</p>*

### :goggles: :one: VR Level 1 - Denver Photogrammetry

Cesium for Unity supports virtual reality platforms. This level is set up for users with VR headsets to get started quickly. In this scene, explore high-resolution photogrammetry data of Denver, Colorado with VR controller support. To build this for the Oculus Quest 2, open **VR01_CesiumDenver** in the Unity Editor and follow the instructions to build an `APK` file for the headset. These instructions also appear in the UI of the scene itself.

1. Ensure that your Unity Editor has Android build support. If not, follow the instructions [here](https://docs.unity3d.com/Manual/android-sdksetup.html) to set it up.
2. Go to `File -> Build Settings` and change the platform to Android.
3. Remove the original sample scene from the scenes list, then click the "Add Open Scenes" button.
4. Go to `Edit -> Project Settings`. Scroll down to find and click on "XR Plug-in Management".
5. Go to the "Android" section and check the box next to "OpenXR".
6. Finally, in the Build Settings window, click "Build and Run" to build an `APK`.

### :green_book:License

[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0.html). Cesium for Unity Samples is free to use as starter project for both commercial and non-commercial use.
