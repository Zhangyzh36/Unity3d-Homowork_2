﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour {

	public float selfSpeed; 
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		this.transform.RotateAround(this.transform.position, Vector3.up, selfSpeed * Time.deltaTime);
	}
}
