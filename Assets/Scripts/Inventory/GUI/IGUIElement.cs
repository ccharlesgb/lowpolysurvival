using UnityEngine;

interface IGUIElement
{
	void Update();

	void Draw();

	Rect GetWindowSize();

}
