using System;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DayCycle : MonoBehaviour
{
    public bool isDay = false;

    [Range(0.0f,1.0f)]
    public float dayProgress = 0.0f; //Current progression 0-1
    public float dayLength = 1.0f; //Length of the day in minutes
    public AnimationCurve skyIntensity;
    [Range(0.0f, 1.0f)]
    public float sunHeight = 1.0f;
    public Light sun;

    [Range(0.0f,24.0f)]
    public float startTime; //Start hour
    [Range(0.0f,25.0f)]
    public float hourTime;

    private float timeOffset; //Offset for Time.time (sets the start hour)

    //Light Colours
    public Color ambientLightDay;
    public Color ambientLightNight;
    public Color ambientLightDusk;

    //Sky Colours
    public Color skyColorDay;
    public Color skyColorDay2;
    public AnimationCurve skyDayAmount;
    public Color skyColorNight;
    public Color skyColorNight2;
    public AnimationCurve skyNightAmount;
    public Color skyColorDusk;
    public Color skyColorDusk2;
    public AnimationCurve skyDuskAmount;

    public void Start()
    {
        dayProgress = startTime/24.0f;
        sun = GetComponent<Light>();
    }

    public void Update()
    {
        //Update progress
        timeOffset = startTime / 24.0f;
        float delta = Time.fixedDeltaTime;
        dayProgress = (Time.time/60.0f)/dayLength + timeOffset;
        dayProgress = dayProgress - Mathf.Floor(dayProgress);

        //Change light intensity based off curve
        sun.intensity = skyIntensity.Evaluate(dayProgress);

        //Rotate the sun
        Quaternion sunRot = transform.rotation;
        float xAng = Mathf.Sin(dayProgress * 2 * Mathf.PI);
        float yAng = Mathf.Cos(dayProgress * 2 * Mathf.PI);
        sunRot.SetLookRotation(new Vector3(xAng, yAng, 1.0f-sunHeight));
        transform.rotation = sunRot;

        //Debug.Log(xAng);

        TransitionSkyColor();

        //Just a helper
        hourTime = dayProgress*24.0f;
    }

    void TransitionSkyColor()
    {
        float dayAmount = skyDayAmount.Evaluate(dayProgress);
        float duskAmount = skyDuskAmount.Evaluate(dayProgress);
        float nightAmount = skyNightAmount.Evaluate(dayProgress);

        float amountSum = dayAmount + duskAmount + nightAmount;

        dayAmount /= amountSum;
        duskAmount /= amountSum;
        nightAmount /= amountSum;

        //Set sky color
        Color skyCol1 = skyColorDay*dayAmount+skyColorDusk*duskAmount+skyColorNight*nightAmount;
        Color skyCol2 = skyColorDay2 * dayAmount + skyColorDusk2 * duskAmount + skyColorNight2 * nightAmount;
        RenderSettings.skybox.SetColor("_TopColor", skyCol1);
        RenderSettings.skybox.SetColor("_BottomColor", skyCol2);
        //Set Fog Color
        RenderSettings.fogColor = skyCol2;

        //Set Ambiet Light
        Color newAmbient = ambientLightDay * dayAmount + ambientLightDusk * duskAmount + ambientLightNight * nightAmount;
        RenderSettings.ambientLight = newAmbient;
    }

    void OnDisable()
    {
        RenderSettings.skybox.SetColor("_TopColor", skyColorDay);
        RenderSettings.skybox.SetColor("_BottomColor", skyColorDay2);
    }
}