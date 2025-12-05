using UnityEngine;
using System.Collections.Generic;

public class FogOfWarManager : MonoBehaviour
{
    public static FogOfWarManager Instance;

    public int textureResolution = 250; // Higher will impact game performance
    public float worldSize = 100f;
    //public float defaultVisionRadius = 5f;

    private Texture2D fogTexture;
    private Color32[] pixels;
    private byte[,] fogData;
    private MeshRenderer fogRenderer;
    private float fogHeight = 1f;

    private List<FogRevealer> fogRevealers = new List<FogRevealer>();
    public Material fogMaterial;
    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        fogTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
        fogTexture.filterMode = FilterMode.Bilinear;
        fogData = new byte[textureResolution, textureResolution];
        pixels = new Color32[textureResolution * textureResolution];

        // Set entire area into black
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;

        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();

        GameObject fogPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fogPlane.name = "FogPlane";
        fogPlane.transform.position = new Vector3(worldSize / 2f, fogHeight, worldSize / 2f);
        fogPlane.transform.localScale = new Vector3(worldSize, worldSize, 1f);
        fogPlane.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        fogPlane.layer = LayerMask.NameToLayer("CursorPriority");

        fogRenderer = fogPlane.GetComponent<MeshRenderer>();
        fogRenderer.material = fogMaterial;
        fogRenderer.material.mainTexture = fogTexture;

        Vector2 texelSize = new Vector2(1f / textureResolution, 1f / textureResolution);
        fogMaterial.SetVector("_TexelSize", new Vector4(texelSize.x, texelSize.y, 0, 0));

        // Apply the fog update immediately
        UpdateFog();
    }

    void LateUpdate()
    {
        UpdateFog();
    }

    void UpdateFog()
    {
        // Fade current visible to explored (grey)
        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                if (fogData[x, y] == 2)
                    fogData[x, y] = 1;
            }
        }

        // Reveal from all units
        foreach (FogRevealer revealer in fogRevealers)
        {
            RevealCircle(WorldToTex(revealer.gameObject.transform.position), revealer.personalRevealArea);
        }

        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                int index = y * textureResolution + x;
                byte state = fogData[x, y];

                if (state == 0)
                    pixels[index] = Color.black;
                else if (state == 1)
                    //pixels[index] = Color.gray; // Optional make grey pixels
                    pixels[index] = Color.clear;
                else
                    pixels[index] = Color.clear;
            }
        }

        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();

        Vector2 texelSize = new Vector2(1f / textureResolution, 1f / textureResolution);
        fogMaterial.SetVector("_TexelSize", new Vector4(texelSize.x, texelSize.y, 0, 0));
    }

    void RevealCircle(Vector2Int center, float radiusWorld)
    {
        // Convert the radius from world units to texture pixels
        int radiusTex = Mathf.RoundToInt((radiusWorld / worldSize) * textureResolution);

        // Get the x and y coordinate of the circle center in texture space
        int cx = center.x;
        int cy = center.y;

        // Loop over a square region that bounds the circle (from -radiusTex to +radiusTex)
        for (int y = -radiusTex; y <= radiusTex; y++)
        {
            for (int x = -radiusTex; x <= radiusTex; x++)
            {
                // Calculate the actual pixel coordinates in the texture
                int fx = cx + x;
                int fy = cy + y;

                // Skip if the pixel is outside the texture boundaries
                if (fx < 0 || fx >= textureResolution || fy < 0 || fy >= textureResolution) continue;

                if (x * x + y * y <= radiusTex * radiusTex)
                {
                    // Mark this pixel as currently visible (2) in the fogData array
                    fogData[fx, fy] = 2; // Visible
                }
            }
        }
    }

    Vector2Int WorldToTex(Vector3 worldPos)
    {
        float normX = Mathf.InverseLerp(0, worldSize, worldPos.x);
        float normZ = Mathf.InverseLerp(0, worldSize, worldPos.z);

        int x = Mathf.FloorToInt(normX * textureResolution);
        int y = Mathf.FloorToInt(normZ * textureResolution);

        return new Vector2Int(x, y);
    }

    // Units can register/unregister themselves:
    public void RegisterVisionSource(FogRevealer t)
    {
        if (!fogRevealers.Contains(t))
            fogRevealers.Add(t);
    }

    public void UnregisterVisionSource(FogRevealer t)
    {
        fogRevealers.Remove(t);
    }

    void OnDrawGizmosSelected()
    {
        if (fogRenderer == null)
            return;

        Gizmos.color = Color.green;
        Vector3 size = new Vector3(worldSize, 0.1f, worldSize);
        Vector3 fog = fogRenderer.transform.position;
        Gizmos.DrawWireCube(fog, size);
    }
}
