using Sirenix.OdinInspector;
using UnityEngine;

public class ImageNodeCreator : MonoBehaviour
{
    public Texture2D imageTexture;
    public int nodeOffset = 10; // Spacing between nodes
    public GameObject fillerNodePrefab;

    [Button]
    public void CreateNodes()
    {

        // Ensure the texture is readable
        if (imageTexture == null || !imageTexture.isReadable)
        {
            Debug.LogError("Texture is not readable! Enable 'Read/Write' in the texture settings.");
            return;
        }

        int imageHeight = imageTexture.height;
        int imageWidth = imageTexture.width;

        // Loop through texture rows and columns
        for (int y = 0; y < imageHeight; y += nodeOffset)
        {
            bool isReversed = (y / nodeOffset) % 2 == 1; // Zigzag pattern

            if (isReversed)
            {
                for (int x = imageWidth - 1; x >= 0; x -= nodeOffset)
                {
                    CreateNodeAt(x, y);
                }
            }
            else
            {
                for (int x = 0; x < imageWidth; x += nodeOffset)
                {
                    CreateNodeAt(x, y);
                }
            }
        }
    }

    void CreateNodeAt(int x, int y)
    {
        // Fetch the pixel at the given coordinates
        Color pixelColor = imageTexture.GetPixel(x, y);

        // Debug the pixel alpha value to ensure correct transparency detection
        Debug.Log($"Pixel at ({x}, {y}) Alpha: {pixelColor.a}");

        // Skip transparent pixels
        if (pixelColor.a <= 0.1f)
        {
            return;
        }

        // Instantiate the filler node prefab
        var node = Instantiate(fillerNodePrefab, this.transform);
        // Convert the coordinates to anchored position (considering UI space)
        RectTransform rectTransform = node.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Flip y-coordinate for UI space
            rectTransform.anchoredPosition = new Vector2(x, y);
            node.name = $"({x}, {y})";
        }
        else
        {
            Debug.LogError("Prefab must contain a RectTransform component.");
        }
    }
}
