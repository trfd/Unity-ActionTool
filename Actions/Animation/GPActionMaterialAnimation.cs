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
	[GPActionAlias("Animation/MaterialAnimation")]
	public class GPActionMaterialAnimation : GPAction 
	{
		public enum FieldType
		{
			NONE,
			COLOR,
			FLOAT
		}

		#region Private Members

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
        private GPActionMaterialPropertyAnimation m_impl;

		/// <summary>
		/// The type of animation
		/// </summary>
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private FieldType m_animationType;

		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private bool m_useThis;

		/// <summary>
		/// Material animated.
		/// </summary>
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private Material m_material;

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
	
		#endregion

		#region Properties

		public FieldType AnimationType
		{
			get{ return m_animationType; }
			set
			{
				if(m_animationType == value)
					return;

				m_animationType = value;

				ChangeImpl();
			}
		}

        public GPActionMaterialPropertyAnimation Implementation
		{
			get{ return m_impl; }
		}

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

		protected override void OnTrigger()
		{
			m_impl.Trigger();
		}

		protected override void OnUpdate()
		{
			if(m_impl.HasEnded)
			{
				End();
				return;
			}

			m_impl.Update();
		}

		#endregion

		#region Internal

		private void ChangeImpl()
		{
			m_impl = null;

			System.Type implType;

			switch(m_animationType)
			{
			case FieldType.NONE:
				return;
			case FieldType.COLOR:
				implType = typeof(GPActionMaterialColorAnimation);
				break;
			case FieldType.FLOAT:
                implType = typeof(GPActionMaterialFloatAnimation);
				break;
			default:
				Debug.LogError("Unsupported type: "+m_animationType);
				return;
			}

			m_impl = (GPActionMaterialPropertyAnimation) this.gameObject.AddComponent(implType); //ScriptableObject.CreateInstance(implType);

			m_impl._parent = this;
			m_impl.ParentHandler = ParentHandler;

		    m_impl.enabled = false;
            m_impl.hideFlags = HideFlags.HideInInspector;

		    m_impl._name = _name + "_" + m_impl.GetType().ToString();
		}

		private void ChangeMaterial()
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
