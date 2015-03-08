using UnityEngine;
using System.Collections;

public class RightCannon : MonoBehaviour {

	public float ballLifetime;
	public int minAngle;
	public int maxAngle;
	public int angle;
	public GameObject cannonBallPrefab;

	// Use this for initialization
	void Start () {
		gameObject.transform.position = new Vector3 (8, (10*(2.0f/3.0f))-5, 0);
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			angle = Random.Range (minAngle, maxAngle+1);
			gameObject.transform.eulerAngles = new Vector3(0,0,90-angle);
			GameObject ball = Instantiate (cannonBallPrefab) as GameObject;
			Destroy(ball, ballLifetime);
		}
	}
}
