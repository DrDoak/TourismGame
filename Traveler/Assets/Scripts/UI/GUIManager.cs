using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public static GUIManager Instance = null;

    public GameObject MenuProperty;
    public GameObject SelectionProperty;
    public GameObject ActionTextPrefab;

    //public Slider HealthBarPrefab;
    public GameObject UIBarPrefab;
    public GameObject CurrentTarget;
    public TextMeshProUGUI ExpText;

    private List<GameObject> PropertyLists;
    private Dictionary<string, UIBarInfo> uibars = new Dictionary<string, UIBarInfo>();

    GameObject m_actionTextHolder;
    private List<UIActionText> m_actionTextList;
    Dictionary<string, GameObject> m_iconList;

    void Awake()
    {
        Instance = this;
        PropertyLists = new List<GameObject>();
        m_iconList = new Dictionary<string, GameObject>();
        m_actionTextList = new List<UIActionText>();
    }
    void Update()
    {
        foreach (UIBarInfo u in uibars.Values)
        {
            if (u.target != null)
                u.funcUpdate(u);
        }
        List<UIActionText> newL = new List<UIActionText>();
        foreach (UIActionText uat in m_actionTextList)
        {
            float life = Time.timeSinceLevelLoad - uat.timeCreated;
            if (life > uat.timeToDisplay)
            {
                Destroy(uat.element);
            }
            else
            {
                int pads = Mathf.RoundToInt(Mathf.Max(0f, 40f - (40f * life / 0.2f)));
                uat.element.GetComponent<TextMeshProUGUI>().text = uat.text.PadLeft(pads);
                Color c = uat.element.GetComponent<TextMeshProUGUI>().color;
                float remaininglife = (uat.timeCreated + uat.timeToDisplay) - Time.timeSinceLevelLoad;
                if (remaininglife < 0.2f)
                {
                    uat.element.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, Mathf.Max(0f, 1f * remaininglife / 0.2f));
                }
                else
                {
                    uat.element.GetComponent<TextMeshProUGUI>().color = new Color(c.r, c.g, c.b, Mathf.Min(1f, (1f * life / 0.2f)));
                }
                newL.Add(uat);
            }
        }
        m_actionTextList = newL;
    }

    public void AddUIBar(UIBarInfo uib)
    {
        if (uib.id == "SameAsLabel")
            uib.id = uib.UILabel;
        RemoveUIBar(uib.id);
        GameObject newBar = Instantiate(UIBarPrefab, transform);
        uib.element = newBar;
        UIBar uibar = uib.element.GetComponent<UIBar>();
        uibar.Initialize(uib, uibars.Count);
        uib.uib = uibar;
        uibars.Add(uib.id, uib);
    }

    public void RemoveUIBar(string id)
    {
        if (uibars.ContainsKey(id))
        {
            int NumRemoved = uibars[id].uib.Num;
            Destroy(uibars[id].element);
            uibars.Remove(id);
            foreach (UIBarInfo uibi in uibars.Values)
            {
                uibi.uib.OnRemove(NumRemoved);
            }
        }
    }

    public void ClearAllUIBars()
    {
        int i = 0;
        foreach (UIBarInfo uibi in uibars.Values)
        {
            uibi.uib.OnRemove(i);
            i++;
        }
    }
    public void AddText(UIActionText uai)
    {
        uai.timeCreated = Time.timeSinceLevelLoad;
        GameObject go = Instantiate(ActionTextPrefab, transform.Find("ActionText"));
        go.GetComponent<TextMeshProUGUI>().text = uai.text.PadLeft(40);
        go.GetComponent<TextMeshProUGUI>().color = new Color(uai.textColor.r, uai.textColor.r, uai.textColor.b, 0f);

        uai.element = go;
        m_actionTextList.Add(uai);
    }

    public void ReplaceText(UIActionText uai)
    {
        List<UIActionText> newL = new List<UIActionText>();
        foreach (UIActionText uat in m_actionTextList)
        {
            if (uai.id == uat.id)
                Destroy(uat.element);
            else
                newL.Add(uat);
        }
        m_actionTextList = newL;
        AddText(uai);
    }

    public void RemoveText(UIActionText uai)
    {
        List<UIActionText> newL = new List<UIActionText>();
        foreach (UIActionText uat in m_actionTextList)
        {
            if (uai.id == uat.id)
                Destroy(uat.element);
            else
                newL.Add(uat);
        }
        m_actionTextList = newL;
    }

    public void SetHUD(bool active)
    {
        if (!active)
        {
            //HealthBarPrefab.gameObject.SetActive( false);
            ExpText.gameObject.SetActive(false);
            transform.GetChild(0).Find("PropList").gameObject.SetActive(false);
        }
        else
        {
            //HealthBarPrefab.gameObject.SetActive( true);
            ExpText.gameObject.SetActive(true);
            transform.GetChild(0).Find("PropList").gameObject.SetActive(true);
        }
    }

    public static void ClosePropertyLists()
    {
        foreach (GameObject go in Instance.PropertyLists)
        {
            Destroy(go);
        }
        Instance.PropertyLists.Clear();
    }


    public void AddPropIcon(Property p)
    {
        /*if (!m_iconList.ContainsKey(p.GetType().ToString()) ){
			System.Type sysType = p.GetType ();
			Property mp = (Property)GetComponentInChildren (sysType);
			GameObject go = Instantiate (IconPropertyPrefab);
			go.transform.SetParent(transform.GetChild(0).Find ("PropList"),false);
			if (mp != null) {
				go.GetComponent<Image> ().sprite = mp.icon;
			} else {
				//go.GetComponent<Image> ().sprite = mp.icon;
			}
			m_iconList [p.GetType().ToString()] = go;
		}*/
    }

    public void ClearPropIcons()
    {
        foreach (GameObject g in m_iconList.Values)
        {
            Destroy(g);
        }
        m_iconList.Clear();
    }
    public void RemovePropIcon(Property p)
    {
        if (m_iconList.ContainsKey(p.GetType().ToString()))
        {
            Destroy(m_iconList[p.GetType().ToString()]);
            m_iconList.Remove(p.GetType().ToString());
        }
    }

/*
    public void OnSetPlayer(BasicMovement bm)
    {
        ClearPropIcons();
        CurrentTarget = bm.gameObject;
        foreach (Property p in bm.GetComponents<Property>()) {
			AddPropIcon (p);
		}
    }
*/
}