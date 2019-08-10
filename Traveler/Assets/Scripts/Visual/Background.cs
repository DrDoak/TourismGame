using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class BackgroundManage : MonoBehaviour {

	public bool scrolling, paralax,autoScroll,lockOnCam;
	public float backgroundSize; 
	public float paralaxSpeed;
	public Vector3 innateSpeed;
	private Transform cameraTransform;
	private Transform[] layers;
	private float viewZone = 16;
	private int leftIndex;
	private int rightIndex;

	private Vector3 m_lastCamera;

	// Use this for initialization
	void Start () {
		cameraTransform = FindObjectOfType<CameraController>().transform;
		layers = new Transform[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			layers [i] = transform.GetChild (i);
		}
		viewZone = backgroundSize * 2f;
		leftIndex = 0;
		rightIndex = layers.Length - 1;
		m_lastCamera = new Vector3(cameraTransform.position.x,cameraTransform.position.y,cameraTransform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if (paralax) {
			float deltaX = cameraTransform.position.x - m_lastCamera.x;
			float deltaY = cameraTransform.position.y - m_lastCamera.y;
            float deltaZ = cameraTransform.position.z - m_lastCamera.z;

			transform.position += new Vector3 (deltaX * paralaxSpeed,deltaY*paralaxSpeed, deltaZ);
		}
		if (autoScroll) {
			transform.position += innateSpeed * Time.deltaTime;
		}
		if (lockOnCam) {
			transform.position = new Vector3 (cameraTransform.position.x, cameraTransform.position.y, transform.position.z);
		}
		m_lastCamera = new Vector3 (cameraTransform.position.x, cameraTransform.position.y,cameraTransform.position.z);
		if (scrolling) {
			if (cameraTransform.position.x < (layers [leftIndex].transform.position.x + viewZone))
				ScrollLeft ();
			if (cameraTransform.position.x > (layers [rightIndex].transform.position.x - viewZone))
				ScrollRight ();
		}
	}
	private void ScrollLeft() 
	{
//		int lastRight = rightIndex;
		Vector3 oldPos = layers [leftIndex].position;
		layers[rightIndex].position = new Vector3(oldPos.x - backgroundSize,oldPos.y,oldPos.z);
		leftIndex = rightIndex;
		//Debug.Log ("Scrolling Left");
		rightIndex--;
		if (rightIndex < 0)
			rightIndex = layers.Length - 1;
	}
	private void ScrollRight()
	{
		//Debug.Log ("Scrolling Right");
//		int lastLeft = leftIndex;
		Vector3 oldPos = layers [rightIndex].position;
		layers[leftIndex].position = new Vector3(oldPos.x + backgroundSize,oldPos.y,oldPos.z);
		rightIndex = leftIndex;
		leftIndex ++;
		if (leftIndex == layers.Length)
			leftIndex = 0;
	}
}
