/* Code by Alexander Tulloh, Stegabyte 2013
 * Feel free to use for whatever you want, but if something goes wrong, it is on you! */
using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public interface IOctreeContent
	{
		bool Enabled {get;set;}
		Bounds Bounds {get;}
		Bounds WorldBounds {get;}
		GameObject GameObject {get;}
	}

	public static class OctreeContentFactory
	{
		public static IOctreeContent Create(GameObject obj)
		{
			if(obj.GetComponentInChildren<Collider>() != null)
				return new CollidableContent(obj, obj.GetComponentInChildren<Collider>());

			else if(obj.GetComponentInChildren<Renderer>() != null)
				return new RenderableContent(obj, obj.GetComponentInChildren<Renderer>());

			else
				throw new NotImplementedException("Cannot generate octree content for object " + obj.name + " of type " + obj.GetType().Name);
		}
	}

	public abstract class OctreeContentBase : IOctreeContent
	{
		public virtual bool Enabled {get;set;}

		public GameObject GameObject {get; private set;}

		public abstract Bounds Bounds {get;}
		
		public abstract Bounds WorldBounds {get;}

		public OctreeContentBase(GameObject obj)
		{
			this.GameObject = obj;
		}
	}
	
	public class RenderableContent : OctreeContentBase
	{
		private Renderer _Renderer;
		
		public RenderableContent(GameObject obj, Renderer renderer):base(obj)
		{
			_Renderer = renderer;
		}
		
		public override Bounds Bounds 
		{
			get
			{
				return _Renderer.bounds;
			}
		}
		
		public override Bounds WorldBounds 
		{
			get
			{
				return _Renderer.bounds;
			}
		}
	}
	
	public class CollidableContent : OctreeContentBase
	{
		private Collider _Collider;
		
		public CollidableContent(GameObject obj, Collider collider):base(obj)
		{
			_Collider = collider;
		}
		
		public override Bounds Bounds 
		{
			get
			{
				return _Collider.bounds;
			}
		}
		
		public override Bounds WorldBounds 
		{
			get
			{
				return _Collider.bounds;
			}
		}
	}
}

