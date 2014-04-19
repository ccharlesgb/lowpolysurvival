using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

	// Use this for initialization
	
	private Material matSky;
	
	public float dayMinutes = 1;
	public float refreshWait = 0.2f;
	
	public float speed = 0.01f;
	public float timeFrac;
	
	private float startTime;
	private float dayStartTime;
	
	public Shader skyShader;
	
	public int day = 0;
	
	public Color dayCol;
	
	private Transform _transform;
	
	void Start () 
	{
		//matSky = new Material(skyShader);
		//matSky.name = "SkyboxMat";
		//RenderSettings.skybox = matSky;
		
		//matSky.SetColor ("_DayColor", dayCol);
		
		startTime = Time.time;
		dayStartTime = Time.time;
		//Caching
		_transform = transform;
		timeFrac = 0;
		day = 0;
		speed = refreshWait / (dayMinutes * 60);
		StartCoroutine("ProgressSun");
	}
	
	float GetIntensity(float frac)
	{
		//Guassina funciton to to sun intensity
		float sigma = 0.11f;
		float mean = 0.25f;
		float peak = 0.3f;
		float exponent = Mathf.Pow ((frac-mean), 2.0f) / (2*sigma*sigma);
		float intensity = peak * Mathf.Exp (-exponent);
			
		return intensity;
	}
	
	IEnumerator ProgressSun()
	{	
		while (true)
		{
			timeFrac = (Time.time - dayStartTime) / (dayMinutes * 60.0f);
			//timeFrac = timeFrac - day;
			if (timeFrac >= 1)
			{
				day++;
				dayStartTime = Time.time;
				timeFrac = 0.0f;	
			}
			GetComponent<Light>().intensity = GetIntensity (timeFrac);
			Vector3 sunRot = new Vector3(360 * timeFrac,0,0);
			Quaternion newRot = Quaternion.Euler (sunRot);
			_transform.rotation = newRot;
			yield return new WaitForSeconds(refreshWait);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
