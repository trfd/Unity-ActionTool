//
// GPConditionCompareDistanceInspector.cs
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
	[GPConditionInspector(typeof(GPConditionCompareDistance))]
	public class GPConditionCompareDistanceInspector : GPConditionInspector
	{
		#region Private Members
		
		private ValueProviderEditor<GameObject> m_providerAEditor;
		private ValueProviderEditor<GameObject> m_providerBEditor;
		private ValueProviderEditor<float> m_providerDistanceEditor;
		private ValueComparerEditor<float> m_comparerEditor;
		
		#endregion
		
		protected override void OnInspectorGUI()
		{
			GPConditionCompareDistance compare = (GPConditionCompareDistance) Condition;
			
			// Comparer
			
			if(m_comparerEditor == null)
				m_comparerEditor = new ValueComparerEditor<float>();
			
			compare._comparerType.Type = m_comparerEditor.Display(compare._comparerType.Type);
			
			// Provider A
			
			EditorGUILayout.LabelField("Object A");
			
			EditorGUI.indentLevel++;
			
			if (m_providerAEditor == null)
			{
				m_providerAEditor = new ValueProviderEditor<GameObject>();
				m_providerAEditor.Provider = compare._providerA;
			}
			
			if (m_providerAEditor.Provider != compare._providerA)
				m_providerAEditor.Provider = compare._providerA;
			
			
			m_providerAEditor.Display();
			
			EditorGUI.indentLevel--;
			
			// Provider B
			
			EditorGUILayout.LabelField("Object B");
			
			EditorGUI.indentLevel++;
			
			if (m_providerBEditor == null)
			{
				m_providerBEditor = new ValueProviderEditor<GameObject>();
				m_providerBEditor.Provider = compare._providerB;
			}
			
			if (m_providerBEditor.Provider != compare._providerB)
				m_providerBEditor.Provider = compare._providerB;
			
			
			m_providerBEditor.Display();
			
			EditorGUI.indentLevel--;

			// Distance

			EditorGUILayout.LabelField("Compare To");
			
			EditorGUI.indentLevel++;

			if (m_providerDistanceEditor == null)
			{
				m_providerDistanceEditor = new ValueProviderEditor<float>();
				m_providerDistanceEditor.Provider = compare._distance;
			}
			
			if (m_providerDistanceEditor.Provider != compare._distance)
				m_providerDistanceEditor.Provider = compare._distance;

			m_providerDistanceEditor.Display();
			
			EditorGUI.indentLevel--;
		}
	}
}