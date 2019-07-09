using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UIBarDisplayMode {FRACTION, PERCENT, BASE, NONE};

public class UIBarInfo {
	public delegate void UIBarUpdateFunction(UIBarInfo uib);
	public string UILabel;
	public bool DisplayLabel = true;
	public UIBarDisplayMode DisplayMode = UIBarDisplayMode.FRACTION;
	public string id = "SameAsLabel";
	public Color FillColor = Color.red;
	public bool useScale = true;
	public float scale = 1.0f;
	public GameObject element;
	public GameObject target;
	public UIBarUpdateFunction funcUpdate;
	public UIBar uib;
}

public class UIBar : MonoBehaviour {

	private Slider m_slider;
	private TextMeshProUGUI m_text;
	private Image m_fill_image;

	private string m_description;
	private UIBarDisplayMode m_displayMode;

	private float m_scale = 1.0f;
	private bool m_use_scale = true;
	public int Num = 0;
	private const float BarHeight = 25;
	private const float StartingHeight = -100;
	private const float StartingX = 100;
	// Use this for initialization
	void Start () {
		m_slider = transform.Find ("Health_Bar").GetComponent<Slider> ();
		m_text = transform.Find ("UI_Label").GetComponent<TextMeshProUGUI> ();
		m_fill_image = transform.Find ("Health_Bar").Find ("Fill_Area").Find ("Fill").GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Initialize(UIBarInfo ubi, int n) {
		SetColor (ubi.FillColor);
		if (ubi.DisplayLabel)
			SetLabel (ubi.UILabel);
		else
			SetLabel ("");
		SetUseScale (ubi.useScale);
		SetScale (ubi.scale);
		SetDisplayMode (ubi.DisplayMode);
		Vector2 v = new Vector2 (StartingX, BarHeight * n + StartingHeight);
		GetComponent<RectTransform> ().anchoredPosition = v;
		Num = n;
	}
	public void OnRemove(int n) {
		if (n < Num)
			Num -= 1;
		Vector2 v = new Vector2 (StartingX, BarHeight * Num + StartingHeight);
		GetComponent<RectTransform> ().anchoredPosition = v;
	}
	public void SetUseScale(bool us) {
		m_use_scale = us;
	}
	public void SetScale(float s) {
		m_scale = s;
	}
	public void SetColor(Color c) {
		m_fill_image = transform.Find ("Health_Bar").Find ("Fill_Area").Find ("Fill").GetComponent<Image> ();
		m_fill_image.color = c;
	}

	public void SetLabel(string label) {
		m_description = label;
	}
	public void SetDisplayMode(UIBarDisplayMode display) {
		m_displayMode = display;
		if (m_displayMode == UIBarDisplayMode.NONE) {
			m_text.text = m_description;
		}
	}

	public void UpdateValues(float value, float maxValue) {
		m_slider.value = value;
		m_slider.maxValue = maxValue;
		Vector3 oS = m_slider.GetComponent<RectTransform> ().localScale;
		if (m_use_scale) {
			m_slider.GetComponent<RectTransform> ().localScale = new Vector3 (m_scale * (maxValue / 100f), oS.y, oS.z);
		} else {
			m_slider.GetComponent<RectTransform> ().localScale = new Vector3(1f,oS.y,oS.z);
		}

		if (m_displayMode == UIBarDisplayMode.FRACTION) {
			m_text.text = m_description + ": " + value.ToString().PadRight(5) + " / " + maxValue.ToString().PadRight(5);
		} else if (m_displayMode == UIBarDisplayMode.BASE) {
			m_text.text = m_description + ": " + value.ToString().PadRight(5);
		} else if (m_displayMode == UIBarDisplayMode.PERCENT) {
			float perc = Mathf.Round(100f * (value / maxValue));
			m_text.text = m_description + ": " + perc.ToString().PadLeft(4) + " %";
		}
	}
}