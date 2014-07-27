using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshairTexture;
    public float crosshairScale = 1.0f;

    private Rect crosshairPosition;


    void OnGUI()
    {
        if (crosshairTexture == null)
        {
            enabled = false;
            return;
        }

        int crossWidth = (int)(crosshairTexture.width/crosshairScale);
        int crossHeight = (int)(crosshairTexture.height/crosshairScale);

        int w = Screen.width;
        int h = Screen.height;
        crosshairPosition.x = (w  - crossWidth) / 2;
        crosshairPosition.y = (h - crossHeight) / 2;
        crosshairPosition.width = crossWidth;
        crosshairPosition.height = crossHeight;
        GUI.DrawTexture(crosshairPosition, crosshairTexture);
    }
}
