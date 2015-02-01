//
// GPActionEnableComponent.cs
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


namespace ActionTool
{
	[GPActionInspectorAttribute(typeof(GPActionEnableComponent))]
	public class GPActionEnableComponentInspector : GPActionDefaultInspector
	{
		#region Private Members

		Component[] m_components;

		string[] m_componentList;

		GameObject m_componentParentObject;

		int m_componentPopupIndex;

		#endregion

		public override void SetAction(GPAction action)
		{
			base.SetAction(action);

			GPActionEnableComponent ecAction = (GPActionEnableComponent) TargetAction;

			if(ecAction._component == null)
				return;

			m_componentParentObject = ecAction._component.gameObject;

			CreateComponentList();
		}

		protected override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GPActionEnableComponent action = (GPActionEnableComponent) TargetAction;

			if(m_componentList == null || m_components == null)
			{
				CreateComponentList();
			}

			bool prevThisObject = action._thisObject;

			action._thisObject = EditorGUILayout.ToggleLeft("This Object",action._thisObject);

			if(prevThisObject != action._thisObject)
			{
				if(action._thisObject && action._component != null)
				{
					action._component = null;
					m_componentParentObject = action.ParentGameObject;
				}

				CreateComponentList();
			}

			if(action._thisObject)
				m_componentParentObject = action.ParentGameObject;
			else
			{
				GameObject prevParent = m_componentParentObject;

				m_componentParentObject = (GameObject) EditorGUILayout.ObjectField("Parent Object",prevParent,typeof(GameObject),true);

				if(m_componentParentObject != prevParent)
				{
					action._component = null;
					CreateComponentList();
				}

			}

			if(m_componentParentObject == null)
			{
				m_componentList = null;
				m_components = null;
				return;
			}

			
			int prevSelectedIdx = m_componentPopupIndex;
			
			m_componentPopupIndex = EditorGUILayout.Popup("Component",prevSelectedIdx,m_componentList);
			
			if(prevSelectedIdx != m_componentPopupIndex)
			{
				action._component = m_components[m_componentPopupIndex];
			}
		}

		private void CreateComponentList()
		{
			GPActionEnableComponent action = (GPActionEnableComponent) TargetAction;

			if(m_componentParentObject == null)
				return;

			m_components = m_componentParentObject.GetComponents<Component>();

			m_componentList = new string[m_components.Length];

			m_componentPopupIndex = 0;

			for(int i = 0 ; i < m_components.Length ; i++)
			{
				if(m_components[i] == action._component)
					m_componentPopupIndex = i;

				m_componentList[i] = m_components[i].GetType().Name+" ("+m_components[i].GetInstanceID()+")";
			}
		}

	}
}
