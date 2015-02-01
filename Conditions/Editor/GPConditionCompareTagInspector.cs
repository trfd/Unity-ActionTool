//
// GPConditionCompareStringInspector.cs
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
	[GPConditionInspector(typeof(GPConditionCompareTag))]
	public class GPConditionCompareTagInspector : GPConditionInspector
	{
		#region Private Members

		private ValueProviderEditor<GameObject> m_providerEditor;

		#endregion
		
		protected override void OnInspectorGUI()
		{
			GPConditionCompareTag compare = (GPConditionCompareTag) Condition;

			// Provider

			EditorGUILayout.LabelField("Object");

			EditorGUI.indentLevel++;

			if (m_providerEditor == null)
			{
				m_providerEditor = new ValueProviderEditor<GameObject>();
				m_providerEditor.Provider = compare._objectProvider;
			}
			
			if (m_providerEditor.Provider != compare._objectProvider)
				m_providerEditor.Provider = compare._objectProvider;
			
			
			m_providerEditor.Display();

			EditorGUI.indentLevel--;

			EditorGUILayout.LabelField("Object");

			EditorGUI.indentLevel++;

			compare._tag = EditorGUILayout.TagField("Tag",compare._tag);

			EditorGUI.indentLevel--;
		}
	}
}