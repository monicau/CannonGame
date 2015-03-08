using UnityEngine;
using System.Collections;

public class MergeMeshes : MonoBehaviour {
	public Material material;

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
	}
}
