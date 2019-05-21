using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public Dictionary<string, Zone> m_registeredZones;
    private static ZoneManager m_instance;

    public static ZoneManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            m_registeredZones = new Dictionary<string, Zone>();
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
    }

    public static bool IsPointInZone(Vector3 point, string zone)
    {
        /*Debug.Log("Querying Zone: " + zone + " has zone?: " +
            m_instance.m_registeredZones.ContainsKey(zone));*/

        if (m_instance.m_registeredZones.ContainsKey(zone))
        {
            return m_instance.m_registeredZones[zone].IsInZone(point);
        }
        return false;
    }
    public static bool IsHaveObject(AICharacter aic, string zone)
    {

        if (m_instance.m_registeredZones.ContainsKey(zone))
        {
            return m_instance.m_registeredZones[zone].IsHaveObject(aic);
        }
        return false;
    }
    public static Zone GetZone(string id)
    {
        if (m_instance.m_registeredZones.ContainsKey(id))
        {
            return m_instance.m_registeredZones[id];
        }
        return null;
    }

    public static void RegisterZone(Zone z)
    {
        m_instance.m_registeredZones[z.Label] = z;
    }
    public static void DeRegisterZone(Zone z)
    {
        if (m_instance.m_registeredZones.ContainsKey(z.Label))
            m_instance.m_registeredZones.Remove(z.Label);
    }
}
