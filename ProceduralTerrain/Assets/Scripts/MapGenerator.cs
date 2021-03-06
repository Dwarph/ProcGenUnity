using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Noise;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NOISE_MAP,
        COLOUR_MAP,
        MESH
    }

    public DrawMode drawMode;
    public NoiseMapValues noiseMapValues;
    public bool AutoUpdate = false;
    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(noiseMapValues);

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        switch (drawMode)
        {
            case DrawMode.COLOUR_MAP:
                Color[] terrainTypeColourMap = TextureGenerator.ColourMapFromNoiseMapAndTerrainTypes(noiseMap, regions, noiseMapValues.MapWidth, noiseMapValues.MapHeight);
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(terrainTypeColourMap, noiseMapValues.MapWidth, noiseMapValues.MapHeight));
                break;

            default:
            case DrawMode.NOISE_MAP:
                Color[] monoColourMap = TextureGenerator.MonoColourMapFromHeightMap(noiseMap);
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(monoColourMap, noiseMapValues.MapWidth, noiseMapValues.MapHeight));
                break;

            case DrawMode.MESH:
                Color[] meshColourMap = TextureGenerator.ColourMapFromNoiseMapAndTerrainTypes(noiseMap, regions, noiseMapValues.MapWidth, noiseMapValues.MapHeight);
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColourMap(meshColourMap, noiseMapValues.MapWidth, noiseMapValues.MapHeight));
                break;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
