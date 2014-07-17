using UnityEngine;
using System.Collections;

public class TextureTools
{
    //Quite slow
    public static Texture2D DeepCopy(Texture2D input)
    {
        Texture2D output = new Texture2D(input.width, input.height);
        output.SetPixels(input.GetPixels());
        output.Apply();
        return output;
    }

    public static Vector2 GetSlopeAt(Texture2D input, int x, int y, int gap)
    {
        float valAtPos = input.GetPixel(x, y).grayscale;
        Vector2 gradient = new Vector2();

        if (x >= input.width - gap - 2 || y >= input.height - gap - 2)
        {
            return gradient;
        }

        gradient.x = input.GetPixel(x + gap, y).grayscale - valAtPos;
        gradient.y = input.GetPixel(x, y + gap).grayscale - valAtPos;
        return gradient;
    }

    public static Texture2D GetDerivativeMap(Texture2D tex, int subSamples)
    {
        Texture2D output = new Texture2D(tex.width, tex.height);

        Vector2 gradient = new Vector2();
        float maxGrad = 0.0f;
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.width; y++)
            {
                gradient = GetSlopeAt(tex, x, y, subSamples);
                float grad = gradient.magnitude;
                if (grad > maxGrad)
                    maxGrad = grad;

                output.SetPixel(x, y, new Color(grad, grad, grad, 1.0f));
            }
        }
        //Debug.Log(maxGrad);
        //Normalize the gradient map
        Color col = new Color();
        if (maxGrad != 0.0f)
        {
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.width; y++)
                {
                    col = output.GetPixel(x, y);
                    col.r /= maxGrad;
                    col.g /= maxGrad;
                    col.b /= maxGrad;
                    output.SetPixel(x, y, col);
                }
            }
        }
        output.Apply();
        return output;
    }
}
