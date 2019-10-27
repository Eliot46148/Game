using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration {
    public static double[,] CreateMap(int pixWidth, int pixHeight, float xOrg, float yOrg)
    {
        double[,] pix = new double[pixHeight, pixWidth];
        float y = 0.0F;

        while (y < pixHeight)
        {
            float x = 0.0F;
            while (x < pixWidth)
            {
                float xCoord = xOrg + x / pixWidth;
                float yCoord = yOrg + y / pixHeight;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y, (int)x] = sample;
                x++;
            }
            y++;
        }
        return pix;
    }

    public static double Noise(int x, int y)
    {
        return Mathf.PerlinNoise(x, y);
    }
}
