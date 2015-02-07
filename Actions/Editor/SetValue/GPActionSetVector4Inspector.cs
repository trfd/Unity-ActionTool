//
// GPActionSetVector4Inspector.cs
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
	[GPActionInspector(typeof(GPActionSetVector4))]
	public class GPActionSetVector4Inspector : GPActionInspector 
	{
		#region Private Members

		private ValueProviderEditor<Vector4> m_providerAEditor;
		private ValueProviderEditor<Vector4> m_providerBEditor;

		#endregion

		protected override void OnInspectorGUI()
		{
			GPActionSetVector4 action = (GPActionSetVector4) TargetAction;

			// Provider A
			
			EditorGUILayout.LabelField("Variable");
			
			EditorGUI.indentLevel++;
			
			if (m_providerAEditor == null)
			{
				m_providerAEditor = new ValueProviderEditor<Vector4>();
				m_providerAEditor.Provider = action._variable;
			}
			
			if (m_providerAEditor.Provider != action._variable)
				m_providerAEditor.Provider = action._variable;
			
			
			m_providerAEditor.Display();
			
			EditorGUI.indentLevel--;
			
			// Provider B
			
			EditorGUILayout.LabelField("Value");
			
			EditorGUI.indentLevel++;
			
			if (m_providerBEditor == null)
			{
				m_providerBEditor = new ValueProviderEditor<Vector4>();
				m_providerBEditor.Provider = action._newValue;
			}
			
			if (m_providerBEditor.Provider != action._newValue)
				m_providerBEditor.Provider = action._newValue;
			
			
			m_providerBEditor.Display();
			
			EditorGUI.indentLevel--;
		}
	}
}
