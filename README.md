[![Cesium for Unity Logo](Assets/Cesium_for_Unity-Logo-WhiteBGH.jpg)](https://cesium.com/)

# Cesium for Unity Samples

The Cesium for Unity Samples is a starter project to learn and explore the [Cesium for Unity](https://cesium.com/platform/cesium-unity?utm_source=cesium-unreal&utm_medium=github&utm_campaign=unreal) plugin.

The levels in this project will walk you through the plugin's features and demonstrate global-scale content, applications, and experiences in Unity 3D.

![TODO: Image](Images/melbourne.jpg)
*<p align="center">Photogrammetry of Melbourne, Australia visualized in Unity using Cesium for Unity.<br>Open the <b>02_CesiumMelbourne</b> level in Cesium for Unity Samples to experience it yourself!</p>*

### :rocket: Get Started

1. **[Download Cesium for Unity Samples](https://github.com/CesiumGS/cesium-unity-samples/releases)**.
2. Extract the zip file into a suitable location on your computer.
3. Open Unity Editor and select "Open Project". Then point it to the extracted directory. Unity will load the project and also download Cesium for Unity using the Package Manager.
4. In the Asset Browser, select and load `Assets -> Scenes -> 01_CesiumUnity`. You can also select other levels as you browse and explore the samples.

Have questions? Ask them on the [community forum](https://community.cesium.com).

## :mountain: Level Descriptions

### :one: Level 1 - Cesium World

The sample levels begin with a scene in San Francisco. You'll see [Cesium World Terrain](https://cesium.com/platform/cesium-ion/content/cesium-world-terrain/) and [Cesium OSM Buildings](https://cesium.com/platform/cesium-ion/content/cesium-osm-buildings/) in this level. This level is designed to familiarize you with the core Cesium Game Objects and components like Cesium World Terrain and Cesium Georeference.

Be sure to sign into your Cesium ion account using the Cesium button in the toolbar.

### :two: Level 2 - Melbourne Photogrammetry

In this level, explore high-resolution photogrammetry data of Melbourne. This data is streamed from Cesium ion with the 3D Tiles format, a spatial index for streaming massive 3D content over the web. 3D Tiles makes it possible to stream hundreds of gigabytes of data over the web using hierarchical level of detail, spatial subdivision, and runtime optimizations.

Be sure to change the time of day to create beautiful lighting in your scenes.

See how this level was created with the [Adding Datasets tutorial](https://cesium.com/learn/unreal/unreal-datasets/).

### :three: Level 3 - Denver Photogrammetry with an Unity Character Controller

In this level, explore even more high-resolution photogrammetry data of Denver using a third-person character controller. Whereas Melbourne allowed you to freely fly around and explore, in this level you'll walk right up to the real-world data. When streaming content through Cesium for Unity, physics and gravity will continue to work as expected with your character controllers. For more information on character controllers, check out the [Using Custom Controllers tutorial](https://cesium.com/learn/unreal/unreal-custom-controllers/).

Grab some cool screenshots to share with us as you explore and customize the scenes.

![Photogrammetry of Union Station in Denver, CO captured by [Aerometrex](https://aerometrex.com.au/) visualized in Unity 3D using Cesium for Unity.](Images/aerometrex-denver.jpg)
*<p align="center">Photogrammetry of Union Station in Denver, CO captured by [Aerometrex](https://aerometrex.com.au/).<br>Open <b>03_CesiumDenver</b> in the Cesium for Unity Samples to walk around the dataset.</p>*

### :four: Level 4 - Using Sublevels to Explore the World

You can build experiences in different locations around the world, without even changing levels. In this level, we've added a few locations around the world that you can explore. Enter play mode and jump between locations by pressing the 1-5 keys on your keyboard. These levels are loaded in and georeferenced by working with Unity's Sublevel feature.

You can also add your own locations, like your hometown or your favorite vacation spot. Check out the [Sublevels tutorial](https://cesium.com/learn/unreal/unreal-sublevels/) to learn how.

### :six: Level 6 - Metadata

Cesium for Unity enables you to access metadata encoded in your 3D Tiles datasets. In this level, explore the buildings in New York City and see what information is included in the tileset about each building.

![Metadata of New York City buildings visualized.](Images/metadata.JPG)
*<p align="center">Metadata of New York City buildings visualized.<br>Open <b>06_CesiumMetadata</b> in the Cesium for Unity Samples to explore the New York City's metadata.</p>*

Looking to use metadata in your project? Check out the [Visualizing Metadata tutorial](https://cesium.com/learn/unreal/unreal-visualize-metadata/) to learn more.

### :green_book:License

[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0.html). Cesium for Unity Samples is free to use as starter project for both commercial and non-commercial use.
