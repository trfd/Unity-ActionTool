//
// GPActionMaterialAnimationInspector.cs
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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
	[GPActionInspectorAttribute(typeof(GPActionMaterialColorAnimation))]
	public class GPActionMaterialColorAnimationInspector : GPActionDefaultInspector
    {
        #region Private Members

		private string[] m_properties;
		private int[] m_propertyIndexes;

		private int m_selectedIndex;

        #endregion

        protected override void OnInspectorGUI()
		{
			GPActionMaterialColorAnimation anim = (GPActionMaterialColorAnimation) TargetAction;

			anim.UseThisObject = EditorGUILayout.Toggle("Use This Object",anim.UseThisObject);

			Material newMaterial = (Material) EditorGUILayout.ObjectField("Material",anim.Material,typeof(Material),true);

			if(newMaterial != anim.Material)
				ChangeMaterial(newMaterial);

			if(anim.Material == null)
				return;
            else
            {
				if(m_properties == null)
					CreatePropertyList(anim.Material.shader);

				if(m_properties.Length == 0)
				{
					EditorGUILayout.HelpBox("No color properties",MessageType.Info);
					return;
				}

				m_selectedIndex = System.Array.IndexOf(m_properties,anim._animatedVariable);

				m_selectedIndex = Mathf.Max(0,m_selectedIndex);

				m_selectedIndex = EditorGUILayout.Popup("Property",m_selectedIndex, m_properties);

				anim._animatedVariable = m_properties[m_selectedIndex];
            }

			anim._duration = EditorGUILayout.FloatField("Duration",anim._duration);

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(SerialObject.FindProperty("_colorMap"));
			if(EditorGUI.EndChangeCheck())
			{
				SerialObject.ApplyModifiedProperties();
			}

			anim._curve = EditorGUILayout.CurveField("Curve"   ,anim._curve);

			if(anim.UseThisObject && anim.ParentGameObject.GetComponent<Renderer>())
			{
				EditorGUILayout.HelpBox("'Use This Action' requires a Renderer in the GameObject",MessageType.Error);
			}
		}

		private void CreatePropertyList(Shader shader)
		{
			string[] properties = new string[ShaderUtil.GetPropertyCount(shader)];
			
			for (int i = 0; i < properties.Length; i++)
				properties[i] = ShaderUtil.GetPropertyName(shader, i);

			List<string> colorProperties = new List<string>();
			List<int> indexProperties = new List<int>();


			for(int i = 0; i<properties.Length ; i++)
			{
				if(ShaderUtil.GetPropertyType(shader,i) == ShaderUtil.ShaderPropertyType.Color)
				{
					colorProperties.Add(properties[i]);
					indexProperties.Add(i);
				}
			}

			m_properties = colorProperties.ToArray();
			m_propertyIndexes =  indexProperties.ToArray();
		}

		private void ChangeMaterial(Material newMaterial)
		{
			GPActionMaterialAnimation anim = (GPActionMaterialAnimation) TargetAction;

			anim.Material = newMaterial;

			if(newMaterial == null)
				return;

			CreatePropertyList(newMaterial.shader);
		}
	}
}
