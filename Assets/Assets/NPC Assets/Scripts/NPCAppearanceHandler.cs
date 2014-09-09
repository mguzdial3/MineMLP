﻿using UnityEngine;
using System.Collections;

public class NPCAppearanceHandler : MonoBehaviour {
	public static string SAVE_STRING="Appearance";
	public static string COLOR="Color";


	public Transform headJoint;

	private Quaternion bodyLookTarget;
	private Quaternion headLookTarget;

	private float _headSpeed=2.0f;
	private float _bodySpeed=2.0f;

	//Appearance
	public Renderer bodyRenderer;
	private Color[] _colorOptions = {Color.gray, Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta};
	private int _colorIndex;

	public void Init(){
		headLookTarget = headJoint.rotation;
		bodyLookTarget = transform.rotation;

		//Make it a wee bit different
		_colorIndex = Random.Range (0, _colorOptions.Length);
		bodyRenderer.material.color = _colorOptions [_colorIndex];
	}

	public void InitFromSave(int colorIndex){
		headLookTarget = headJoint.rotation;
		bodyLookTarget = transform.rotation;
		//Make it a wee bit different
		_colorIndex = colorIndex;
		bodyRenderer.material.color = _colorOptions [_colorIndex];
	}

	// Update is called once per frame
	void Update () {
		if (bodyLookTarget != transform.rotation) {
			transform.rotation=Quaternion.Lerp(transform.rotation,bodyLookTarget,Time.deltaTime*_bodySpeed);		
		}

		if ( headLookTarget != headJoint.rotation) {
			headJoint.rotation=Quaternion.Lerp(headJoint.rotation,headLookTarget,Time.deltaTime*_headSpeed);		
		}
	}

	public void SetNewGoal(Vector3 goal){
		Vector3 goalEvenY = goal;
		goalEvenY.y = transform.position.y;

		bodyLookTarget = Quaternion.LookRotation (goalEvenY - transform.position, Vector3.up);

		if (goal.y != transform.position.y) {
			headLookTarget = 	Quaternion.LookRotation (goal - headJoint.position, Vector3.up);	
		}
		else{
			headLookTarget = bodyLookTarget;
		}

		
	}

	public string GetSaveString(){
		string saveString = SAVE_STRING + " ";
		saveString += _colorIndex+" ";
		return saveString;
	}
}