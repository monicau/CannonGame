using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour {

	public bool enableWind;
	public float pos_x;//x component of ball's position
	public float pos_y;//y component of ball's position
	public float prev_pos_x;
	public float prev_pos_y;
	public float velocityMin;
	public float velocityMax;
	public float initialVelocity;
	public float vi_x;//x component of initial velocity
	public float vi_y;//y component of initial velocity
	public float angle;
	public float gravity;
	public float airResistance_x;
	public float airResistance_y;
	public float wind;
	private Wind windComponent;
	private GameObject cannon;

	// Use this for initialization
	void Start () {
		cannon = GameObject.Find ("RightCannon");
		windComponent = GameObject.Find ("Wind").GetComponent<Wind> ();
		wind = windComponent.w;
		prev_pos_x = cannon.transform.position.x;
		prev_pos_y = cannon.transform.position.y;
		pos_x = cannon.transform.position.x - 1f;
		pos_y = cannon.transform.position.y + 1f;
		transform.position = new Vector3 (pos_x, pos_y, 0);
		//Get latest angle of cannon
		angle = cannon.GetComponent<RightCannon> ().angle;
		//Find the x and y components of initial velocity
		initialVelocity = Random.Range (velocityMax, velocityMax + 1);
		vi_x = initialVelocity * Mathf.Cos (Mathf.Deg2Rad*angle);
		vi_y = initialVelocity * Mathf.Sin (Mathf.Deg2Rad*angle);
	}

	public void Bounce(float x, float y) {
		Debug.Log ("old:" + vi_x + "," + vi_y);
		vi_x = x;
		vi_y = y;

	}
	public void Bounce() {
		vi_x = 0;
		vi_y = 0;
	}
	
	// Update is called once per frame
	void Update () {
		prev_pos_x = pos_x;
		prev_pos_y = pos_y;

		//Get latest wind value
		wind = windComponent.w;

		//Apply air resistance to velocity
		if (vi_x < 0) {
			vi_x = vi_x - (vi_x * airResistance_x);
		} else {
			vi_x = vi_x - (vi_x * airResistance_x);
		}
		if (vi_y < 0) {
			vi_y = vi_y - (vi_y * airResistance_y);
		} else {
			vi_y = vi_y - (vi_y * airResistance_y);
		}

		//Apply gravity to velocity with v = v' + at where a=gravity 
		vi_y = vi_y + gravity;

		//Apply wind force and initial velocity to position
		if (!enableWind) {
			wind = 0;
		}
//
		if (pos_x < 0 && vi_x > 0) { 
			pos_x = -1.0f * (Mathf.Abs (pos_x) + vi_x) + (-1.0f * wind);
		} else if (pos_x >= 0 && vi_x > 0) {
			pos_x = pos_x - vi_x + (-1.0f * wind);
		} else if (vi_x < 0) {
			pos_x = pos_x + Mathf.Abs (vi_x) + (-1.0f * wind);
		} else {
			pos_x = 0;
		}
		pos_y = pos_y + vi_y;
		//Update cannon ball position
//		transform.position = new Vector3 (pos_x, pos_y, 0);
		transform.position = Vector3.Lerp (transform.position, new Vector3 (pos_x, pos_y, 0), 0.005f);

		//Destroy if not moving
		if (prev_pos_x == pos_x && prev_pos_y == pos_y) {
			Destroy(gameObject);
		}
	}
}
