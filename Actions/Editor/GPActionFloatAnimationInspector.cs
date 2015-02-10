using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Utils;

namespace ActionTool
{
	[GPActionInspector(typeof(GPActionFloatAnimation))]
	public class GPActionFloatAnimationInspector : GPActionDefaultInspector
	{
		#region Private Members

        private ValueProviderEditor<float> m_providerAEditor;

		#endregion

		protected override void OnInspectorGUI()
		{
			GPActionFloatAnimation action = (GPActionFloatAnimation) TargetAction;

            // Provider A

            EditorGUILayout.LabelField("Variable");

            EditorGUI.indentLevel++;

            if (m_providerAEditor == null)
            {
                m_providerAEditor = new ValueProviderEditor<float>();
                m_providerAEditor.Provider = action._provider;
            }

            if (m_providerAEditor.Provider != action._provider)
                m_providerAEditor.Provider = action._provider;


            m_providerAEditor.Display();

            EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(SerialObject.FindProperty("_duration"));
			EditorGUILayout.PropertyField(SerialObject.FindProperty("_curve"));

			SerialObject.ApplyModifiedProperties();

		}
	}
}
