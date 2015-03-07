using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {

	public float w;
	public float range;
	public GameObject arrow;
	public GameObject head;

	// Use this for initialization
	void Start () {
		w = 0;
		InvokeRepeating ("UpdateWind", 0.5f, 0.5f);
	}
	void UpdateWind() {
		float oldW = w;
		w = Random.Range (-range, range+1);
		if (w < 0 && oldW > 0) {
			arrow.transform.eulerAngles = new Vector3 (0, 0, 180);
			arrow.transform.localScale = new Vector3((w)/range,1,1);
			head.transform.localScale = new Vector3(1,1,1);
//			arrowHead.transform.eulerAngles = new Vector3 (0, 0, 180);
//			arrowBody.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 180);
		} 
		if (w > 0 && oldW < 0) {
			arrow.transform.eulerAngles = new Vector3 (0, 0, 180);
			arrow.transform.localScale = new Vector3((w)/range,1,1);
			head.transform.localScale = new Vector3(1,1,1);
//			arrowHead.transform.eulerAngles = new Vector3 (0, 0, 0);
//			arrowBody.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 0);
		}
//		arrowBody.GetComponent<RectTransform> ().localScale = new Vector3 ((w)/range,1,1);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
