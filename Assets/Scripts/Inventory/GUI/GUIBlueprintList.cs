using System;
using LowPolySurvival.Inventory.Blueprints;
using UnityEngine;

namespace LowPolySurvival.Inventory
{
	internal class GUIBlueprintList : GUIElement
	{

		private BlueprintList _blueprintList;

		// Use this for initialization
		public GUIBlueprintList(IGUIElement parentElement, GUIPosition position) : base(parentElement, position)
		{
			// Load the global blueprint list.
			_blueprintList = Blueprints.MasterList.Instance.List;

			WindowRect = new Rect(0, 0, 200, 200);
		}

		public override void Draw()
		{
			for (int i = 0; i < _blueprintList.List.Count; i++)
			{
				Blueprint blueprint = _blueprintList.List[i];

				var rect = new Rect(WindowRect.x, WindowRect.y + 20 * i, 100, 20);
				if (GUI.Button(rect, blueprint.PrintName))
				{
					// TODO: Handle blueprint usage.
				}
			}
		}
	}
}