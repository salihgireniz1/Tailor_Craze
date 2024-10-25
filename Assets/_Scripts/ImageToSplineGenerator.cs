using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using Sirenix.OdinInspector;

public class ImageToSplineGenerator : MonoBehaviour
{
    public Texture2D image;
    public SplineComputer splineComputer;
    public float pointSpacing = 20f; // Number of filled pixels to skip before adding a new point
    public float zDepth = 0f; // Z axis depth for spline points
    public float splineScale = 1f; // Scale factor for the spline points

    // Button to manually trigger the generation in the editor
    [Button]
    public void GenerateZigZagSplineFromImage()
    {
        List<SplinePoint> points = new List<SplinePoint>();
        int filledPixelCount = 0;

        // Track the x position of the last point added
        int lastX = -1;

        // Loop through the pixels in the image
        for (int y = 0; y < image.height; y++)
        {
            int currentRowLastX = -1; // Keep track of the last x position in the current row

            for (int x = (lastX != -1) ? lastX : 0; x < image.width; x++)
            {
                Color pixelColor = image.GetPixel(x, y);

                // Check if the pixel is filled (non-transparent)
                if (pixelColor.a > 0.1f) // Adjust threshold for transparency if needed
                {
                    filledPixelCount++;

                    // Add a spline point every 'pointSpacing' filled pixels
                    if (filledPixelCount >= pointSpacing)
                    {
                        filledPixelCount = 0;

                        // Create a spline point at the current pixel position, applying the scale only to x and y
                        Vector3 position = new Vector3(x * splineScale, y * splineScale, zDepth);
                        SplinePoint newPoint = new SplinePoint(position);

                        // Add the new point to the list
                        points.Add(newPoint);

                        // Update the current row's last filled pixel
                        currentRowLastX = x;
                    }
                }
            }

            // Update lastX to be the last filled pixel of the current row
            if (currentRowLastX != -1)
            {
                lastX = currentRowLastX;
            }
        }

        // Set the points on the spline computer (only once after all points are added)
        if (points.Count > 0)
        {
            splineComputer.SetPoints(points.ToArray());
        }
    }
}
