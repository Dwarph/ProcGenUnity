using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Noise;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NOISE_MAP,
        COLOUR_MAP
    }

    public enum SwitchMode
    {
        INSTANT,
        LERPED
    }

    public DrawMode drawMode;
    public SwitchMode switchMode;
    public float lerpColourMapTime;
    public NoiseMapValues noiseMapValues;
    public bool AutoUpdate = false;
    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(noiseMapValues);

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        if (switchMode == SwitchMode.INSTANT)
        {
            InstantSwitch(mapDisplay, noiseMap);
        }
        else if (switchMode == SwitchMode.LERPED)
        {
            LerpedSwitch(mapDisplay, noiseMap);
        }
    }
    private void InstantSwitch(MapDisplay mapDisplay, float[,] noiseMap)
    {
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
        }
    }

    private void LerpedSwitch(MapDisplay mapDisplay, float[,] noiseMap)
    {
        Color[] terrainTypeColourMap = TextureGenerator.ColourMapFromNoiseMapAndTerrainTypes(noiseMap, regions, noiseMapValues.MapWidth, noiseMapValues.MapHeight);
        Color[] monoColourMap = TextureGenerator.MonoColourMapFromHeightMap(noiseMap);
        switch (drawMode)
        {
            case DrawMode.COLOUR_MAP:
                IEnumerator colourMapCoroutine = mapDisplay.SwitchColourMapsOverTime(monoColourMap, terrainTypeColourMap, noiseMapValues.MapWidth, noiseMapValues.MapHeight, lerpColourMapTime);
                StartCoroutine(colourMapCoroutine);
                break;

            default:
            case DrawMode.NOISE_MAP:
                IEnumerator noiseMapCoroutine = mapDisplay.SwitchColourMapsOverTime(terrainTypeColourMap, monoColourMap, noiseMapValues.MapWidth, noiseMapValues.MapHeight, lerpColourMapTime);
                StartCoroutine(noiseMapCoroutine);
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
