using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Utils;

namespace ActionTool
{
	[GPActionInspector(typeof(GPActionVector3ComponentAnimation))]
	public class GPActionVector3ComponentAnimationInspector : GPActionDefaultInspector
	{
		#region Private Members

        private ValueProviderEditor<Vector3> m_providerAEditor;

		#endregion

		protected override void OnInspectorGUI()
		{
            GPActionVector3ComponentAnimation action = (GPActionVector3ComponentAnimation)TargetAction;

            // Provider A

            EditorGUILayout.LabelField("Variable");

            EditorGUI.indentLevel++;

            if (m_providerAEditor == null)
            {
                m_providerAEditor = new ValueProviderEditor<Vector3>();
                m_providerAEditor.Provider = action._provider;
            }

            if (m_providerAEditor.Provider != action._provider)
                m_providerAEditor.Provider = action._provider;


            m_providerAEditor.Display();

            EditorGUI.indentLevel--;

            action._component = (Vector3Component)EditorGUILayout.EnumPopup("Component", action._component);
            action._duration = EditorGUILayout.FloatField("Duration", action._duration);
            action._curve = EditorGUILayout.CurveField("Curve", action._curve);

		}
	}
}
