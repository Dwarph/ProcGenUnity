using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public IEnumerator SwitchColourMapsOverTime(Color[] a, Color[] b, int width, int height, float TotalTime)
    {
        float startTime = Time.time;
        float elapsedTime = Time.time - startTime;
        Color[] lerpedColourMap = new Color[a.Length];
        while (elapsedTime < TotalTime)
        {
            for (int i = 0; i < a.Length; i++)
            {
                lerpedColourMap[i] = Color.Lerp(a[i], b[i], elapsedTime / TotalTime);
            }
            DrawTexture(TextureGenerator.TextureFromColourMap(lerpedColourMap, width, height));

            elapsedTime = Time.time - startTime;
            yield return null;
        }

    }
}
