using UnityEngine;
using System.Collections;


public class InputState
{
    private static int menuCount = 0;
    public static bool IsMenuOpen()
    {
        return menuCount > 0;
    }

    public static void AddMenuLevel()
    {
        menuCount++;
    }

    public static void LowerMenuLevel()
    {
        menuCount--;
    }
}
