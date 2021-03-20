﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float MAX_VIEW_DISTANCE = 450;
    public Transform viewer;
    public Material mapMaterial;
    public static Vector2 viewerPos;
    private static MapGenerator mapGenerator;
    private int chunkSize;
    private int chunksVisibleInViewDistance;
    private Dictionary<Vector2, TerrainChunk> TerrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> TerrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.MAP_CHUNK_SIZE - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(MAX_VIEW_DISTANCE / chunkSize);
    }

    private void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        //Feel like this is not the most performant way to do this
        foreach (TerrainChunk terrainChunk in TerrainChunksVisibleLastUpdate)
        {
            terrainChunk.SetVisible(false);
        }
        TerrainChunksVisibleLastUpdate.Clear();

        Vector2 currentChunkCoord = new Vector2()
        {
            x = Mathf.RoundToInt(viewerPos.x / chunkSize),
            y = Mathf.RoundToInt(viewerPos.y / chunkSize),
        };

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset < chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 offset = new Vector2(xOffset, yOffset);
                Vector2 viewedChunkCoord = currentChunkCoord + offset;
                if (TerrainChunkDict.ContainsKey(viewedChunkCoord))
                {
                    TerrainChunkDict[viewedChunkCoord].UpdateTerrainChunk(viewerPos);

                    if (TerrainChunkDict[viewedChunkCoord].IsVisible())
                    {
                        TerrainChunksVisibleLastUpdate.Add(TerrainChunkDict[viewedChunkCoord]);
                    }
                }
                else
                {
                    TerrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);
            meshObject =  new GameObject("TerrainChunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = position3D;
            meshObject.transform.parent = parent;
            SetVisible(false);

            mapGenerator.RequestMapData(OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData){
            mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        }

        private void OnMeshDataReceived(MeshData meshData){
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk(Vector2 viewerPosition)
        {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge < MAX_VIEW_DISTANCE;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
