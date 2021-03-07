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

    // LOD chunks have to be a factor of chunk size -1, 
    // 240 factors are 2,4,8,10,12 which is just nice c:
    // Max chunk size is 255 as unity has limit of 65025 vertices (255*255)
    public const int MAP_CHUNK_SIZE = 241;

    public DrawMode drawMode;

    [Range(0, 6)] public int levelOfDetail;

    public NoiseMapValues noiseMapValues;
    public float MeshHeightMultiplier = 1;
    public AnimationCurve MeshHeightCurve;
    public bool AutoUpdate = false;
    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(noiseMapValues, MAP_CHUNK_SIZE);

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        switch (drawMode)
        {
            case DrawMode.COLOUR_MAP:
                Color[] terrainTypeColourMap = TextureGenerator.ColourMapFromNoiseMapAndTerrainTypes(noiseMap, regions, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE);
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(terrainTypeColourMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
                break;

            default:
            case DrawMode.NOISE_MAP:
                Color[] monoColourMap = TextureGenerator.MonoColourMapFromHeightMap(noiseMap);
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(monoColourMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
                break;

            case DrawMode.MESH:
                Color[] meshColourMap = TextureGenerator.ColourMapFromNoiseMapAndTerrainTypes(noiseMap, regions, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE);
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, MeshHeightMultiplier, MeshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(meshColourMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
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
