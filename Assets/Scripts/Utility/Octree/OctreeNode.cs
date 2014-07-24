/* Code by Alexander Tulloh, Stegabyte 2013
 * Feel free to use for whatever you want, but if something goes wrong, it is on you! */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssemblyCSharp
{
	public class OctreeNode
	{						
		private bool _IsLeaf;
		private int _Level;
		private int _MaxLevel;
		private List<IOctreeContent> _Contents;		
		private OctreeNode[] _Children;
		private Bounds _ActualBounds;
		
		public ReadOnlyCollection<IOctreeContent> Contents {get;private set;}
		public Bounds Bounds {get;private set;}
		
		public OctreeNode (Bounds bounds, int level, int maxLevel)
		{
			_Level = level;
			_MaxLevel = maxLevel;
			_IsLeaf = true;
			_Children = new OctreeNode[8];
			_Contents = new List<IOctreeContent>(1);
			_ActualBounds = bounds;
			this.Bounds = bounds;
			this.Contents = _Contents.AsReadOnly();
		}
		
		public void Add(GameObject gameObject)
		{
			var content = OctreeContentFactory.Create(gameObject);
			this.Add(content);
		}
		
		public void Add(IOctreeContent content)
		{
			var wb = content.WorldBounds;

			if(!this.Bounds.Intersects(wb))
				throw new UnityException("Cannot add an object outside of octree");
			
			// just add if empty
			if((_IsLeaf && _Contents.Count == 0) || _Level == _MaxLevel)
			{
				_Contents.Add (content);
			}
			else
			{				
				if(wb.center.Equals(this.Bounds.center) || (_Contents.Count > 0 && _Contents.TrueForAll(i => wb.center.Equals(i.WorldBounds.center))))
				{
					_Contents.Add(content);
				}
				else
				{
					if(_IsLeaf)
					{
						// build children
						_IsLeaf = false;
						
						var ex = this.Bounds.extents * 0.5f;
						
						_Children[0] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(-ex.x, -ex.y, -ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						_Children[1] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(-ex.x, -ex.y, ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						_Children[2] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(ex.x, -ex.y, -ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						_Children[3] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(ex.x, -ex.y, ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						_Children[4] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(-ex.x, ex.y, -ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						_Children[5] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(-ex.x, ex.y, ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						_Children[6] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(ex.x, ex.y, -ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						_Children[7] = new OctreeNode(new Bounds(this.Bounds.center + new Vector3(ex.x, ex.y, ex.z), this.Bounds.extents), _Level + 1, _MaxLevel);
						
						// move any existing items
						foreach(var i in _Contents.ToArray())
						{
							var iwb = i.WorldBounds;
							
							if(!iwb.center.Equals(this.Bounds.center))
							{
								for(var j = 0; j < 8; j++)
								{
									if(_Children[j].Intersects(iwb))
										_Children[j].Add(i);
								}
								
								_Contents.Remove(i);
							}
						}
					}
					
					// add new item into child
					for(var j = 0; j < 8; j++)
					{
						if(_Children[j].Intersects(wb))
							_Children[j].Add(content);
					}
				}
			}
		}
		
		public void Remove(GameObject gameObject)
		{
			var match = _Contents.FirstOrDefault(c => c.GameObject == gameObject);

			if(match != null)
				_Contents.Remove(match);
			
			if(!_IsLeaf)
			{
				var emptyChildren = 0;
				var hasFloater = true;
				List<IOctreeContent> floaters = null;
				
				for(var i = 0; i < 8; i++)
				{
					_Children[i].Remove(gameObject);
					
					if(_Children[i].IsEmpty())
					{
						emptyChildren++;
					}
					else if(_Children[i].IsLeaf())
					{
						if(floaters == null)
						{
							floaters = _Children[i].Contents.ToList();
						}
						else if(!floaters.Equals(_Children[i].Contents.ToList()))
						{
							hasFloater = false;
						}
					}
					else
					{
						hasFloater = false;
					}
				}
				
				if(emptyChildren == 8)
				{
					_IsLeaf = true;
					_Children = new OctreeNode[8];
				}
				else if(hasFloater && _Contents.Count == 0)
				{
					foreach(var p in floaters)
						_Contents.Add(p);
					
					_IsLeaf = true;
					_Children = new OctreeNode[8];
				}
			}
		}

		public IOctreeContent GetContent(GameObject gameObject)
		{
			var match = _Contents.FirstOrDefault(c => c.GameObject == gameObject);
			
			if(match != null)
				return match;
			else if(!_IsLeaf)
			{
				for(var i = 0; i < 8; i++)
				{
					match =	_Children[i].GetContent(gameObject);
					
					if(match != null)
						return match;
				}
			}
			
			return null;
		}
		
		public bool IsEmpty()
		{
			return _IsLeaf && _Contents.Count == 0;
		}
		
		public bool IsLeaf()
		{
			return _IsLeaf;
		}
		
		public bool Intersects(Bounds bounds)
		{
			return _ActualBounds.Intersects(bounds);
		}
		
		public bool IntersectsRay(Ray ray)
		{
			return _ActualBounds.IntersectRay(ray);
		}
		
		public bool Raycast(Ray ray, float dist, out OctreeRaycastHit hit, int mask)
		{
			var didHit = false;
			
			hit = new OctreeRaycastHit(float.MaxValue, Vector3.zero, null);
			
			if(!_IsLeaf)
			{
				// sort children
				var ch = new List<KeyValuePair<float,OctreeNode>>(8);
				
				for(var i = 0; i < 8; i++)
				{
					float d;
					if(!_Children[i].IsEmpty() && _Children[i].Bounds.IntersectRay(ray, out d) && d < dist)
					{
						ch.Add(new KeyValuePair<float,OctreeNode>(d, _Children[i]));
					}
				}
				
				ch.Sort(delegate(KeyValuePair<float, OctreeNode> x, KeyValuePair<float, OctreeNode> y) {
					return x.Key.CompareTo(y.Key);
				});
				
				for(var i = 0; i < ch.Count; i++)
				{
					// ignore if already found something closer than whole box
					if(hit.Distance < ch[i].Key)
						break;
					
					OctreeRaycastHit h;
					if(ch[i].Value.Raycast(ray, dist, out h, mask) && h.Distance < hit.Distance && h.Distance < dist)
					{
						hit = h;
						didHit = true;
					}
				}
			}
			
			// cast against actual items
			foreach(var content in _Contents)
			{
				if((mask == 0x0 || ((1 << content.GameObject.layer) & mask) != 0x0) && content.Enabled)
				{
					float d;
					var b = content.WorldBounds;
					if(b.IntersectRay(ray, out d) && d < hit.Distance && d < dist)
					{
						hit = new OctreeRaycastHit(d, ray.origin + ray.direction.normalized * d, content);
						didHit = true;
					}
				}
			}
			
			return didHit;
		}
		
		public bool FindClosest(Vector3 position, out OctreeClosestPoint result, int mask)
		{
			throw new NotImplementedException();
		}

		// Drawing

		private static Material _LineMaterial;

		public void OnPostRender()
		{
			if(_LineMaterial == null)
			{
				_LineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
				                            "SubShader { Pass { " +
				                            "    Blend SrcAlpha OneMinusSrcAlpha " +
				                            "    ZWrite Off Cull Off Fog { Mode Off } " +
				                            "    BindChannels {" +
				                            "      Bind \"vertex\", vertex Bind \"color\", color }" +
				                            "} } }" );
				_LineMaterial.hideFlags = HideFlags.HideAndDontSave;
				_LineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
			}

			_LineMaterial.SetPass(0);
			
			GL.Begin( GL.LINES );
			GL.Color( new Color(1,0,1,0.5f) );

			// this cube
			_DrawCube(this.Bounds.center, this.Bounds.size);

			/*GL.Vertex3( 0, 0, 0 );
			GL.Vertex3( 1, 0, 0 );
			GL.Vertex3( 0, 1, 0 );
			GL.Vertex3( 1, 1, 0 );


			GL.Color( new Color(0,0,0,0.5f) );
			GL.Vertex3( 0, 0, 0 );
			GL.Vertex3( 0, 1, 0 );
			GL.Vertex3( 1, 0, 0 );
			GL.Vertex3( 1, 1, 0 );*/
			GL.End();
			
			if(!_IsLeaf)
			{
				for(var j = 0; j < 8; j++)
				{
					_Children[j].OnPostRender();
				}
			}


			/*
			Gizmos.color = new Color(1,1,1,0.1f);
			Gizmos.DrawWireCube(this.Bounds.center, this.Bounds.size);
			
			if(!_IsLeaf)
			{
				for(var j = 0; j < 8; j++)
				{
					_Children[j].OnDrawGizmos();
				}
			}
			
			foreach(var p in _Contents)
			{
				Gizmos.color = p.Enabled ? new Color(1,1,0, 0.5f) : new Color(1,0,0,0.5f);
				Gizmos.DrawWireCube(p.WorldBounds.center, p.WorldBounds.size);
			}*/
		}

		private static void _DrawCube(Vector3 centre, Vector3 size)
		{
			var halfSize = size * 0.5f;
			
			var p1 = centre + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
			var p2 = centre + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
			var p3 = centre + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
			var p4 = centre + new Vector3(halfSize.x, -halfSize.y, -halfSize.z); 
			
			var p5 = centre + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
			var p6 = centre + new Vector3(-halfSize.x, halfSize.y, halfSize.z);
			var p7 = centre + new Vector3(halfSize.x, halfSize.y, halfSize.z);
			var p8 = centre + new Vector3(halfSize.x, halfSize.y, -halfSize.z); 
			
			GL.Vertex(p1);
			GL.Vertex(p2);
			GL.Vertex(p2);
			GL.Vertex(p3);
			GL.Vertex(p3);
			GL.Vertex(p4);
			GL.Vertex(p4);
			GL.Vertex(p1);

			GL.Vertex(p5);
			GL.Vertex(p6);
			GL.Vertex(p6);
			GL.Vertex(p7);
			GL.Vertex(p7);
			GL.Vertex(p8);
			GL.Vertex(p8);
			GL.Vertex(p5);
			
			GL.Vertex(p1);
			GL.Vertex(p5);
			GL.Vertex(p2);
			GL.Vertex(p6);
			GL.Vertex(p3);
			GL.Vertex(p7);
			GL.Vertex(p4);
			GL.Vertex(p8);
		}
	}
}

