
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public bool isNan(float x)
    {
        if (x != x)
        {
            return true;
        }
        return false;
    }

    public float Remap (float value, float from1, float to1, float from2, float to2) 
    {
        if (value > to1)
        {
            return to1;
        }
        if (value < from1)
        {
            return from1;
        }
        
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public float getNormalDistVal(float minValue, float maxValue)
    {
        float u, v, S;
    
        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);
    
        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
    
        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    public float angleBetweenVectors(Vector3 v1, Vector3 v2)
    {
        float dotproduct = Vector3.Dot(v1,v2);

        // dotproduct doesn't tell us to rotate clockwise or anticlockwise
        Vector3 crossproduct = Vector3.Cross(v1,v2);
        float polarity = crossproduct.z/Mathf.Abs(crossproduct.z);

        return Mathf.Acos(dotproduct/(v1.magnitude * v2.magnitude)) * polarity * -1;
    }
}
