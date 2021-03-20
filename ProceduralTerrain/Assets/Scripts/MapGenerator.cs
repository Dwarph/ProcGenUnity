using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Noise;

public class MapGenerator : MonoBehaviour
{
    public struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

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

    private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(noiseMapValues, MAP_CHUNK_SIZE);
        return new MapData()
        {
            heightMap = noiseMap,
            colourMap = TextureGenerator.ColourMapFromNoiseMapAndTerrainTypes(noiseMap, regions, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE)
        };
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        switch (drawMode)
        {
            case DrawMode.COLOUR_MAP:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
                break;

            default:
            case DrawMode.NOISE_MAP:
                Color[] colourMap = TextureGenerator.MonoColourMapFromHeightMap(mapData.heightMap);
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
                break;

            case DrawMode.MESH:
                colourMap = TextureGenerator.ColourMapFromNoiseMapAndTerrainTypes(mapData.heightMap, regions, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE);
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, MeshHeightMultiplier, MeshHeightCurve, levelOfDetail),
                                    TextureGenerator.TextureFromColourMap(colourMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
                break;
        }
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
    }

    public void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    public void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, MeshHeightMultiplier, MeshHeightCurve, levelOfDetail);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
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

public struct MapData
{
    public float[,] heightMap;
    public Color[] colourMap;
}