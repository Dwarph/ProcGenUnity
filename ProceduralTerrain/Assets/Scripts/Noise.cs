using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Noise
{
    [System.Serializable]
    public struct NoiseMapValues
    {
        public int Seed;
        [Min(0.501f)] public float Scale;
        [Range(1, 29)] public int Octaves;
        [Range(0, 1)] public float Persistance;
        [Min(1)] public float Lacunarity;
        public Vector2 Offset;
        public Vector2 Centre;
    }

    private const int RNG_RANGE = 10000;

    public static float[,] GenerateNoiseMap(NoiseMapValues noiseParams, int mapChunkSize)
    {
        if (noiseParams.Scale <= 0)
        {
            noiseParams.Scale = float.MinValue;
        }

        float[,] noiseMap = new float[mapChunkSize, mapChunkSize];
        System.Random rng = new System.Random(noiseParams.Seed);
        Vector2[] octaveOffsets = new Vector2[noiseParams.Octaves];
        for (int i = 0; i < noiseParams.Octaves; i++)
        {
            Vector2 offset = new Vector2(){
                x = rng.Next(-RNG_RANGE, RNG_RANGE) + noiseParams.Offset.x + noiseParams.Centre.x,
                y = rng.Next(-RNG_RANGE, RNG_RANGE) + noiseParams.Offset.y + noiseParams.Centre.y
            };
            octaveOffsets[i] = offset;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapChunkSize / 2;
        float halfHeight = mapChunkSize / 2;



        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
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
                    Vector2 sample = new Vector2(){
                        x = (x - halfWidth) / noiseParams.Scale * freq + octaveOffsets[i].x,
                        y = (y - halfHeight) / noiseParams.Scale * freq + octaveOffsets[i].y
                    };

                    float perlinValue = Mathf.PerlinNoise(sample.x, sample.y);

                    //Change from range (0 - 1) to (-1 - 1) to get more interesting noise
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
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
