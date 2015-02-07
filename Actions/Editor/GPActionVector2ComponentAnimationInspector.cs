using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Utils;

namespace ActionTool
{
	[GPActionInspector(typeof(GPActionVector2ComponentAnimation))]
	public class GPActionVector2ComponentAnimationInspector : GPActionDefaultInspector
	{
		#region Private Members

        private ValueProviderEditor<Vector2> m_providerAEditor;

		#endregion

		protected override void OnInspectorGUI()
		{
            GPActionVector2ComponentAnimation action = (GPActionVector2ComponentAnimation)TargetAction;

            // Provider A

            EditorGUILayout.LabelField("Variable");

            EditorGUI.indentLevel++;

            if (m_providerAEditor == null)
            {
                m_providerAEditor = new ValueProviderEditor<Vector2>();
                m_providerAEditor.Provider = action._provider;
            }

            if (m_providerAEditor.Provider != action._provider)
                m_providerAEditor.Provider = action._provider;


            m_providerAEditor.Display();

            EditorGUI.indentLevel--;

            action._component = (Vector2Component)EditorGUILayout.EnumPopup("Component", action._component);
            action._duration = EditorGUILayout.FloatField("Duration", action._duration);
            action._curve = EditorGUILayout.CurveField("Curve", action._curve);

		}
	}
}
