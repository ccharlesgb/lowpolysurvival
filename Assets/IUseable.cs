using UnityEngine;
using System.Collections;

public interface IUseable
{
    void OnHoverStart(); //What happens when the object gets hovered over
    void OnHoverEnd(); //What happens when it stops being hovered over
    void OnUse(); //What happens when it gets used
}
