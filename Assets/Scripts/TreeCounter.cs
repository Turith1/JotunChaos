using UnityEngine;

public class TreeCounter : MonoBehaviour
{
    void Start()
    {
        Terrain terrain = Terrain.activeTerrain;

        int treeCount = terrain.terrainData.treeInstances.Length;

        Debug.Log("Total trees: " + treeCount);
    }
}
