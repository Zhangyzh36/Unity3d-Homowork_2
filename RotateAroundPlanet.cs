using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPlanet : MonoBehaviour {

	public Transform origin;
	public float speed;
	float axisX, axisY;
	// Use this for initialization
	void Start () {
		axisX = Random.Range (-3,3);
		axisY = Random.Range (15, 20);
	}

	// Update is called once per frame
	void Update () {
		Vector3 axis = new Vector3(axisX, axisY, 0);  
		this.transform.RotateAround(origin.position, axis, speed * Time.deltaTime);  
	}
}
