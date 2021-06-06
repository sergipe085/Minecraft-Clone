using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    static int   maxHeight  = 150;
    static float smooth     = 0.01f;
    static int   octaves    = 4;
    static float persistent = 0.5f;

    public static int GenerateBedrockHeight(float x, float z) {
        float height = Map(0, 10, 0, 1, FBM(x * smooth * 30, z * smooth * 30, octaves + 1, persistent));
        return (int)height;
    }

    public static int GenerateStoneHeight(float x, float z) {
        float height = Map(0, maxHeight - 15, 0, 1, FBM(x * smooth, z * smooth, octaves, persistent));
        return (int)height;
    }

    public static int GenerateHeight(float x, float z) {
        float height = Map(0, maxHeight, 0, 1, FBM(x * smooth, z * smooth, octaves, persistent));
        return (int) height;
    }

    public static float FBM3D(float x, float y, float z, float smooth, float oct, float pers) {

        float XY = FBM(x * smooth, y * smooth, oct, pers);
        float YZ = FBM(y * smooth, z * smooth, oct, pers);
        float XZ = FBM(x * smooth, z * smooth, oct, pers);

        float YX = FBM(y * smooth, x * smooth, oct, pers);
        float ZY = FBM(z * smooth, y * smooth, oct, pers);
        float ZX = FBM(z * smooth, x * smooth, oct, pers);

        return (XY + YZ + XZ + YX + ZY + ZX) / 6.0f;
    }

    static float Map(float newMin, float newMax, float origMin, float origMax, float value) {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(origMin, origMax, value));
    }

    static float FBM(float x, float z, float oct, float pers) {
        float total = 0; 
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        float offset = 64000f;

        for (int i = 0; i < oct; i++) {
            total += Mathf.PerlinNoise((offset + x) * frequency, (offset + z) * frequency) * amplitude;

            maxValue += amplitude;
            amplitude *= pers;
            frequency *= 2;
        }

        return total / maxValue;
    }
}
