using UnityEngine;
using System.Collections;

public class PanelClose : MonoBehaviour {

	public void ClosePanel() {
		gameObject.SetActive (false);
	}
}
