using UnityEngine;
using System.Collections;

public class MergeMeshes : MonoBehaviour {

	public Material material;
	private bool merged;

	void Start() {
		merged = false;
	}
	void Awake() {
	}
	void Merge () {

		merged = true;
		//Merge the meshes
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int x = 0;
		while (x < meshFilters.Length) {
			combine[x].mesh = meshFilters[x].sharedMesh;
			combine[x].transform = meshFilters[x].transform.localToWorldMatrix;
			meshFilters[x].gameObject.active = false;
			x++;
		}
		Debug.Log ("Combine length:" + combine.Length);
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
		transform.gameObject.active = true;
		transform.gameObject.renderer.material = material;
	}
	private void combine(){
		merged = true;
		if(!transform.GetComponent<MeshFilter> () ||  !transform.GetComponent<MeshRenderer> () )
		{
			transform.gameObject.AddComponent<MeshFilter>();
			transform.gameObject.AddComponent<MeshRenderer>();
		}
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while(i<meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[0].transform.localToWorldMatrix;
			i++;
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
		transform.gameObject.active = true;
		transform.gameObject.renderer.material = material;
	}
	void Update() {
	}
}
