using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MergeMeshes : MonoBehaviour {
	public Material material;
	public Vector3[] vertices;
	public bool left = false;
	public bool right = false;

	private void CreateVerticesList() {
		//Grab list of vertices in this mesh
		Vector3[] verticesList = transform.GetComponent<MeshFilter> ().mesh.vertices;
		//We want to keep only the front facing half of the vertices
		//To do so, we sort the vertices
		verticesList = SortVectorByX (left, verticesList);
		//Remove duplicates by transferring the top half into a HashSet
		HashSet<Vector3> set = new HashSet<Vector3> ();
		for (int i=0; i<verticesList.Length/2; i++) {
			set.Add(verticesList[i]);
		}
		//Transfer back into an array.. There must be a better way but my brain wants to sleep
		Vector3[] prunedList = new Vector3[set.Count];
		int j = 0;
		foreach (Vector3 v in set) {
			prunedList[j] = v;
			j++;
		}
		vertices = prunedList;
	}
	private Vector3[] SortVectorByX(bool leftSlope, Vector3[] vectors) {
		for (int i = 0; i < vectors.Length-1; i++) {
			for (int j = i + 1; j > 0; j--) {
				if (leftSlope) {
					if (vectors[j-1].x < vectors[j].x) {
						Vector3 temp = vectors[j-1];
						vectors[j-1] = vectors[j];
						vectors[j] = temp;
					}
				} else {
					if (vectors[j-1].x > vectors[j].x) {
						Vector3 temp = vectors[j-1];
						vectors[j-1] = vectors[j];
						vectors[j] = temp;
					}
				}
			}
		}
		return vectors;
	}
	public void CombineMeshes(){
		if (transform.GetComponent<MeshFilter>() == null) {
			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
		}
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int x = 0;
		while (x < meshFilters.Length) {
			Mesh mesh = meshFilters[x].sharedMesh;
			combine[x].mesh = mesh;
			combine[x].transform = meshFilters[x].transform.localToWorldMatrix;
			meshFilters[x].gameObject.SetActive(false);
			x++;
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
		transform.gameObject.SetActive(true);
		transform.gameObject.renderer.material = material;
		transform.position = new Vector3(0, 0, 0);
		CreateVerticesList ();
	}
}
