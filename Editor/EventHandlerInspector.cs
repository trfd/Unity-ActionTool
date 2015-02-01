using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

using ActionTool;
using Utils.Reflection;
using EventHandler = ActionTool.EventHandler;


[CustomEditor(typeof(EventHandler))]
public class EventHandlerInspector : Editor
{
    #region Private Members

    /// <summary>
    /// The Index in GPAction popup currently selected
    /// </summary>
    private int m_actionTypeSelectedIndex = 0;

    /// <summary>
    /// Holds whether the button "Create Action" button has been pushed
    /// </summary>
    private bool m_createNewAction = false;

	private GPActionInspector m_actionInspector;

    #endregion

    #region Inspector 

    public override void OnInspectorGUI()
    {
        EventHandler handler = (EventHandler)target;

        // Display Default MonoBehaviour editor

        base.OnInspectorGUI();

        // Display Handler Kind popup

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("State: "+handler.State.ToString());

		if(EditorApplication.isPlaying && GUILayout.Button("Debug Trigger"))
		{
			handler.EventTrigger(new GPEvent{EventID= handler._eventID});
		}
    }

	private void DisplayActionManagementField()
	{
		EventHandler handler = (EventHandler)target;

		if(handler.Action == null)
			DisplayActionCreationField();
		else
			DisplayActionDeleteField();
	}

    private void DisplayActionCreationField()
    {
        if(m_createNewAction)
        {
            EditorGUILayout.BeginHorizontal();

            m_actionTypeSelectedIndex = EditorGUILayout.Popup("Action", m_actionTypeSelectedIndex, 
			                                                  GPActionManager.s_gpactionTypeNames);

            if (GUILayout.Button("Create"))
            {
               CreateAction();
                m_createNewAction = false;
            }
            
            if (GUILayout.Button("Cancel"))
                m_createNewAction = false;
            
            EditorGUILayout.EndHorizontal();

           
        }
        else if (GUILayout.Button("Create Action"))
            m_createNewAction = true;
    }

	private void DisplayActionDeleteField()
	{
		if(GUILayout.Button("Delete action"))
		{
			DeleteAction();
		}
	}



	private void CreateAction()
	{
		if (m_actionTypeSelectedIndex >= GPActionManager.s_gpactionTypes.Length)
			throw new Exception("Out of bound index");

		EventHandler handler = (EventHandler)target;

		System.Type actionType = GPActionManager.s_gpactionTypes[m_actionTypeSelectedIndex];

        handler.Action = handler.AddAction(actionType);
	}

	private void DeleteAction()
	{
		EventHandler handler = (EventHandler)target;
		
		if(handler.Action == null)
			return;

		if(EditorUtility.DisplayDialog("Confirm Delete",
		   	                           "Are you sure you want to delete this action ? " +
		                               "This can not be undone!",
		                               "Confirm","Cancel"))
		{
			handler.GetGPActionObjectMapperOrCreate().ResetGPActionObjectHolder(handler);
		}
	}

    

	private void CreateActionInspector(EventHandler handler)
	{
		System.Type inspectorType = GPActionInspectorManager.InspectorTypeForAction(handler.Action);

		if(inspectorType == null)
			return;

		m_actionInspector = (GPActionInspector) Activator.CreateInstance(inspectorType);
		m_actionInspector.TargetAction = handler.Action;
	}

    #endregion
}
