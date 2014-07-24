/* Code by Alexander Tulloh, Stegabyte 2013
 * Feel free to use for whatever you want, but if something goes wrong, it is on you! */
using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class OctreeRaycastHit
	{
		public float Distance {get;private set;}
		public IOctreeContent Content {get;private set;}
		public Vector3 Position {get;private set;}
		
		public OctreeRaycastHit (float distance, Vector3 position, IOctreeContent content)
		{
			this.Distance = distance;
			this.Content = content;
			this.Position = position;
		}
	}
}

