/* Code by Alexander Tulloh, Stegabyte 2013
 * Feel free to use for whatever you want, but if something goes wrong, it is on you! */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class Octree
	{
		private OctreeNode _RootNode;
		
		private int _MaxDepth;
		
		public Octree(Bounds bounds, int maxDepth)
		{
			_MaxDepth = maxDepth;
			_RootNode = new OctreeNode(bounds, 0, maxDepth);	
		}
		
		public void Add(GameObject gameObject)
		{
			_RootNode.Add(gameObject);
		}
		
		public void Add(GameObject gameObject, bool enabled)
		{
			_RootNode.Add(gameObject);
			_RootNode.GetContent(gameObject).Enabled = enabled;
		}
		
		public void UpdatePosition(GameObject gameObject)
		{
			var enabled = _RootNode.GetContent(gameObject).Enabled;
			_RootNode.Remove(gameObject);
			_RootNode.Add (gameObject);
			_RootNode.GetContent(gameObject).Enabled = enabled;
		}
		
		public void UpdateEnabled(GameObject gameObject, bool enabled)
		{
			_RootNode.GetContent(gameObject).Enabled = enabled;
		}
		
		public void Remove(GameObject gameObject)
		{
			_RootNode.Remove(gameObject);
		}
		
		public void Clear()
		{
			_RootNode = new OctreeNode(_RootNode.Bounds,0, _MaxDepth);
		}
		
		public void FindClosest(Vector3 position, out OctreeClosestPoint result)
		{
			this.FindClosest(position, out result, 0x0);
		}
		
		public void FindClosest(Vector3 position, out OctreeClosestPoint result, int mask)
		{
			_RootNode.FindClosest(position, out result, mask);
		}
		
		public bool Raycast(Ray ray, float dist, out OctreeRaycastHit hit)
		{
			return this.Raycast(ray, dist, out hit, 0x0);
		}
		
		public bool Raycast(Ray ray, float dist, out OctreeRaycastHit hit, int mask)
		{
			return _RootNode.Raycast(ray, dist, out hit, mask);
		}
		
		public bool Raycast(Ray ray, float dist, int mask)
		{
			OctreeRaycastHit hit;
			ray.direction = ray.direction.normalized;
			return _RootNode.Raycast(ray, dist, out hit, mask);
		}
		
		public bool Raycast(Ray ray, float dist)
		{
			OctreeRaycastHit hit;
			
			return _RootNode.Raycast(ray, dist, out hit, 0x0);
		}
	
		public void OnPostRender()
		{
			_RootNode.OnPostRender();
		}
	}
}

