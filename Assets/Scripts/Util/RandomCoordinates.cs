using UnityEngine;
using Fusion;

public class RandomCoordinates
{
    public static Vector3 FromBoundsAndY(Bounds bounds, float y)
    {
        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minZ = bounds.min.z;
        float maxZ = bounds.max.z;

        
        float destinationX = Random.Range(minX, maxX);
        float destinationZ = Random.Range(minZ, maxZ);

        return new Vector3(destinationX, y, destinationZ);
    }
}
