using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dog : MonoBehaviour {
	
	public float angle;
	public float minVelocity;
	public float maxVelocity;
	public GameObject dot;
	public float gravity;
	public float timeStep;
	public float airResistance_x;
	public float airResistance_y;
	public Material dogMaterial;
	float vi_x;
	float vi_y;
	Vector3[] currentPos;
	Vector3[] previousPos;
	Vector3[] forceAccumulators;
	List<GameObject> dots;
	List<Vector3> dogCoordinates;
	Wind wind;
	List<LineRenderer> lines;
	List<GameObject> lineObjects;//objects that each have a LineRenderer
	GameObject eye;
	GameObject cannon;

	// Use this for initialization
	void Start () {
		//Get latest angle of cannon
		cannon = GameObject.Find ("LeftCannon");
		angle = cannon.GetComponent<LeftCannon> ().angle;
		//Find the x and y components of initial velocity
		float initialVelocity = Random.Range (minVelocity, maxVelocity);
		vi_x = initialVelocity * Mathf.Cos (Mathf.Deg2Rad*angle);
		vi_y = initialVelocity * Mathf.Sin (Mathf.Deg2Rad*angle);

		wind = GameObject.Find ("Wind").GetComponent<Wind> ();
		lines = new List<LineRenderer> ();
		lineObjects = new List<GameObject> ();

		for (int i=0; i<17; i++) {
			GameObject o = new GameObject ();
			o.transform.parent = gameObject.transform;
			LineRenderer lineR = o.AddComponent<LineRenderer> ();
			lineR.material = dogMaterial;
			lineR.SetWidth (0.1f, 0.1f);
			lineR.SetVertexCount (2);
			lines.Add (lineR);
			lineObjects.Add (o);
		}

		float cannonX = 8;
		float cannonY = (10 * (2.0f / 3.0f)) - 5;

		//Set the vertex locations of the dog
		dogCoordinates = new List<Vector3> ();
		//Body:
		dogCoordinates.Add (new Vector3 (0 - cannonX, 0 + cannonY, 0));
		dogCoordinates.Add (new Vector3 (1 - cannonX, 0 + cannonY, 0));
		dogCoordinates.Add (new Vector3(1-cannonX,0.5f+cannonY,0));
		dogCoordinates.Add (new Vector3(0-cannonX,0.5f+cannonY,0));
		//Neck:
		dogCoordinates.Add (new Vector3 (-0.2f - cannonX, 0.7f + cannonY, 0));
		//Eye
		dogCoordinates.Add (new Vector3 (-0.3f - cannonX, 0.85f + cannonY, 0));
		//Head:
		dogCoordinates.Add (new Vector3 (-0.2f - cannonX, 1.0f + cannonY, 0));
		dogCoordinates.Add (new Vector3 (-0.6f - cannonX, 1.0f + cannonY, 0));
		dogCoordinates.Add (new Vector3 (-0.6f - cannonX, 0.7f + cannonY, 0));
		//Tail:
		dogCoordinates.Add (new Vector3 (1.3f - cannonX, 0.8f + cannonY, 0));
		//Front leg:
		dogCoordinates.Add (new Vector3 (0.25f - cannonX, 0 + cannonY, 0));
		dogCoordinates.Add (new Vector3 (0.25f - cannonX, -0.3f + cannonY, 0));
		dogCoordinates.Add (new Vector3 (0.25f - cannonX, -0.6f + cannonY, 0));
		dogCoordinates.Add (new Vector3 (0.05f - cannonX, -0.6f + cannonY, 0));
		//Rear leg:
		dogCoordinates.Add (new Vector3 (0.75f - cannonX, 0 + cannonY, 0));
		dogCoordinates.Add (new Vector3 (0.75f - cannonX, -0.3f + cannonY, 0));
		dogCoordinates.Add (new Vector3 (0.75f - cannonX, -0.6f + cannonY, 0));
		dogCoordinates.Add (new Vector3 (0.55f - cannonX, -0.6f + cannonY, 0));

		currentPos = new Vector3[dogCoordinates.Count];
		previousPos = new Vector3[dogCoordinates.Count];
		forceAccumulators = new Vector3[dogCoordinates.Count];

		for (int i=0; i<dogCoordinates.Count; i++) {
			currentPos[i] = dogCoordinates[i];
			previousPos[i] = Subtract(dogCoordinates[i], new Vector3(vi_x, vi_y, 0));
			forceAccumulators[i] = new Vector3(-1*wind.w + (-1*airResistance_x), airResistance_y + gravity, 0);
		}
	}
	private void Verlet() {
		for (int i=0; i< currentPos.Length; i++) {
			Vector3 currPos = currentPos[i];
			//Verlet equation:
			//nextPosition = currPos + (currPos-prevPos)*(thisTimeStep/lastTimeStep) + forceAccumlators*thisTimeStep*thisTimeStep
			//Let's break it up to: nextPosition = a + b  <-- assuming constant timesteps
			Vector3 a = new Vector3(currentPos[i].x + currentPos[i].x - previousPos[i].x, currentPos[i].y + currentPos[i].y - previousPos[i].y, 0);
			Vector3 b = forceAccumulators[i] * timeStep * timeStep;
			currentPos[i] = new Vector3(a.x+b.x, a.y+b.y, 0);
			previousPos[i] = currPos;
		}
	}
	//Constraint source: http://www.pagines.ma1.upc.edu/~susin/files/AdvancedCharacterPhysics.pdf
	private void SatisfyContraints() {
		//Body constraints: distance(v0, v1)==1, distance(v2,v3)==1, dist(v1,v2)==0.5, dist(v0,v3)==0.5, dist(v1,v3)==1.118, dist(v0,v2)==1.118
		//distance(v0, v1)==1
		Vector3 delta = Subtract (currentPos [1], currentPos [0]);
		float deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		float diff = (deltaLength-1) / deltaLength;
		currentPos [0] = Add (currentPos [0], delta * 0.5f * diff);
		currentPos [1] = Subtract (currentPos [1], delta * 0.5f * diff);
		//distance(v2,v3)==1
		delta = Subtract (currentPos [3], currentPos [2]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-1) / deltaLength;
		currentPos [2] = Add (currentPos [2], delta * 0.5f * diff);
		currentPos [3] = Subtract (currentPos [3], delta * 0.5f * diff);
		//dist(v1,v2)==0.5
		delta = Subtract (currentPos [2], currentPos [1]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.5f) / deltaLength;
		currentPos [1] = Add (currentPos [1], delta * 0.5f * diff);
		currentPos [2] = Subtract (currentPos [2], delta * 0.5f * diff);
		//dist(v0,v3)==0.5
		delta = Subtract (currentPos [3], currentPos [0]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.5f) / deltaLength;
		currentPos [0] = Add (currentPos [0], delta * 0.5f * diff);
		currentPos [3] = Subtract (currentPos [3], delta * 0.5f * diff);
		//dist(v1,v3)==1.118  Real distance is sqrt(0.5^2 + 1^2) but we want to be cheap
		delta = Subtract (currentPos [3], currentPos [1]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-1.118f) / deltaLength;
		currentPos [1] = Add (currentPos [1], delta * 0.5f * diff);
		currentPos [3] = Subtract (currentPos [3], delta * 0.5f * diff);
		//dist(v0,v2)==1.118
		delta = Subtract (currentPos [2], currentPos [0]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-1.118f) / deltaLength;
		currentPos [0] = Add (currentPos [0], delta * 0.5f * diff);
		currentPos [2] = Subtract (currentPos [2], delta * 0.5f * diff);
		//Neck constraint
		delta = Subtract (currentPos [4], currentPos [3]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.28f) / deltaLength;//Real distance is sqrt(0.2^2 + 0.2^2) 
		currentPos [3] = Add (currentPos [3], delta * 0.5f * diff);
		currentPos [4] = Subtract (currentPos [4], delta * 0.5f * diff);
		//Head constraint
		delta = Subtract (currentPos [6], currentPos [4]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.3f) / deltaLength;
		currentPos [4] = Add (currentPos [4], delta * 0.5f * diff);
		currentPos [6] = Subtract (currentPos [6], delta * 0.5f * diff);
		delta = Subtract (currentPos [7], currentPos [6]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.4f) / deltaLength;
		currentPos [6] = Add (currentPos [6], delta * 0.5f * diff);
		currentPos [7] = Subtract (currentPos [7], delta * 0.5f * diff);
		delta = Subtract (currentPos [8], currentPos [7]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.3f) / deltaLength;
		currentPos [7] = Add (currentPos [7], delta * 0.5f * diff);
		currentPos [8] = Subtract (currentPos [8], delta * 0.5f * diff);
		delta = Subtract (currentPos [8], currentPos [4]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.4f) / deltaLength;
		currentPos [4] = Add (currentPos [4], delta * 0.5f * diff);
		currentPos [8] = Subtract (currentPos [8], delta * 0.5f * diff);
		delta = Subtract (currentPos [7], currentPos [4]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.5f) / deltaLength;
		currentPos [4] = Add (currentPos [4], delta * 0.5f * diff);
		currentPos [7] = Subtract (currentPos [7], delta * 0.5f * diff);
		//Tail constraint
		delta = Subtract (currentPos [9], currentPos [2]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.42f) / deltaLength;//Real distance is sqrt(0.3^2 + 0.3^2)
		currentPos [2] = Add (currentPos [2], delta * 0.5f * diff);
		currentPos [9] = Subtract (currentPos [9], delta * 0.5f * diff);
		//Front leg constraint
		delta = Subtract (currentPos [11], currentPos [10]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.3f) / deltaLength;
		currentPos [10] = Add (currentPos [10], delta * 0.5f * diff);
		currentPos [11] = Subtract (currentPos [11], delta * 0.5f * diff);
		delta = Subtract (currentPos [12], currentPos [11]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.3f) / deltaLength;
		currentPos [11] = Add (currentPos [11], delta * 0.5f * diff);
		currentPos [12] = Subtract (currentPos [12], delta * 0.5f * diff);
		delta = Subtract (currentPos [13], currentPos [12]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.2f) / deltaLength;
		currentPos [12] = Add (currentPos [12], delta * 0.5f * diff);
		currentPos [13] = Subtract (currentPos [13], delta * 0.5f * diff);
		//Rear leg constraint
		delta = Subtract (currentPos [15], currentPos [14]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.3f) / deltaLength;
		currentPos [14] = Add (currentPos [14], delta * 0.5f * diff);
		currentPos [15] = Subtract (currentPos [15], delta * 0.5f * diff);
		delta = Subtract (currentPos [16], currentPos [15]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.3f) / deltaLength;
		currentPos [15] = Add (currentPos [15], delta * 0.5f * diff);
		currentPos [16] = Subtract (currentPos [16], delta * 0.5f * diff);
		delta = Subtract (currentPos [17], currentPos [16]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.2f) / deltaLength;
		currentPos [16] = Add (currentPos [16], delta * 0.5f * diff);
		currentPos [17] = Subtract (currentPos [17], delta * 0.5f * diff);
		//Eye constraint
		delta = Subtract (currentPos [6], currentPos [5]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.18f) / deltaLength;//Real distance sqrt(0.1^2 + 0.15^2)
		currentPos [5] = Add (currentPos [5], delta * 0.5f * diff);
		currentPos [6] = Subtract (currentPos [6], delta * 0.5f * diff);
		delta = Subtract (currentPos [8], currentPos [5]);
		deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta));
		diff = (deltaLength-0.335f) / deltaLength;//Real distance sqrt(0.3^2 + 0.15^2)
		currentPos [5] = Add (currentPos [5], delta * 0.5f * diff);
		currentPos [8] = Subtract (currentPos [8], delta * 0.5f * diff);
	}
	private void AccumulateForces() {
		for (int i=0; i<forceAccumulators.Length; i++) {
			forceAccumulators[i] = new Vector3(-1*wind.w + (-1*airResistance_x), airResistance_y + gravity, 0);
		}
	}
	public void TimeStep() {
		AccumulateForces ();
		Verlet ();
		SatisfyContraints ();
	}
	// Update is called once per frame
	void Update () {
		timeStep = Time.deltaTime;
		TimeStep ();
	
		//Draw body
		for (int i=0; i<3; i++) {
			lines[i].SetPosition(0, currentPos[i]);
			lines[i].SetPosition(1, currentPos[i+1]);
		}
		//Close up the rectangle of the body
		lines[3].SetPosition(0, currentPos[3]);
		lines[3].SetPosition(1, currentPos[0]);
		//Draw neck
		lines [4].SetPosition (0, currentPos [3]);
		lines [4].SetPosition (1, currentPos [4]);
		//Draw head
		lines[5].SetPosition(0, currentPos[4]);
		lines[5].SetPosition(1, currentPos[6]);
		lines[6].SetPosition(0, currentPos[6]);
		lines[6].SetPosition(1, currentPos[7]);
		lines[7].SetPosition(0, currentPos[7]);
		lines[7].SetPosition(1, currentPos[8]);
		lines[8].SetPosition(0, currentPos[8]);
		lines[8].SetPosition(1, currentPos[4]);
		//Draw tail
		lines[9].SetPosition(0, currentPos[2]);
		lines[9].SetPosition(1, currentPos[9]);
		//Draw front leg
		lines[10].SetPosition(0, currentPos[10]);
		lines[10].SetPosition(1, currentPos[11]);
		lines[11].SetPosition(0, currentPos[11]);
		lines[11].SetPosition(1, currentPos[12]);
		lines[12].SetPosition(0, currentPos[12]);
		lines[12].SetPosition(1, currentPos[13]);
		//Draw rear leg
		lines[13].SetPosition(0, currentPos[14]);
		lines[13].SetPosition(1, currentPos[15]);
		lines[14].SetPosition(0, currentPos[15]);
		lines[14].SetPosition(1, currentPos[16]);
		lines[15].SetPosition(0, currentPos[16]);
		lines[15].SetPosition(1, currentPos[17]);
		//Draw eye 
		lines [16].SetPosition (0, currentPos [5]);
		lines [16].SetPosition (1, Add(currentPos [5], new Vector3(-0.05f,0,0)));
	}
	Vector3 Add(Vector3 a, Vector3 b) {
		Vector3 result = new Vector3 (a.x + b.x, a.y + b.y, a.z + b.z);
		return result;
	}
	Vector3 Subtract(Vector3 a, Vector3 b) {
			Vector3 result = new Vector3 (a.x - b.x, a.y - b.y, a.z - b.z);
			return result;
	}
		                                                                        
}
