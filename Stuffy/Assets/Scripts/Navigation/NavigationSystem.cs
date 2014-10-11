﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NavigationSystem : MonoBehaviour {
    private List<DynamicMesh> _meshes;
    private List<MeshLink> _meshLinks;

    void Awake() {
        _meshes = new List<DynamicMesh>();
    }

	void Start() {
	
	}
	
	void Update() {

	}

    public void AddMesh() {
        DynamicMesh newMesh = CreateNewMesh();
        
        newMesh.OnVertexLinkCreated += HandleVertexLinkCreated;
        newMesh.OnVertexLinkDestroyed += HandleVertexLinkDestroyed;

        _meshes.Add(newMesh);
    }
    
    public void RemoveMesh() {
        DynamicMesh lastMesh = _meshes[_meshes.Count - 1];
        _meshes.Remove(lastMesh);
        
        lastMesh.OnVertexLinkCreated -= HandleVertexLinkCreated;
        lastMesh.OnVertexLinkDestroyed -= HandleVertexLinkDestroyed;
        
        GameObject.DestroyImmediate(lastMesh.gameObject);
    }

    private void HandleVertexLinkCreated(DynamicVertex v1, DynamicVertex v2) {
        Debug.Log("link created");

        VertexLink vertexLink = new VertexLink(v1, v2);

        // Find existing link between meshes if one exists.
        MeshLink meshLink = GetMeshLinkForVertexLink(vertexLink);

        // No link exists between the two meshes.
        if(meshLink == null) {
            GameObject newGo = new GameObject();
            meshLink = newGo.AddComponent<MeshLink>();
        }

        // User tries to create a third link between two meshes.
        if(meshLink.IsFull()) {
            Debug.LogError("Can only link two pairs of vertices between a mesh.");
            v1.BreakVertexLink();
            return;
        }

        meshLink.AddVertexLink(vertexLink);
    }

    private MeshLink GetMeshLinkForVertexLink(VertexLink vertexLink) {
        foreach(MeshLink meshLink in _meshLinks) {
            if(meshLink.HasMatchingLink(vertexLink))
                return meshLink;
        }

        return null;
    }

    private void HandleVertexLinkDestroyed(DynamicVertex v1, DynamicVertex v2) {
        Debug.Log("link destroyed");
    }

    private DynamicMesh CreateNewMesh() {
        GameObject prototype = (GameObject)Resources.Load("Prefabs/DynamicMesh");
        
        GameObject go = (GameObject)GameObject.Instantiate(prototype);
        go.transform.parent = gameObject.transform;
        go.name = "DynamicMesh" + (_meshes.Count + 1);
        go.transform.localPosition = Vector3.zero;
        
        return go.GetComponent<DynamicMesh>();
    }
    
    private MeshLink CreateNewMeshLink() {
        GameObject prototype = (GameObject)Resources.Load("Prefabs/MeshLink");

        GameObject go = (GameObject)GameObject.Instantiate(prototype);
        go.transform.parent = gameObject.transform;
        go.name = "MeshLink" + (_meshLinks.Count + 1);
        
        return go.GetComponent<MeshLink>();
    }
}