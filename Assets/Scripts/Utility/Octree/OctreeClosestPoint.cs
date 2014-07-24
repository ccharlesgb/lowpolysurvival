/* Code by Alexander Tulloh, Stegabyte 2013
 * Feel free to use for whatever you want, but if something goes wrong, it is on you! */
using System;

namespace AssemblyCSharp
{
	public class OctreeClosestPoint
	{
		public float Distance {get;private set;}
		public IOctreeContent Content {get;private set;}
		public OctreeNode Node {get;private set;}
		
		public OctreeClosestPoint (float distance, IOctreeContent content, OctreeNode node)
		{
			this.Distance = distance;
			this.Content = content;
			this.Node = node;
		}
	}
}

