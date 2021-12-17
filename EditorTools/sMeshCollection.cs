using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class sMeshCollection : MonoBehaviour
{
    Mesh m;

    public void Combine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        int vertexCount = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(true);
            vertexCount += combine[i].mesh.vertices.Length;   
            if (vertexCount > 64000)
            {
                break;
            }

            i++;
        }
        GetComponent<MeshFilter>().sharedMesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true);
        m = GetComponent<MeshFilter>().sharedMesh;
        GetComponent<MeshCollider>().sharedMesh = m;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(sMeshCollection))]
public class MeshCollectionEditor : Editor
{

    sMeshCollection scr;

    public override void OnInspectorGUI()
    {

        scr = (sMeshCollection)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Combine Meshes"))
        {
            scr.Combine();
        }

    }

}
#endif
