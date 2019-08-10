using UnityEngine;
using System.Collections;
public class BackgroundLayer : MonoBehaviour {

	public bool IsLooping, IsParallax,IsAutoScroll,IsLockOnCam;
	public float BackgroundSize; 
	public float ParallaxRatio;
	public Vector3 AutoScrollSpeed;

	private Transform m_camTransform;
	private Transform[] m_pieces;
	private float m_viewZone = 16;
	private int m_leftIndex;
	private int m_rightIndex;

	private Vector3 m_lastCamPos;

	// Use this for initialization
	void Start () {
		m_camTransform = FindObjectOfType<CameraController>().transform;
		m_pieces = new Transform[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			m_pieces [i] = transform.GetChild (i);
		}
		m_viewZone = BackgroundSize * 2f;
		m_leftIndex = 0;
		m_rightIndex = m_pieces.Length - 1;
		m_lastCamPos = new Vector3(transform.position.x,transform.position.y, m_camTransform.position.z);
	}
	
	// The Background Layer MUST update after the camera in order to have stable scrolling.
	// so we use LateUpdate to guarantee this.
	void LateUpdate () {
		if (m_camTransform == null) {
			m_camTransform = FindObjectOfType<CameraController>().transform;
			if (m_camTransform == null)
				return;
		}
		if (IsParallax) {
			float deltaX = m_camTransform.position.x - m_lastCamPos.x;
			float deltaY = m_camTransform.position.y - m_lastCamPos.y;
            float deltaZ = m_camTransform.position.z - m_lastCamPos.z;

            transform.position += new Vector3 (deltaX * ParallaxRatio,deltaY*ParallaxRatio, deltaZ);
		}
		if (IsAutoScroll) {
			transform.position += AutoScrollSpeed * Time.deltaTime;
		}
		if (IsLockOnCam) {
			transform.position = new Vector3 (m_camTransform.position.x, m_camTransform.position.y, transform.position.z);
		}
		m_lastCamPos = new Vector3 (m_camTransform.position.x, m_camTransform.position.y,m_camTransform.position.z);
		if (IsLooping) {
			if (m_camTransform.position.x < (m_pieces [m_leftIndex].transform.position.x + m_viewZone))
				ScrollLeft ();
			if (m_camTransform.position.x > (m_pieces [m_rightIndex].transform.position.x - m_viewZone))
				ScrollRight ();
		}
	}
	private void ScrollLeft() 
	{
		Vector3 oldPos = m_pieces [m_leftIndex].position;
		m_pieces[m_rightIndex].position = new Vector3(oldPos.x - BackgroundSize,oldPos.y,oldPos.z);
		m_leftIndex = m_rightIndex;
		m_rightIndex--;
		if (m_rightIndex < 0)
			m_rightIndex = m_pieces.Length - 1;
	}
	private void ScrollRight()
	{
		Vector3 oldPos = m_pieces [m_rightIndex].position;
		m_pieces[m_leftIndex].position = new Vector3(oldPos.x + BackgroundSize,oldPos.y,oldPos.z);
		m_rightIndex = m_leftIndex;
		m_leftIndex ++;
		if (m_leftIndex == m_pieces.Length)
			m_leftIndex = 0;
	}
}
