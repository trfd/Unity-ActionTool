using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Utils;

namespace ActionTool
{
	[GPActionInspector(typeof(GPActionColorComponentAnimation))]
	public class GPActionColorComponentAnimationInspector : GPActionDefaultInspector
	{
		#region Private Members

        private ValueProviderEditor<Color> m_providerAEditor;

		#endregion

		protected override void OnInspectorGUI()
		{
            GPActionColorComponentAnimation action = (GPActionColorComponentAnimation)TargetAction;

            // Provider A

            EditorGUILayout.LabelField("Variable");

            EditorGUI.indentLevel++;

            if (m_providerAEditor == null)
            {
                m_providerAEditor = new ValueProviderEditor<Color>();
                m_providerAEditor.Provider = action._provider;
            }

            if (m_providerAEditor.Provider != action._provider)
                m_providerAEditor.Provider = action._provider;


            m_providerAEditor.Display();

            EditorGUI.indentLevel--;

            action._component = (ColorComponent)EditorGUILayout.EnumPopup("Component", action._component);
            action._duration = EditorGUILayout.FloatField("Duration", action._duration);
            action._curve = EditorGUILayout.CurveField("Curve", action._curve);

		}
	}
}
