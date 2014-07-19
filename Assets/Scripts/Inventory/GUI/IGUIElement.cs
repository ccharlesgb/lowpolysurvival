using UnityEngine;

namespace LowPolySurvival.Inventory
{
	interface IGUIElement
	{
		void Update();

		void Draw();

		Rect GetWindowSize();

	}
}
