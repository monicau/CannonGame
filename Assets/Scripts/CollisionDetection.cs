using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetection : MonoBehaviour {

	public bool debugOn;
	public float x_stdev;
	public float y_stdev;
	public string slopeName1;
	public string slopeName2;
	public float dampFactor;
	public Material redMat;
	public GameObject dot;
	private GameObject slope1;
	private GameObject slope2;
	private Vector3[] vertices1;
	private Vector3[] vertices2;
	private float r;
	private float r_34;
	private Vector3 ballVertex;
	private bool collided;
	private GameObject[] dots;

	// Use this for initialization
	void Start () {
		collided = false;
		slope1 = GameObject.Find (slopeName1);
		slope2 = GameObject.Find (slopeName2);
		vertices1 = slope1.GetComponent<MergeMeshes>().vertices;
		vertices2 = slope2.GetComponent<MergeMeshes>().vertices;
		r = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
		r_34 = r * (3.0f / 4.0f);
		if (debugOn) {
			dots = new GameObject[vertices1.Length];
			for (int i=0; i<vertices1.Length; i++) {
				dots [i] = Instantiate (dot) as GameObject;
				dots [i].name = "Dot" + i.ToString ();
				dots [i].transform.position = vertices1 [i];
			}
		}
	}

	// Update is called once per frame
	void Update () {
		//Get vertices of the ball
		ballVertex = transform.position;
		//Do minkowski difference 
		int j = 0;
		while (j<vertices1.Length && !collided) {
		float x = ballVertex.x - vertices1[j].x;
		float y = ballVertex.y - vertices1[j].y;
			if (Mathf.Abs(x)<x_stdev && Mathf.Abs(y)<y_stdev) {
				collided = true;
				if (debugOn) {
					Debug.Log(x+","+y);
					Debug.Log("COLLIDED!!");
					dots[j].renderer.material = redMat;
				}
				gameObject.GetComponent<CannonBall>().Bounce(dampFactor);
			}
			j++;
		}
		//TODO: Also detect for the other slope
	}
}
