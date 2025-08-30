using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // necesario para guardar assets


public class MeshCombiner : MonoBehaviour
{
#if  UNITY_EDITOR
    
    [ContextMenu("Combinar y Guardar Meshes")]
    public void CombineAndSaveMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        Dictionary<Material, List<CombineInstance>> materialToMesh = new();

        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null || mf.GetComponent<MeshRenderer>() == null)
                continue;

            Material mat = mf.GetComponent<MeshRenderer>().sharedMaterial;
            if (!materialToMesh.ContainsKey(mat))
                materialToMesh[mat] = new List<CombineInstance>();

            CombineInstance ci = new CombineInstance
            {
                mesh = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };
            materialToMesh[mat].Add(ci);
        }

        // Carpeta donde se guardarán los assets
        string folderPath = "Assets/CombinedMeshes";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets", "CombinedMeshes");

        GameObject combinedParent = new GameObject("CombinedMeshes");
        combinedParent.transform.SetParent(transform, false);

        int index = 0;
        foreach (var kvp in materialToMesh)
        {
            GameObject go = new GameObject("CombinedMesh_" + index);
            go.transform.SetParent(combinedParent.transform, false);

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(kvp.Value.ToArray(), true, true);

            // Guardar mesh como asset
            string assetPath = $"{folderPath}/CombinedMesh_{name}_{index}.asset";
            AssetDatabase.CreateAsset(combinedMesh, assetPath);
            AssetDatabase.SaveAssets();

            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            mr.sharedMaterial = kvp.Key;

            index++;
        }

        Debug.Log("Meshes combinados y guardados en " + folderPath);
    }
#endif
}