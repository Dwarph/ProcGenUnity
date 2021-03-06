using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Noise;

public class MapGenerator : MonoBehaviour
{
    public NoiseMapValues noiseMapValues;
    public bool AutoUpdate = false;
    public TerrainType[] regions;


    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(noiseMapValues);
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        mapDisplay.DrawNoiseMap(noiseMap);
    }
}

[System.Serializable]
public struct TerrainType{
    public string name;
    public float height;
    public Color colour;
}
