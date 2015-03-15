using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetection : MonoBehaviour {

	public float cRestitution;
	public bool debugOn;
	public float x_stdev;
	public float y_stdev;
	public Material redMat;
	public GameObject dot;
	private GameObject slope1;
	private Vector3[] vertices1;
	private bool collided;
	private GameObject[] dots;
	private CannonBall cannonball;

	// Use this for initialization
	void Start () {
		cannonball = gameObject.GetComponent<CannonBall> ();
		collided = false;
		slope1 = GameObject.Find ("LeftSlope");
		vertices1 = slope1.GetComponent<MergeMeshes>().vertices;
		if (debugOn) {
			//If debug on, draw dots at the vertices of cliff
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

		//For some reason, the code below doesn't work. The ball doesn't bounce back even when the velocity turns out to be reflected.  Bug may be in CannonBall Update method.
//			for (int i=0; i<vertices1.Length-1; i++) {
//				foreach (Vector3 ballPoint in ballPoints) {
//					if (ballPoint.y > vertices1 [i].y && ballPoint.y < vertices1 [i + 1].y && isLeft (vertices1 [i], vertices1 [i + 1], ballPoint) && !isLeft (Add (vertices1 [i], new Vector3 (-1, 0, 0)), Add (vertices1 [i + 1], new Vector3 (-1, 0, 0)), ballPoint)) {
//						float v_x = gameObject.GetComponent<CannonBall> ().vi_x;
//						float v_y = gameObject.GetComponent<CannonBall> ().vi_y;
//						Vector3 v_in = new Vector3 (v_x, v_y, 0);
//						Vector3 intersection = LineIntersectionPoint (vertices1 [i], vertices1 [i + 1], Vector3.zero, new Vector3 (v_x, v_y, 0));
//						//Find the normal
//						Vector3 a = Subtract (vertices1 [i + 1], vertices1 [i]);
//						Vector3 b = Subtract (new Vector3 (vertices1 [i].x, vertices1 [i].y, 1), vertices1 [i]);
//						Vector3 normal = Vector3.Cross (a,b).normalized;
//						//Find reflecting velocity: v_out = v_in + (-(1+cRestitution)*v_in_normal) * normal
//						//where v_in_normal = v_in dot normal
//						Vector3 subresult = (-(1+cRestitution) * Vector3.Dot (v_in, normal))*normal;
//						Vector3 v_out = Add(v_in, subresult);
//						cannonball.Bounce (v_out.x, v_out.y);
//						break;
//					}
//				}
//			}

//		Do minkowski difference instead and default no bounce
		int j = 0;
		while (j<vertices1.Length && !collided) {
			for (int i=0; i<ballPoints.Length; i++) {
				float x = ballPoints[i].x - vertices1[j].x;
				float y = ballPoints[i].y - vertices1[j].y;
				if (Mathf.Abs(x)<x_stdev && Mathf.Abs(y)<y_stdev) {
					collided = true;
					if (debugOn) {
						//Print message and change point of collision to red
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

	private Vector3 Add(Vector3 a, Vector3 b) {
		Vector3 result = new Vector3 (a.x + b.x, a.y + b.y, a.z + b.z);
		return result;
	}
	private Vector3 Subtract(Vector3 a, Vector3 b) {
		Vector3 result = new Vector3 (a.x - b.x, a.y - b.y, a.z - b.z);
		return result;
	}
	private Vector3 LineIntersectionPoint(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2) {
		// Get A,B,C of first line - points : pointA1 to pointA2
		float A1 = pointA2.y-pointA1.y;
		float B1 = pointA1.x-pointA2.x;
		float C1 = A1*pointA1.x+B1*pointA1.y;
		
		// Get A,B,C of second line - points : pointB1 to pointB2
		float A2 = pointB2.y-pointB1.y;
		float B2 = pointB1.x-pointB2.x;
		float C2 = A2*pointB1.x+B2*pointB1.y;
		
		// Get delta and check if the lines are parallel
		float delta = A1*B2 - A2*B1;
		if (delta == 0)	return Vector3.zero;//lines don't intersect
		
		// Return the Vector3 intersection point
		return new Vector3((B2*C1 - B1*C2)/delta, (A1*C2 - A2*C1)/delta);
	}
}
