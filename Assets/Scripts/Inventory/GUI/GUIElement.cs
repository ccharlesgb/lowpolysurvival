using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GUIElement : IGUIElement
{

	protected List<IGUIElement> Elements;
	protected IGUIElement ParentElement;
	protected GUIPosition Position;
	protected Rect WindowRect;

	protected GUIElement(IGUIElement parentElement, GUIPosition position)
	{
		ParentElement = parentElement;
		Position = position;

		Elements = new List<IGUIElement>();
	}

	public virtual void Update()
	{
		// Update position.
		WindowRect.x = ParentElement.GetWindowSize().x + Position.x;
		WindowRect.y = ParentElement.GetWindowSize().y + Position.y;

		foreach (IGUIElement element in Elements)
		{
			element.Update();
		}
	}

	public virtual void Draw()
	{
		foreach (IGUIElement element in Elements)
		{
			element.Draw();
		}
	}

	public Rect GetWindowSize()
	{
		return WindowRect;
	}
}