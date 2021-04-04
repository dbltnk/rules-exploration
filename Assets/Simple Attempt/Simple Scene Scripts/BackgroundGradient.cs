using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundGradient : MonoBehaviour
{
    private RawImage img;
    private Texture2D backgroundTexture;
    private GridManagerScript gridManager;

    private float alphaMin = 0.45F;
    private float alphaMax = 0.55F;
    private float alphaCurrent = 0.5f;

    private float uVRectMin = -0.2F;
    private float uVRectMax = 0.2F;
    private float uVRectCurrent = 0f;

    void Start () {
        Color[] colors = new Color[4];
        int speciesCount = 0;
        gridManager = FindObjectOfType<GridManagerScript>();
        // Use the colors of the first 4 species per level, if there are any.
        foreach (SpeciesObject species in gridManager.currentLevel.specificSpecies) {
            colors[speciesCount] = species.color;
            speciesCount++;
            if (speciesCount >= 3) break;
        }
        // If there is only one species, let's make up a random one.
        while (speciesCount < 2) {
            colors[speciesCount] = RandomColor();
            speciesCount++;
        }
        // Fill the list with inverted colors.
        while (speciesCount < 4) {
            int r = Random.Range(0, speciesCount+1);
            colors[speciesCount] = InvertColor(colors[r]);
            speciesCount++;
        }
        // And make sure we don't ever use the same color twice.
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (i != j) {
                    if (colors[i] == colors[j]) colors[j] = RandomColor();
                }
            }
        }

        img = GetComponent<RawImage>();
        backgroundTexture = new Texture2D(2, 2);
        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
        backgroundTexture.filterMode = FilterMode.Bilinear;
        SetColor(colors[0], colors[1], colors[2], colors[3]);
    }
    private void Update () {
        float alpha = Mathf.Lerp(alphaMin, alphaMax, alphaCurrent);
        alphaCurrent += 0.1f * Time.deltaTime;
        if (alphaCurrent > 1.0f) {
            float temp = alphaMax;
            alphaMax = alphaMin;
            alphaMin = temp;
            alphaCurrent = 0.0f;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);

        float uVRect = Mathf.Lerp(uVRectMin, uVRectMax, uVRectCurrent);
        uVRectCurrent += 0.1f * Time.deltaTime;
        if (uVRectCurrent > 1.0f) {
            float temp = uVRectMax;
            uVRectMax = uVRectMin;
            uVRectMin = temp;
            uVRectCurrent = 0.0f;
        }
        img.uvRect = new Rect(uVRect, uVRect, 1f, 1f);
    }

    public void SetColor (Color c1, Color c2, Color c3, Color c4) {
        backgroundTexture.SetPixels(new Color[] { c1, c2, c3, c4 });
        backgroundTexture.Apply();
        img.texture = backgroundTexture;
    }

    Color InvertColor (Color c) {
        Color invertedColor = new Color(1 - c.r, 1 - c.g, 1 - c.b, c.a);
        return invertedColor;
    }

    Color RandomColor() {
        float r = Random.Range(0f, 1f), g = Random.Range(0f, 1f), b = Random.Range(0f, 1f);
        return new Color(r, g, b, 1f);
    }
}
