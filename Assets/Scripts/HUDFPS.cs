using System;
using UnityEngine;

/// <summary>
///		FPS Counter.
/// </summary>
/// From http://wiki.unity3d.com/index.php/FramesPerSecond, by Opless.
/// Modified slightly
public class HUDFPS : MonoBehaviour
{
	// Attach this to a GUIText to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// correct overall FPS even if the interval renders something like
	// 5.5 frames.

	public float UpdateInterval = 0.5F;

	private float _accumFPS; // FPS accumulated over the interval
    private float _accumMS; //Delta time accumulated
	private int _frames; // Frames drawn over the interval
	private float _timeleft; // Left time for current interval

	private void Start()
	{
		if (!guiText)
		{
			Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			enabled = false;
			return;
		}
		_timeleft = UpdateInterval;
	}

	private void Update()
	{
		_timeleft -= Time.deltaTime;
		_accumFPS += Time.timeScale/Time.deltaTime;
	    _accumMS += (Time.deltaTime * 1000.0f) / Time.timeScale;
		++_frames;

		// Interval ended - update GUI text and start new interval
		if (_timeleft <= 0.0)
		{
			// display two fractional digits (f2 format)
            float fps = _accumFPS/ _frames;
		    float ms = _accumMS/_frames;
            string format = String.Format("{0:F2} ({1:F2} FPS)",ms, fps);
			guiText.text = format;

			guiText.material.color = GetTextColor(fps);

			//	DebugConsole.Log(format,level);
			_timeleft = UpdateInterval;
            _accumFPS = 0.0F;
		    _accumMS = 0.0f;
			_frames = 0;
		}
	}

	private Color GetTextColor(float fps)
	{
		if (fps < 30)
		{
			return Color.yellow;
		}
		if (fps < 10)
		{
			return Color.red;
		}

		return Color.green;
	}
}