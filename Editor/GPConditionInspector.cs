//
// GPConditionInspector.cs
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
	public class GPConditionInspector 
	{
		#region Private Members

		private GPCondition m_condition;

        protected SerializedObject m_serialObject;

		#endregion

		#region Properties

		public GPCondition Condition
		{
			get{ return m_condition;  } 
			set
            {
                if (m_condition != value)
                    m_serialObject = new SerializedObject(value);
                m_condition = value; 
            }
		}

        public SerializedObject SerialObject
        {
            get { return m_serialObject; }
        }

		#endregion

		public void DrawInspector()
		{
			OnInspectorGUI();
		}
		
		protected virtual void OnInspectorGUI()
		{}

		#region Static Interface

		public static int CreateConditionField(ref GPCondition condition, int selectedIndex, EventHandler handler)
		{
			if(condition != null)
				return selectedIndex;

			EditorGUILayout.BeginHorizontal();

			int idx = EditorGUILayout.Popup(selectedIndex,GPConditionManager.s_gpconditionTypeNames);

			if(GUILayout.Button("Add"))
				condition = AddCondition(GPConditionManager.s_gpconditionTypes[idx],handler);

			EditorGUILayout.EndHorizontal();

			return idx;
		}

		public static GPCondition AddCondition(System.Type type, EventHandler handler)
		{
            GPCondition condition = (GPCondition)handler.GetGPActionObjectMapperOrCreate().AddCondition(type, handler);

			condition.SetHandler(handler);

			return condition;
		}

		public static GPConditionInspector CreateInspector(GPCondition condition)
		{
			GPConditionInspector insp = (GPConditionInspector) System.Activator.CreateInstance(
					GPConditionInspectorManager.InspectorTypeForCondition(condition));
			insp.Condition = condition;

			return insp;
		}

		#endregion
	}

    public class GPConditionDefaultInspector : GPConditionInspector
    {
        protected override void OnInspectorGUI()
        {
            SerializedProperty property = m_serialObject.GetIterator();

            bool remainingProperties = property.NextVisible(true);

            // Hide Script
            if (remainingProperties)
                remainingProperties = property.NextVisible(true);

            Stack<SerializedProperty> endParentStack = new Stack<SerializedProperty>();

            while (remainingProperties)
            {
                while (endParentStack.Count > 0 && SerializedProperty.EqualContents(endParentStack.Peek(), property))
                {
                    endParentStack.Pop();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(property);

                if (property.hasVisibleChildren)
                {
                    SerializedProperty endProperty = property.GetEndProperty();

                    if (!property.isExpanded)
                    {
                        if (endProperty.propertyPath == "")
                        {
                            remainingProperties = false;
                        }

                        property = endProperty;

                        continue;
                    }
                    else
                    {
                        endParentStack.Push(endProperty);
                        EditorGUI.indentLevel++;
                    }
                }

                remainingProperties = property.NextVisible(true);
            }

            if (m_serialObject.targetObject != null)
                m_serialObject.ApplyModifiedProperties();
        }

    }

}
