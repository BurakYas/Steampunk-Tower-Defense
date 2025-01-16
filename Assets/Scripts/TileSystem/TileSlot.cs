using System.Collections.Generic;
using UnityEngine;

public class TileSlot : MonoBehaviour
{
    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    private MeshFilter meshFilter => GetComponent<MeshFilter>();

    public void SwitchTile(GameObject referenceTile)
    {
        TileSlot newTile = referenceTile.GetComponent<TileSlot>(); // Get the TileSlot component from the referenceTile GameObject and store it in newTile

        meshFilter.mesh = newTile.GetMesh(); // Set the mesh of the current TileSlot to the mesh of the referenceTile
        meshRenderer.material = newTile.GetMaterial();

        foreach (GameObject obj in GetAllChildren())
        {
            DestroyImmediate(obj);
        }

        foreach (GameObject obj in newTile.GetAllChildren())
        {
            Instantiate(obj, transform);
        }
    }

    public Material GetMaterial() => meshRenderer.sharedMaterial;
    public Mesh GetMesh() => meshFilter.sharedMesh;

    public List<GameObject> GetAllChildren()
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        return children;
    }
}
