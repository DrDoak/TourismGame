using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeOfDay
{
    public float Minutes = 0f;
    public float Hours = 0f;
    [HideInInspector]
    public float TotalMinutes = 0f;

    public TimeOfDay()
    {

    }
    public TimeOfDay(float TotalMin)
    {
        TotalMinutes = TotalMin;
        Hours = Mathf.Floor(TotalMin / 60f);
        Minutes = TotalMin - (Hours * 60f);
    }
    public float ToMinutes()
    {
        return (Hours * 60f) + Minutes;
    }
}
public class DayCycleController : MonoBehaviour
{
    public float timeRatio = 1.0f;
    public float AdjustedTimeElapsed;
    public TimeOfDay CurrentTime;

    public TimeOfDay SunriseTime;
    public TimeOfDay SunsetTime;
    public TimeOfDay SunriseDuration;
    public Color dawnColor = new Color(1.0f, 0.4f, 0.4f, 1.0f);
    public Color dayColor = Color.white;
    public Color duskColor = new Color(0.8f, 0.6f, 0.0f, 1.0f);
    public Color nightColor = new Color(0.2f,0.2f,1.0f,1.0f);
    public Color amCol;
    public GameObject SunObject;
    public GameObject MoonObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        AdjustedTimeElapsed += Time.deltaTime * Time.timeScale;

        float TimeMinutes = ((int)(AdjustedTimeElapsed)) % (60 * 24);
        CurrentTime = new TimeOfDay(TimeMinutes);
        float proportionTime = 0f;
        if (TimeMinutes > (SunriseTime.ToMinutes() - SunriseDuration.ToMinutes()/2) && TimeMinutes < SunriseTime.ToMinutes())
        {
            proportionTime = TimeMinutes - (SunriseTime.ToMinutes() - SunriseDuration.ToMinutes() / 2);
            setColor(Color.Lerp(nightColor, dawnColor, (proportionTime) / (SunriseDuration.ToMinutes() / 2)));
        } else if (TimeMinutes < (SunriseTime.ToMinutes() + SunriseDuration.ToMinutes() / 2) && TimeMinutes > SunriseTime.ToMinutes())
        {
            proportionTime = TimeMinutes - (SunriseTime.ToMinutes());
            setColor(Color.Lerp(dawnColor, dayColor, (proportionTime) / (SunriseDuration.ToMinutes() / 2)));
        }

        if (TimeMinutes > (SunsetTime.ToMinutes() - SunriseDuration.ToMinutes() / 2) && TimeMinutes < SunsetTime.ToMinutes())
        {
            proportionTime = TimeMinutes - (SunsetTime.ToMinutes() - SunriseDuration.ToMinutes() / 2);
            setColor(Color.Lerp(dayColor, duskColor, (proportionTime) / (SunriseDuration.ToMinutes() / 2)));
        }
        else if (TimeMinutes < (SunsetTime.ToMinutes() + SunriseDuration.ToMinutes() / 2) && TimeMinutes > SunsetTime.ToMinutes())
        {
            proportionTime = TimeMinutes - (SunsetTime.ToMinutes());
            setColor(Color.Lerp(duskColor, nightColor, (proportionTime) / (SunriseDuration.ToMinutes() / 2)));
        }
        SunObject.transform.rotation = Quaternion.Euler(35f, Mathf.Lerp(-50, 50, TimeMinutes / (24 * 60)), 0f);
    }

    private void setColor(Color c)
    {
        SunObject.GetComponent<Light>().color = c;
        amCol = c;
    }

    public void SetDayNightColor(Color dColor, Color nColor)
    {
        dayColor = dColor;
        nightColor = nColor;
    }
}
