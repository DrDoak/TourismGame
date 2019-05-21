using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum HealthTextMode {DAMAGE, HEALTH};

public class HealthDisplay : MonoBehaviour {

	public HealthTextMode HealthTextDisplay = HealthTextMode.HEALTH;

	bool m_displayAsFraction;

	public bool DisplayAsFraction { get { return m_displayAsFraction; } set { m_displayAsFraction = value; } }


	TextMeshProUGUI m_number;
	Slider m_s;
	const float DISPLAY_TIME = 2.0f;
	const float NUM_DISPLAY_TIME = 1.0f;

	float m_timeDisplayed = 0.0f;
	float m_alpha = 1.0f;
	float m_textAlpha = 1.0f;
	bool num_displayed = false;
	bool bar_displayed = false;

	float m_maxHealth = 100f;
	float m_cumulativeDamage = 1;
	Image m_background;
	Image m_fill;

	public string ValueLabel { get { return m_valueLabel; } set { m_valueLabel = value; } }

	string m_valueLabel = "HP";
	string deltaLabel = "DMG";

	// Use this for initialization
	void Awake () {
		m_number = transform.GetChild (0).GetChild(2).GetComponent<TextMeshProUGUI> ();
		m_s = transform.GetChild (0).GetComponent<Slider> ();
		m_background = transform.GetChild (0).GetChild (0).GetComponent<Image>();
		m_fill = transform.GetChild (0).GetChild (1).GetChild (0).GetComponent<Image>();
		SetAlpha (0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (bar_displayed) {
			m_timeDisplayed += Time.deltaTime;
			if (m_timeDisplayed > DISPLAY_TIME) {
				if (m_alpha > 0) {
					m_alpha -= Time.deltaTime;
					SetAlpha (m_alpha);
				} else {
					OnDisappear ();
				}
			}
			if (num_displayed) {
				if (m_timeDisplayed > NUM_DISPLAY_TIME) {
					if (m_textAlpha > 0) {
						m_textAlpha -= Time.deltaTime;
						Color c = m_number.faceColor;
						m_number.faceColor = new Color (c.r,c.g,c.b, m_textAlpha);
					} else {
						num_displayed = false;
						m_textAlpha = 0.0f;
						m_cumulativeDamage = 0;
					}
				}
			}					
		}
	}

	private void SetAlpha(float a) {
		m_background.color = new Color (0f, 0f, 0f, a);
		m_fill.color = new Color (1f, 1f, 1f, a);
	}
	private void OnDisappear() {
		m_cumulativeDamage = 0;
		bar_displayed = false;
		m_alpha = 1.0f;
	}
	public void ChangeValue(float diff, float currentHealth) {
		m_s.value = (currentHealth / m_maxHealth);
		if (diff >= 0f && diff < 1f)
			return;
		if (Mathf.Sign (m_cumulativeDamage) != Mathf.Sign (diff)) {
			m_cumulativeDamage = 0f;
			m_cumulativeDamage += diff;
			if (Mathf.Abs (m_cumulativeDamage) < 1f)
				return;
			if (diff > 0f) {
				m_number.faceColor = Color.green;
			} else {
				m_number.faceColor = Color.red;
			}
		} 
		m_cumulativeDamage += diff;
		if (Mathf.Abs (m_cumulativeDamage) < 1f)
			return;
		if (HealthTextDisplay == HealthTextMode.HEALTH) {
			if (m_displayAsFraction) {
				string s = Mathf.RoundToInt (currentHealth).ToString () + " \\ " + Mathf.RoundToInt (m_maxHealth).ToString () + " " + m_valueLabel;
				m_number.SetText (s);
			} else {
				m_number.SetText (Mathf.RoundToInt (currentHealth).ToString () + " " + m_valueLabel);
			}
		} else {
			m_number.SetText (Mathf.Abs (Mathf.RoundToInt (m_cumulativeDamage)).ToString () + " " + deltaLabel);
		}
		m_timeDisplayed = 0.0f;
		bar_displayed = true;
		num_displayed = true;
		m_alpha = 1.0f;
		m_textAlpha = 1.0f;
		Color c = m_number.faceColor;
		m_number.faceColor = new Color (c.r,c.g,c.b, m_textAlpha);
		SetAlpha (1.0f);
	}
	public void SetMaxHealth(float maxHealth) {
		m_maxHealth = maxHealth;
		m_s.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (maxHealth / 100) * 32);
	}
}
