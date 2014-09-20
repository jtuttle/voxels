using UnityEngine;
using System.Collections;

public class PropertyNotifier : MonoBehaviour {
	protected void OnValidate() {
        Debug.Log("validate");
	}
}
