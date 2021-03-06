using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Noise;

public class MapGenerator : MonoBehaviour
{

    [Min(1)] public int mapWidth;
    [Min(1)] public int mapHeight;
    public float noiseScale;
    [Min(0)] public int octaves;
    [Range(0, 1)] public float persistance;
    [Min(1)] public float lacunarity;
    public int seed;
    public Vector2 offset;

    public bool AutoUpdate = false;


    public void GenerateMap()
    {
        NoiseMapValues noiseMapValues = new NoiseMapValues()
        {
            MapWidth = mapWidth,
            MapHeight = mapHeight,
            Scale = noiseScale,
            Octaves = octaves,
            Persistance = persistance,
            Lacunarity = lacunarity,
            Seed = seed,
            Offset = offset
        };


        float[,] noiseMap = Noise.GenerateNoiseMap(noiseMapValues);
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        mapDisplay.DrawNoiseMap(noiseMap);
    }
}
