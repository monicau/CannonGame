using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetection : MonoBehaviour {

	public bool debugOn;
	public float x_stdev;
	public float y_stdev;
	public Material redMat;
	public GameObject dot;
	private GameObject slope1;
	private Vector3[] vertices1;
	private float r;
	private Vector3 ballVertex;
	private bool collided;
	private GameObject[] dots;
	private CannonBall cannonball;

	// Use this for initialization
	void Start () {
		cannonball = gameObject.GetComponent<CannonBall> ();
		collided = false;
		slope1 = GameObject.Find ("LeftSlope");
		vertices1 = slope1.GetComponent<MergeMeshes>().vertices;
		r = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
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
		Vector3[] ballPoints = new Vector3[8];
		float radius = gameObject.renderer.bounds.extents.x;
		ballPoints[0] = new Vector3(transform.position.x, transform.position.y + radius, 0); 
		ballPoints[1] = new Vector3(transform.position.x-(radius*3/4), transform.position.y+(radius*3/4), 0); 
		ballPoints[2] = new Vector3(transform.position.x - radius, transform.position.y, 0); 
		ballPoints[3] = new Vector3(transform.position.x-(radius*3/4), transform.position.y-(radius*3/4), 0); 
		ballPoints[4] = new Vector3(transform.position.x, transform.position.y - radius, 0); 
		ballPoints[5] = new Vector3(transform.position.x+(radius*3/4), transform.position.y-(radius*3/4), 0); 
		ballPoints[6] = new Vector3(transform.position.x+radius, transform.position.y, 0); 
		ballPoints[7] = new Vector3(transform.position.x+(radius *3/4), transform.position.y+(radius*3/4), 0); 
//		Do minkowski difference 
		int j = 0;
		while (j<vertices1.Length && !collided) {
			for (int i=0; i<ballPoints.Length; i++) {
				float x = ballPoints[i].x - vertices1[j].x;
				float y = ballPoints[i].y - vertices1[j].y;
				if (Mathf.Abs(x)<x_stdev && Mathf.Abs(y)<y_stdev) {
					collided = true;
					if (debugOn) {
						Debug.Log(x+","+y);
						Debug.Log("COLLIDED!!");
						dots[j].renderer.material = redMat;
					}
					cannonball.Bounce();
				}
			}
			j++;
		}
	}

	private bool isLeft(Vector3 linePointA, Vector3 linePointB, Vector3 c) {
		return ((linePointB.x - linePointA.x)*(c.y - linePointA.y) - (linePointB.y - linePointA.y)*(c.x - linePointA.x)) > 0;
	}
}
