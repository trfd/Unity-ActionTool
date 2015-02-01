//
// GPActionCompoundInspector.cs
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

/*
namespace ActionTool
{
	[GPActionInspectorAttribute(typeof(GPActionCompound))]
	public class GPActionCompoundInspector : GPActionDefaultInspector
	{
		#region Private Members

		/// <summary>
		/// The m_create new action.
		/// </summary>
		private bool m_createNewAction;

		/// <summary>
		/// The index of the m_action type selected.
		/// </summary>
		private int m_actionTypeSelectedIndex;

		#endregion

		public override void SetAction(GPAction action)
		{
			base.SetAction (action);

			GPActionCompound compoundAction = (GPActionCompound) TargetAction;

			m_childrenInspectors = new List<GPActionInspector>();

			for(int i=0 ; i< compoundAction.ActionCount() ; i++)
			{
				AddInspectorFor(compoundAction.ActionAtIndex(i));
			}
		}

		protected override void OnInspectorGUI()
		{
			GPActionCompound compoundAction = (GPActionCompound) TargetAction;

			if(compoundAction.ActionCount() != m_childrenInspectors.Count)
			{
				return;
			}

			string actionTypeName = GPActionManager.s_gpactionNameMap[TargetAction.GetType()];

			Rect startRect = EditorGUILayout.GetControlRect(true,EditorGUIUtility.singleLineHeight*1.3f);
			
			GUI.Box(startRect,compoundAction.EditionName+" ("+actionTypeName+")");

			for(int i=0 ; i< m_childrenInspectors.Count ;i++)
			{

				compoundAction.ActionAtIndex(i).EditionName = EditorGUILayout.TextField(
					"Name",compoundAction.ActionAtIndex(i).EditionName);

				m_childrenInspectors[i].DrawInspector();

				EditorGUILayout.Space();

				if(m_childrenInspectors[i].IsFoldedOut)
				{
					// Remove Button
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if(GUILayout.Button("Remove Action"))
					{
						RemoveActionAt(i);
						i--;
					}
					
					EditorGUILayout.EndHorizontal();
		
				}

				Rect rect = EditorGUILayout.GetControlRect();
				rect.height = 1f;
				EditorGUI.DrawRect(rect,Color.grey);
			}

			EditorGUILayout.Space();

			DisplayActionManagement();

			Rect endRect = EditorGUILayout.GetControlRect();
			endRect.height *= 1.3f;

			GUI.Box(endRect,"End "+compoundAction.EditionName+" ("+actionTypeName+")");
		}

		private void DisplayActionManagement()
		{
			if(m_createNewAction)
			{
				EditorGUILayout.BeginHorizontal();
				
				m_actionTypeSelectedIndex = EditorGUILayout.Popup("Action", m_actionTypeSelectedIndex, 
				                                                  GPActionManager.s_gpactionTypeNames);
				
				if (GUILayout.Button("Add"))
				{
					CreateAction();
					m_createNewAction = false;
				}
				
				if (GUILayout.Button("Cancel"))
					m_createNewAction = false;
				
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add New Action"))
					m_createNewAction = true;
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
*/
