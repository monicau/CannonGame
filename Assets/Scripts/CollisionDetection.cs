using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {

	public string slopeName1;
	public string slopeName2;
	private GameObject slope1;
	private GameObject slope2;
	private Vector3[] vertices1;
	private Vector3[] vertices2;
	private float r;
	private float r_34;
	private Vector3[] ballVertices;
	private bool collided;

	// Use this for initialization
	void Start () {
		collided = false;
		slope1 = GameObject.Find (slopeName1);
		slope2 = GameObject.Find (slopeName2);
		vertices1 = slope1.GetComponent<MeshFilter> ().mesh.vertices;
		vertices2 = slope2.GetComponent<MeshFilter> ().mesh.vertices;
		r = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
		r_34 = r * (3.0f / 4.0f);
		ballVertices = new Vector3[8];
	}
	
	// Update is called once per frame
	void Update () {
		//Get vertices of the ball
		ballVertices [0] = new Vector3 (transform.position.x - r, transform.position.y, 0);
		ballVertices [1] = new Vector3 (transform.position.x - r_34, transform.position.y + r_34, 0);
		ballVertices [2] = new Vector3 (transform.position.x, transform.position.y+r, 0);
		ballVertices [3] = new Vector3 (transform.position.x + r_34, transform.position.y+r_34, 0);
		ballVertices [4] = new Vector3 (transform.position.x + r, transform.position.y, 0);
		ballVertices [5] = new Vector3 (transform.position.x + r_34, transform.position.y - r_34, 0);
		ballVertices [6] = new Vector3 (transform.position.x, transform.position.y - r, 0);
		ballVertices [7] = new Vector3 (transform.position.x - r_34, transform.position.y - r_34, 0);
		//Do minkowski difference
		int i = 0;
		while(i<ballVertices.Length && !collided) {
			int j = 0;
			while (j<vertices1.Length && !collided) {
				float x = ballVertices[i].x - vertices1[i].x;
				float y = ballVertices[i].y - vertices1[i].y;
				
				if (Mathf.Abs(x)<2 && Mathf.Abs(y)<5.5) {
					collided = true;
					Debug.Log(x+","+y);
					Debug.Log("COLLIDED!!");
				}
				j++;
			}
			i++;
		}
		i = 0;
		while(i<ballVertices.Length && !collided) {
			int j = 0;
			while (j<vertices2.Length && !collided) {
				float x = ballVertices[i].x - vertices1[i].x;
				float y = ballVertices[i].y - vertices1[i].y;
				
				if (Mathf.Abs(x)<2 && Mathf.Abs(y)<5.5) {
					collided = true;
					Debug.Log(x+","+y);
					Debug.Log("COLLIDED!!");
				}
				j++;
			}
			i++;
		}
	}
}
