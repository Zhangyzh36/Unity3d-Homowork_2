using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeOfEarth : MonoBehaviour {

	public Transform trans;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = trans.position;	
	}
}
