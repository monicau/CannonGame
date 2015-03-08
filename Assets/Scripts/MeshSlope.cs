using UnityEngine;
using System.Collections;

public class MeshSlope : MonoBehaviour {

	public Material material;


	// http://answers.unity3d.com/questions/139808/creating-a-plane-mesh-directly-from-code.html
	void Start() {
		Vector3[] cliff = new Vector3[10];
		for (int i = 0; i<cliff.Length; i++) {
			cliff[i] = new Vector3(0, i, 0);
		}
		MidpointBisection (cliff, 0, cliff.Length-1);

		GameObject rightSlope = transform.FindChild ("RightSlope").gameObject;
		for (int i=0; i<cliff.Length-1; i++) {
			Vector3[] vectors = new Vector3[4];
			vectors[0] = new Vector3 (cliff[i].x, cliff[i].y, 0);
			vectors[3] = new Vector3 (1, cliff[i].y, 0);
			vectors[2] = new Vector3 (1, cliff[i+1].y, 0);
			vectors[1] = new Vector3 (cliff[i+1].x, cliff[i+1].y, 0);
			GameObject m = CreateMeshObject("r"+i.ToString(), vectors);
			m.transform.parent = rightSlope.transform;
		}
		rightSlope.transform.position = new Vector3(10, -5, 0);
		rightSlope.GetComponent<MergeMeshes> ().CombineMeshes ();
		
		//Reset cliff variable for second slope
		for (int i = 0; i<cliff.Length; i++) {
			cliff[i] = new Vector3(0, i, 0);
		}
		MidpointBisection (cliff, 0, cliff.Length-1);
		GameObject leftSlope = transform.FindChild ("LeftSlope").gameObject;
		for (int i=0; i<cliff.Length-1; i++) {
			Vector3[] vectors = new Vector3[4];
			vectors[0] = new Vector3 (-cliff[i].x, cliff[i].y, 0);
			vectors[1] = new Vector3 (-1, cliff[i].y, 0);
			vectors[2] = new Vector3 (-1, cliff[i+1].y, 0);
			vectors[3] = new Vector3 (-cliff[i+1].x, cliff[i+1].y, 0);
			GameObject m = CreateMeshObject("l"+i.ToString(), vectors); 
			m.transform.parent = leftSlope.transform;
		}
		leftSlope.transform.position = new Vector3(-10, -5, 0);
		leftSlope.GetComponent<MergeMeshes> ().CombineMeshes ();
	}

	void MidpointBisection(Vector3[] array, int start, int stop) {
		int midpoint = (stop+start) / 2;
		if (midpoint != start && midpoint != stop) {
			//Displace the vector at midpoint
			float maxDisplacement = Mathf.Abs(array[stop].y - array[midpoint].y)/2;
			float displacement = (float) Random.Range(1,maxDisplacement+1);
			float baseLength = Mathf.Abs((array[stop].x + array[midpoint].x)/2.0f);
//			Debug.Log("Displacement at ("+midpoint+"):"+ displacement.ToString() + ", base:" + baseLength.ToString());
			array[midpoint] = array[midpoint] - new Vector3(displacement+baseLength, 0, 0);
			//Recursive call
			if (start<midpoint) {
				MidpointBisection(array, start, midpoint);
			}
			if (midpoint<stop) {
				MidpointBisection(array, midpoint, stop);
			}
		}
	}
	GameObject CreateMeshObject(string name, Vector3[] vectors) {
		GameObject plane = new GameObject(name);
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = CreateMesh(vectors[0], vectors[1], vectors[2], vectors[3]);
		MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material.shader = Shader.Find ("Particles/Additive");
		Texture2D tex = new Texture2D(1, 1);
		tex.SetPixel(0, 0, new Color(185.0f/255, 122f/255, 87f/255));
		tex.Apply();
		renderer.material.mainTexture = tex;
		renderer.material.color = new Color(185.0f/255, 122f/255, 87f/255);
		return plane;
	}
	Mesh CreateMesh(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight, Vector3 topLeft)	{
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = new Vector3[] {
			bottomLeft,
			bottomRight,
			topRight,
			topLeft,
		};
		m.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (0, 1),
			new Vector2(1, 1),
			new Vector2 (1, 0)
		};
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		m.RecalculateNormals();
		
		return m;
	}

}
