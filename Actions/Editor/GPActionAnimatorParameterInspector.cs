using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ActionTool
{
	[GPActionInspector(typeof(GPActionAnimatorParameter))]
	public class GPActionAnimatorParameterInspector : GPActionDefaultInspector
	{
		protected override void OnInspectorGUI ()
		{
			GPActionAnimatorParameter animAction = (GPActionAnimatorParameter) TargetAction;
			
			animAction._parameter = EditorGUILayout.TextField("Parameter",animAction._parameter);
			
			animAction._kind = (GPActionAnimatorParameter.Kind) EditorGUILayout.EnumPopup("Parameter Kind", animAction._kind);
			
			switch(animAction._kind)
			{
			case GPActionAnimatorParameter.Kind.TRIGGER:
				break;
			case GPActionAnimatorParameter.Kind.BOOL:
				animAction._boolValue = EditorGUILayout.Toggle("Value",animAction._boolValue);
				break;
			case GPActionAnimatorParameter.Kind.INTEGER:
				animAction._intValue = EditorGUILayout.IntField("Value",animAction._intValue);
				break;
			case GPActionAnimatorParameter.Kind.FLOAT:
				animAction._floatValue = EditorGUILayout.FloatField("Value",animAction._floatValue);
				break;
			}
		}
	}
}
