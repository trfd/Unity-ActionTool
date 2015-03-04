//
// GPActionMaterialAnimation.cs
//
// Author(s):
//       Baptiste Dupy <baptiste.dupy@gmail.com>
//
// Copyright (c) 2014
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;
using System.Collections;

namespace ActionTool
{
	[System.Serializable]
	[GPActionHide]
	public class GPActionMaterialAnimation : GPAction 
	{
		#region Private Members

		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private bool m_useThis;

		/// <summary>
		/// Material animated.
		/// </summary>
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private Material m_material;

        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private Renderer m_renderer;

		#endregion

		#region Protected Members
		
		/// <summary>
		/// Animation Timer.
		/// </summary>
		protected Utils.Timer m_timer;
		
		#endregion

		#region Public Members

		/// <summary>
		/// Duration of animation.
		/// </summary>
		public float _duration;
		
		/// <summary>
		/// The key of animated variable.
		/// </summary>
		public string _animatedVariable;

        public bool _useRenderer;
	
		#endregion

		#region Properties

		public bool UseThisObject
		{
			get{ return m_useThis; }
			set
			{ 
				if(m_useThis == value)
					return;

				m_useThis = value; 

				ChangeMaterial();
			}
		}

		public Material Material
		{
			get{ return m_material;  }
			set{ m_material = value; CheckMaterial(); }
		}


		#endregion

		#region Action Override

		#endregion

		#region Internal

		protected void ChangeMaterial()
		{
			if(m_useThis)
			{
				Renderer thisRenderer = ParentGameObject.GetComponent<Renderer>();

				if(thisRenderer == null)
					return;
#if UNITY_EDITOR
                if(UnityEditor.EditorApplication.isPlaying)
                    m_material = thisRenderer.material;
                else
                    m_material = thisRenderer.sharedMaterial;
#else
                m_material = thisRenderer.material;
#endif
            }
		}

		private void CheckMaterial()
		{
			Renderer thisRenderer = ParentGameObject.GetComponent<Renderer>();

			if(thisRenderer == null)
			{
				m_useThis = false;
			}
			else
			{
#if UNITY_EDITOR
			    m_useThis = ((UnityEditor.EditorApplication.isPlaying  && m_material == thisRenderer.material) ||
			                 (!UnityEditor.EditorApplication.isPlaying && m_material == thisRenderer.sharedMaterial));

#else
                m_useThis = (m_material == thisRenderer.material);
#endif
			}
		}

		#endregion
	}
}
