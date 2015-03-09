using UnityEngine;
using System.Collections;

public class LeftCannon : MonoBehaviour {
	public float dogLifeTime;
	public int minAngle;
	public int maxAngle;
	public GameObject dogPrefab;
	public int angle;

	// Use this for initialization
	void Start () {
		gameObject.transform.position = new Vector3 (-8, (10*(2.0f/3.0f))-5, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Tab)) {
			angle = Random.Range (minAngle, maxAngle+1);
			gameObject.transform.eulerAngles = new Vector3(0,0,90-angle);
			GameObject ball = Instantiate (dogPrefab) as GameObject;
			Destroy(ball, dogLifeTime);
		}
	}
}
