﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Noise
{
    public class NoiseMapValues
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int Seed { get; set; }
        public float Scale { get; set; }
        public int Octaves { get; set; }
        public float Persistance { get; set; }
        public float Lacunarity { get; set; }
        public Vector2 Offset { get; set; }
    }

    public static float[,] GenerateNoiseMap(NoiseMapValues noiseParams)
    {
        if (noiseParams.Scale <= 0)
        {
            noiseParams.Scale = float.MinValue;
        }

        float[,] noiseMap = new float[noiseParams.MapWidth, noiseParams.MapHeight];
        System.Random rng = new System.Random(noiseParams.Seed);
        Vector2[] octaveOffsets = new Vector2[noiseParams.Octaves];
        for (int i = 0; i < noiseParams.Octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000) + noiseParams.Offset.x;
            float offsetY = rng.Next(-100000, 100000) + noiseParams.Offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = noiseParams.MapWidth / 2;
        float halfHeight = noiseParams.MapHeight / 2;



        for (int y = 0; y < noiseParams.MapHeight; y++)
        {
            for (int x = 0; x < noiseParams.MapWidth; x++)
            {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;
                for (int i = 0; i < noiseParams.Octaves; i++)
                {
                    //The higher the frequency, the further apart the noise values will be,
                    // which means that the height values will change more rapidly

                    //Each octave samples from a different random place in the noise, using the offset
                    //Subtracting the halfs means we sample from the centre of the noise, allowing our scale to zoom in centrally
                    float sampleX = (x - halfWidth) / noiseParams.Scale * freq + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / noiseParams.Scale * freq + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                    //Change from range 0-1 to -1 to 1 to get more interesting noise
                    perlinValue = perlinValue * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    //Decreases each octave as persistance is less than 1
                    amplitude *= noiseParams.Persistance;
                    //Increases each octave as lacunarity is greater than 1
                    freq *= noiseParams.Lacunarity;
                }

                noiseMap[x, y] = noiseHeight;

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
            }
        }


        //Normalise values back to be between 0 & 1
        for (int y = 0; y < noiseParams.MapHeight; y++)
        {
            for (int x = 0; x < noiseParams.MapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}