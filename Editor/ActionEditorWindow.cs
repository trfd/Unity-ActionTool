//
// ActionEditorWindow.cs
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
using System.Linq;

namespace ActionTool
{
	public class ActionEditorWindow : EditorWindow 
	{
		#region Static Interface
		
		[MenuItem ("Window/Action Tool")]
		public static void ShowWindow() 
		{
			EditorWindow.GetWindow(typeof(ActionEditorWindow));
		}
	
		#endregion

		#region Private Members

		private float m_inspectorWidth = 250;

		private GPAction[] m_actions;
		private GPActionInspector[] m_actionInspectors;

		private GameObject m_selectedObject;
		private EventHandler[] m_handlersOfSelectedObject;
		private string[] m_handlerNamesOfSelectedObject;
		private EventHandler m_handler;

		private Vector2 m_blueprintScrollPosition;
		private Vector2 m_sidebarScrollPosition;

		private Rect m_blueprintScrollView;

		private int m_selectedBoxID = -1;
		private int m_layoutSelectedBoxID = -1;


		private ActionEditorNode m_selectedNode;

		private bool m_createNewAction = false;
		private UnityEngine.Object m_importPrefab;

		private bool m_displayImportPrefab;

		private int m_actionTypeSelectedIndex;


		private Texture2D m_backgroundBlueprintTex;
		private Texture2D m_backgroundLineInspectorTex;
		private Texture2D m_backgroundInspectorTex;

		#endregion

		#region Const

		private Color m_backColor          = new Color(0.18f, 0.18f, 0.18f);
		private Color m_borderLineColor    = new Color(0.13f, 0.13f, 0.13f);
		private Color m_inspectorBackColor = new Color(0.22f, 0.22f, 0.22f); 

		private Color m_actionNoneColor		  = new Color(1.0f , 1.0f , 1.0f);
		private Color m_actionRunningColor	  = new Color(0.0f , 0.62f, 1.0f);
		private Color m_actionTerminatedColor = new Color(0.36f, 1.0f , 0.0f);
		private Color m_actionFailureColor	  = new Color(1.0f , 0.14f, 0.0f);
		private Color m_eventHandlerColor	  = new Color(1.0f , 0.53f, 0.0f);

		#endregion

		#region Constructor

		public ActionEditorWindow()
		{
			title = "Action Tool";
		}

		#endregion

		#region Destructor

		void OnDestroy()
		{
			DestroyImmediate(m_backgroundBlueprintTex);
			DestroyImmediate(m_backgroundInspectorTex);
			DestroyImmediate(m_backgroundLineInspectorTex);
		}

		#endregion

		#region Update

		public void Update()
		{
			if(m_selectedNode != null)
				Repaint();
		}

		#endregion

		#region EventHandler Management

		/// <summary>
		/// Fetches the Event Handler that should be displayed
		/// </summary>
		protected virtual void FetchEventHandler()
		{
			if(Selection.activeObject == null && Selection.activeGameObject == null)
			{
				GUILayout.Label("No Event Handler Selected ");
				return;
			}
			
			if(Selection.activeGameObject != null)
			{
				if(m_selectedObject != Selection.activeGameObject)
				{
					m_handler = null;
				}

				m_selectedObject = Selection.activeGameObject;

				m_handlersOfSelectedObject = m_selectedObject.GetComponents<EventHandler>();

				if(m_handlersOfSelectedObject.Length == 0)
				{
					GUILayout.Label("No Event Handler Selected ");
					m_handlerNamesOfSelectedObject = null;
					m_handlersOfSelectedObject = null;
					return;
				}

				m_handlerNamesOfSelectedObject = new string[m_handlersOfSelectedObject.Length];

				for(int i=0 ; i<m_handlersOfSelectedObject.Length ; ++i)
				{
					if(m_handlersOfSelectedObject[i]._eventID == null)
						m_handlerNamesOfSelectedObject[i] = "No Event ("+m_handlersOfSelectedObject[i].GetInstanceID()+")";
					else
						m_handlerNamesOfSelectedObject[i] = "Handler:"+m_handlersOfSelectedObject[i]._eventID.Name;
				}
			}
		}

		/// <summary>
		/// Changes the handler currently displayed
		/// </summary>
		/// <param name="handler">Handler.</param>
		protected virtual void ChangeEventHandler(EventHandler handler)
		{
			Reset();
			
			m_handler = handler;

			if(m_handler != null)
				m_handler.CreateEventNode();
		}

		#endregion

		#region Action Management

		protected virtual void FetchActions()
		{
			GPAction[] actions = m_handler.GetGPActionObjectMapperOrCreate().GetAllActions(m_handler);

			// Check if arrays have diff length
			// if true recompute all inspectors

			if(m_actionInspectors == null || actions.Length != m_actions.Length)
			{
				m_actions = actions;
				CreateAllInspectors();

				return;
			}

			// else
			// recompute only inspectors for new actions

			for(int i = 0 ; i < actions.Length ; i++)
			{
				if(m_actions[i] == actions[i])
					continue;

				m_actions[i] = actions[i];
				CreateInspector(i);

			}

			if(m_selectedBoxID >= m_actions.Length)
				m_selectedBoxID = -1;
		}

		#endregion

		#region Dot Managenement

		#endregion

		#region Inspector Management

		protected virtual void CreateAllInspectors()
		{
			m_actionInspectors = new GPActionInspector[m_actions.Length];

			for(int i=0 ; i<m_actionInspectors.Length ; i++)
				CreateInspector(i);

		}

		protected virtual void CreateInspector(int idx)
		{
			if(idx < 0 || idx >= m_actionInspectors.Length)
				throw new System.IndexOutOfRangeException();

			System.Type inspectorType = GPActionInspectorManager.InspectorTypeForAction(m_actions[idx]);
			
			if(inspectorType == null)
				return;
			
			m_actionInspectors[idx] = (GPActionInspector) System.Activator.CreateInstance(inspectorType);
			m_actionInspectors[idx].TargetAction = m_actions[idx];
		}

		#endregion

		#region Selection Management
		
		private void CheckSelectedActionBox()
		{
			if(UnityEngine.Event.current.type == EventType.Layout && m_layoutSelectedBoxID != -1)
			{
				m_selectedBoxID = m_layoutSelectedBoxID;
				m_layoutSelectedBoxID = -1;
			}

            if(UnityEngine.Event.current.isMouse && UnityEngine.Event.current.type == EventType.MouseUp && m_selectedBoxID != -1)
            {
                EditorUtility.SetDirty(m_actions[m_selectedBoxID]);
                m_actionInspectors[m_selectedBoxID].SerialObject.ApplyModifiedProperties();
            }
               

			if(!UnityEngine.Event.current.isMouse || UnityEngine.Event.current.type != EventType.MouseDown)
				return;

			// If click in inspector skip change
			if(UnityEngine.Event.current.mousePosition.x >= position.width-m_inspectorWidth)
				return;
			
			int currSelectedIndex = -1;
			
			for(int i=0 ; i<m_actions.Length ; i++)
			{

				if(IsMouseIn(m_actions[i]._windowRect))
					currSelectedIndex = i;
			}

			ChangeSelectedModule(currSelectedIndex);
		}
		
		private void ChangeSelectedModule(int currSelectedIndex)
		{
			if(m_selectedBoxID != -1)
			{

			}
			                                
			m_layoutSelectedBoxID = currSelectedIndex;

            if(m_layoutSelectedBoxID != -1)
                CreateInspector(m_layoutSelectedBoxID);
		}

		private void CheckSelectedNode()
		{
			if(!UnityEngine.Event.current.isMouse || UnityEngine.Event.current.type != EventType.MouseDown)
				return;

			ActionEditorNode newNode = null;

			if(IsMouseOverNode(m_handler.EventNode))
			{
				newNode = m_handler.EventNode;
			}

			foreach(GPAction action in m_actions)
			{
				if(IsMouseOverNode(action._leftNode))
				{
					newNode = action._leftNode;
				}

				foreach(ActionEditorNode node in action._rightNodes)
				{
					if(IsMouseOverNode(node))
					{
						newNode = node;
					}
				}
			}

			ChangeSelectedNode(newNode);
		}

		private void ChangeSelectedNode(ActionEditorNode node)
		{
			if(m_selectedNode != null)
			{
				if(node != null)
				{
					if(node == m_handler.EventNode)
						CreateHandlerActionConnection(m_selectedNode);
					else if(m_selectedNode == m_handler.EventNode)
						CreateHandlerActionConnection(node);
					else
						CreateConnection(node,m_selectedNode);
				}

				m_selectedNode._selected = false;
				m_selectedNode = null;

				Repaint();

				return;
			}
			// else

			m_selectedNode = node;

			if(m_selectedNode != null)
				m_selectedNode._selected = true;

			Repaint();
		}

		#endregion

		#region Connection Management

		private void CreateConnection(ActionEditorNode node1, ActionEditorNode node2)
		{
			if(node1._owner == node2._owner || node1._owner == null || node2._owner == null)
				return;

			bool node1IsLeft = (node1._owner is GPAction && ((GPAction) node1._owner)._leftNode == node1);
			bool node2IsLeft = (node2._owner is GPAction && ((GPAction) node2._owner)._leftNode == node2);

			if((node1IsLeft && node2IsLeft) || (!node1IsLeft && !node2IsLeft))
				return;

			GPAction parent;
			GPAction child;

			if(node2IsLeft)
			{
				child  = (GPAction) node2._owner;
				parent = (GPAction) node1._owner;
			}
			else
			{
				child  = (GPAction) node1._owner; 
				parent = (GPAction) node2._owner;
			}

			if(!(parent is IActionOwner))
				return;

			if(child._leftNode._connection != null)
			{
				((IActionOwner)child._leftNode._connection._nodeParent._owner).Disconnect(child);
			}

			((IActionOwner) parent).Connect(child);
		}

		protected virtual void CreateHandlerActionConnection(ActionEditorNode node)
		{
			if(m_handler == null || node == null || !(node._owner is GPAction))
				return;

			m_handler.Action = (GPAction) node._owner;
		}

		protected virtual void DisplayAllConnections()
		{
			foreach(GPAction action in m_actions)
			{
				if(action._leftNode._connection == null)
					continue;

				if(action._leftNode._connection.IsValid)
					DisplayConnection(action._leftNode._connection);
			}
		}

		protected virtual void DisplayConnection(ActionEditorConnection connection)
		{
			Vector2 scrollOffset = new Vector2(m_blueprintScrollPosition.x + m_blueprintScrollView.position.x,
			                                   m_blueprintScrollPosition.y + m_blueprintScrollView.position.y);

			Vector2 inPos  = connection._nodeParent._center + connection._nodeParent._owner.WindowRect.position - scrollOffset;
			Vector2 outPos = connection._nodeChild._center  + connection._nodeChild._owner.WindowRect.position  - scrollOffset;
			
			Handles.DrawBezier(inPos, outPos,
			                   inPos  + 30 * Vector2.right,
			                   outPos - 30 * Vector2.right,
			                   Color.white,null,3f);
		}

		protected virtual void DisplayDrawingConnection()
		{
			if(m_selectedNode == null)
				return;

			if(m_selectedNode._owner == null)
			{
				//Debug.LogError("Node: "+m_selectedNode+" has no action");
				return;
			}

			Vector2 scrollOffset = new Vector2(m_blueprintScrollPosition.x + m_blueprintScrollView.position.x,
			                                   m_blueprintScrollPosition.y + m_blueprintScrollView.position.y);

			float sign = (m_selectedNode._owner is GPAction && 
			              ((GPAction) m_selectedNode._owner)._leftNode == m_selectedNode) ? -1f : 1f;

			Vector2 inPos = m_selectedNode._center + m_selectedNode._owner.WindowRect.position - scrollOffset;

			Vector2 outPos = UnityEngine.Event.current.mousePosition;

			Handles.DrawBezier(inPos, outPos,
			                   inPos  + 30 * sign * Vector2.right,
			                   outPos - 30 * sign * Vector2.right,
			                   Color.grey,null,3f);
		}

		#endregion
		
		public virtual void Reset()
		{
		
		}
		
		/// <summary>
		/// Raises the GUI event.
		/// </summary>
		private void OnGUI() 
		{ 
			FetchEventHandler();

			if(m_handler!= null && m_handler.EventNode == null)
				m_handler.CreateEventNode();

			EditorGUIUtility.labelWidth = 80;

			if(m_handler != null)
			{
				FetchActions();

				CheckSelectedActionBox();

				CheckSelectedNode();

				DrawBackground();

				DisplayAllConnections();

				if(m_selectedNode != null)
					DisplayDrawingConnection();

			}

			DisplaySidebar();

			if(m_handler != null)
				DisplayBlueprint();
			
			EditorGUIUtility.labelWidth = 0;
		}

		protected virtual void DisplayAction(int id)
		{
			if(id >= m_actions.Length || id < 0)
				return;

			if(m_actionInspectors == null)
				CreateAllInspectors();
			else if(m_actionInspectors[id] == null)
				CreateInspector(id);

			if(m_actions[id]._leftNode._owner != null)
				m_actions[id]._leftNode.Draw();

			for(int i =0 ; i< m_actions[id]._rightNodes.Count ; i++)
			{
				m_actions[id]._rightNodes[i].Draw();
			}
		
			GUILayout.BeginArea(new Rect(20,15,74,44));

			m_actions[id].DrawWindowContent();

			GUILayout.EndArea();

			GUI.DragWindow(new Rect(0,0,10000,20));
		}

		protected virtual void DisplayHandler(int id)
		{
			if(m_handler._eventID != null)
				GUILayout.Label(m_handler._eventID.Name);

			m_handler.EventNode.Draw();

			GUI.DragWindow(new Rect(0,0,10000,20));
		}

		#region Background

		protected virtual void DrawBackground()
		{
			if(m_backgroundBlueprintTex 	 == null ||
			   m_backgroundLineInspectorTex  == null ||
			   m_backgroundInspectorTex 	 == null)
				CreateTextures();

			float xInspector = position.width-m_inspectorWidth;
			
			DrawQuad(new Rect(0, 0, xInspector, position.height),m_backgroundBlueprintTex);
		}

		#endregion

		#region Sidebar

		protected virtual void DisplaySidebar()
		{	
			if(m_backgroundBlueprintTex 	 == null ||
			   m_backgroundLineInspectorTex  == null ||
			   m_backgroundInspectorTex 	 == null)
				CreateTextures();

			float xInspector = position.width-m_inspectorWidth;

			DrawQuad(new Rect(xInspector-1, 0, 10              , position.height),m_backgroundLineInspectorTex);
			DrawQuad(new Rect(xInspector  , 0, m_inspectorWidth, position.height),m_backgroundInspectorTex);

			GUILayout.BeginArea(new Rect(position.width-m_inspectorWidth+5,0,
			                             m_inspectorWidth-10,position.height));

			m_sidebarScrollPosition = GUILayout.BeginScrollView(m_sidebarScrollPosition,
			                                                    GUILayout.Width(m_inspectorWidth-10),
			                                                    GUILayout.Height(position.height));

			// Header
			
			DisplaySidebarHeader();

			Rect rect = EditorGUILayout.GetControlRect();
			rect.height = 1f;
			EditorGUI.DrawRect(rect,new Color(0.282f,0.282f,0.282f));

			// Inspector

			DisplaySidebarInspector();

			// Footer

			rect = EditorGUILayout.GetControlRect();
			rect.height = 1f;
			
			EditorGUI.DrawRect(rect,new Color(0.282f,0.282f,0.282f));
			
			DisplaySidebarFooter();

			GUILayout.EndScrollView();
			
			GUILayout.EndArea();
		}

		protected virtual void DisplaySidebarHeader()
		{
			DisplayEventHandlerPopup();

			Rect rect = EditorGUILayout.GetControlRect();
			rect.height = 1f;
			EditorGUI.DrawRect(rect,new Color(0.282f,0.282f,0.282f));

			DisplayActionCreationField();
		}

		protected virtual void DisplayEventHandlerPopup()
		{
			if(m_handlersOfSelectedObject == null)
				return;

			int idx = 0;

			if(m_handler != null)
				idx = System.Array.IndexOf(m_handlersOfSelectedObject,m_handler);

			EditorGUILayout.LabelField("Event Handler", EditorStyles.boldLabel);

			int newIdx = EditorGUILayout.Popup(idx, m_handlerNamesOfSelectedObject);

			if(newIdx >= m_handlersOfSelectedObject.Length)
				return;

			if(newIdx != idx || m_handler == null)
				ChangeEventHandler(m_handlersOfSelectedObject[newIdx]);
		}

		protected virtual void DisplaySidebarFooter()
		{
			if(m_selectedBoxID != -1 && m_actions[m_selectedBoxID] is GPActionRelatedObject)
				GUI.color = Color.gray;

			if(GUILayout.Button("Remove Action") && m_selectedBoxID != -1)
					RemoveSelectedAction(); 

			GUI.color = Color.white;

			EditorGUILayout.Space();

			DisplayImportField();
			DisplayExportField();
		}

		protected virtual void DisplaySidebarInspector()
		{	
			if(m_selectedBoxID == -1 || m_handler == null)
				return;

			string name = GetDisplayName(m_actions[m_selectedBoxID].GetType());
			
			name = name.Split('/').Last();

			GUILayout.Label(name, EditorStyles.boldLabel);

			EditorGUILayout.Space();

			m_handler.GetGPActionObjectMapperOrCreate().CheckPrefabConnection(m_handler);

			if(m_handler.PrefabAction != null)
			{
				GUILayout.BeginHorizontal();

				GUILayout.Label("Prefab",EditorStyles.toolbarButton);

				if(GUILayout.Button("Revert",EditorStyles.toolbarButton))
				{
					m_handler.GetGPActionObjectMapperOrCreate().RevertGPActionObjectHolderToPrefab(m_handler);
				}

				if(GUILayout.Button("Apply",EditorStyles.toolbarButton))
				{
					m_handler.GetGPActionObjectMapperOrCreate().ApplyGPActionObjectHolderToPrefab(m_handler);
				}

				if(GUILayout.Button("Break",EditorStyles.toolbarButton))
				{
					m_handler.GetGPActionObjectMapperOrCreate().BreakGPActionObjectHolderFromPrefab(m_handler);
				}

				GUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();
		
			m_actionInspectors[m_selectedBoxID].DrawInspector();
		
		}

		private void DisplayActionCreationField()
		{
			if(m_createNewAction)
			{
				m_actionTypeSelectedIndex = EditorGUILayout.Popup("Action", m_actionTypeSelectedIndex, 
				                                                  GPActionManager.s_gpactionTypeNames);

				EditorGUILayout.BeginHorizontal();
				
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

		private void CreateAction()
		{
			if (m_actionTypeSelectedIndex >= GPActionManager.s_gpactionTypes.Length)
				throw new System.Exception("Out of bound index");
			
			System.Type actionType = GPActionManager.s_gpactionTypes[m_actionTypeSelectedIndex];
			
			m_handler.AddAction(actionType);
		}

		#endregion

		#region BluePrint

		protected virtual void DisplayBlueprint()
		{
			ComputeScrollView();

			float xInspector = position.width-m_inspectorWidth;

			Handles.BeginGUI();

			GUILayout.BeginArea(new Rect(0,0,xInspector,position.height));

			m_blueprintScrollPosition = GUI.BeginScrollView(new Rect(0,0,xInspector,position.height),
			                                                m_blueprintScrollPosition,
			                                                m_blueprintScrollView);

			BeginWindows();

			GUI.backgroundColor = m_eventHandlerColor;

			m_handler._windowRect = GUI.Window(-1, m_handler._windowRect, DisplayHandler, "Event");

			for(int i=0 ; i<m_actions.Length ; i++)
			{
				GPAction box = m_actions[i];

				SetBoxColor(box);

				string name = GetDisplayName(box.GetType());

				Rect newRect = GUI.Window(i, box._windowRect, DisplayAction, name);

				if(box._windowRect != newRect)
				{
					box._windowRect = newRect;
					m_actionInspectors[i].SerialObject.FindProperty("_windowRect").rectValue = newRect;
				}

			}

			EndWindows();

			GUI.EndScrollView();

			GUILayout.EndArea();

			Handles.EndGUI();
		}

		protected virtual void ComputeScrollView()
		{
			float minX, minY;
			float maxX, maxY;

			minX = minY = 0;
			maxX = position.width-m_inspectorWidth;
			maxY = position.height;

			for(int i=0 ; i<m_actions.Length ; i++)
			{
				minX = Mathf.Min(minX, m_actions[i]._windowRect.xMin);
				minY = Mathf.Min(minY, m_actions[i]._windowRect.yMin);

				maxX = Mathf.Max(maxX, m_actions[i]._windowRect.xMax);
				maxY = Mathf.Max(maxY, m_actions[i]._windowRect.yMax);
			}

			minX = Mathf.Min(minX, m_handler._windowRect.xMin);
			minY = Mathf.Min(minY, m_handler._windowRect.yMin);
			
			maxX = Mathf.Max(maxX, m_handler._windowRect.xMax);
			maxY = Mathf.Max(maxY, m_handler._windowRect.yMax);

			m_blueprintScrollView = new Rect(minX, minY, maxX - minX, maxY - minY);
		}

		protected virtual void SetBoxColor(GPAction box)
		{
			Color c;

			switch(box.State)
			{
			case GPAction.ActionState.NONE:
				c = m_actionNoneColor;
				break;
			case GPAction.ActionState.RUNNNING:
				c = m_actionRunningColor;
				break;
			case GPAction.ActionState.TERMINATED:
				c = m_actionTerminatedColor;
				break;
			case GPAction.ActionState.FAILURE:
				c = m_actionFailureColor;
				break;
			default:
				c = Color.gray;
				break;
			}

			if(m_selectedBoxID >= 0 && 
			   m_selectedBoxID <m_actions.Length &&
			   m_actions[m_selectedBoxID] != box)
				c *= 0.5f;

			c.a = 1.0f;

			GUI.backgroundColor = c;
		}

		#endregion

		public void RemoveSelectedAction()
		{
			if(m_selectedBoxID == -1)
				return;

			GPAction action = m_actions[m_selectedBoxID];

			if(action is GPActionRelatedObject)
				return;

			if(action._leftNode._connection != null)
			{
				if(m_actions[m_selectedBoxID]._leftNode._connection._nodeParent == null)
				{
					m_actions[m_selectedBoxID]._leftNode._connection._nodeChild._connection = null;
					m_actions[m_selectedBoxID]._leftNode._connection = null;
				}
				else if(m_actions[m_selectedBoxID]._leftNode._connection._nodeParent._owner != null)
					((IActionOwner)m_actions[m_selectedBoxID]._leftNode._connection._nodeParent._owner)
						.Disconnect(m_actions[m_selectedBoxID]);
			}

			if(action is IActionOwner)
				((IActionOwner) action).DisconnectAll();

			DestroyImmediate(m_actions[m_selectedBoxID]);

			m_selectedBoxID = -1;
			m_layoutSelectedBoxID = -1;

			Repaint();
		}

		private void OnSelectionChange()
		{
			Repaint();
		}

		private void DisplayImportField()
		{
			if(!m_displayImportPrefab)
			{
				if(GUILayout.Button("Import Action"))
				{
					m_displayImportPrefab = true;
				}
			}
			else
			{
				m_importPrefab = EditorGUILayout.ObjectField("Prefab",m_importPrefab,typeof(GameObject),false);
				
				EditorGUILayout.BeginHorizontal();
				
				if(GUILayout.Button("Import"))
				{
					ImportActionPrefab();
					m_displayImportPrefab = false;
				}
				else if(GUILayout.Button("Cancel"))
					m_displayImportPrefab = false;
				
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DisplayExportField()
		{
			if(GUILayout.Button("Export Action"))
			{
				ExportActionPrefab();
			}
		}

		private void ExportActionPrefab()
		{
			if(EditorApplication.isPlaying)
			{
				Debug.LogError("Can not export in play mode");
				return;
			}
			
			m_handler.GetGPActionObjectMapperOrCreate().ExportGPActionObjectHolderPrefab(m_handler);
		}
		
		private void ImportActionPrefab()
		{
			if(EditorApplication.isPlaying)
			{
				Debug.LogError("Can not import in play mode");
				return;
			}

			m_handler.GetGPActionObjectMapperOrCreate().ImportGPActionObjectHolderPrefab(m_handler,m_importPrefab);
		}

		#region Utils

		private string GetDisplayName(System.Type type)
		{
			// Get display name
			
			string name;
			
			if(!GPActionManager.s_gpactionNameMap.TryGetValue(type,out name))
			{
				object[] attr = type.GetCustomAttributes(typeof(GPActionAliasAttribute),false);
				
				if(attr.Length > 0)
					name = ((GPActionAliasAttribute) attr[attr.Length-1])._aliasName;
				else
					name = type.Name;
			}
			
			return name.Split('/').Last();
		}

		private void CreateTextures()
		{
			m_backgroundInspectorTex = new Texture2D(1, 1);
			m_backgroundInspectorTex.SetPixel(0,0,m_inspectorBackColor);
			m_backgroundInspectorTex.Apply();
			
			m_backgroundLineInspectorTex = new Texture2D(1, 1);
			m_backgroundLineInspectorTex.SetPixel(0,0,m_borderLineColor);
			m_backgroundLineInspectorTex.Apply();
			
			m_backgroundBlueprintTex = new Texture2D(10, 10);
			m_backgroundBlueprintTex.wrapMode = TextureWrapMode.Repeat;

			for(int x=0; x < 10 ; x++)
			{
				for(int y=0; y < 10 ; y++)
				{
					if(x%5 == 0 && y%5 ==0)
						m_backgroundBlueprintTex.SetPixel(x,y,m_inspectorBackColor);
					else
						m_backgroundBlueprintTex.SetPixel(x,y,m_backColor);
				}
			}

			m_backgroundBlueprintTex.Apply();
		}

		/// <summary>
		/// Draws a rectangle of specified size and color in the GUI.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="color">Color.</param>
		private void DrawQuad(Rect position, Texture2D tex)
		{
			GUI.skin.box.normal.background = tex;

			//GUI.Box(position, GUIContent.none);

			GUI.DrawTextureWithTexCoords(position, tex, 
			                             new Rect(0, 0, position.width / tex.width, position.height / tex.height));
		}
		
		private bool IsMouseIn(Rect rect)
		{	
			Vector2 scrollOffset = new Vector2(m_blueprintScrollPosition.x + m_blueprintScrollView.position.x,
			                                   m_blueprintScrollPosition.y + m_blueprintScrollView.position.y);

			Vector2 screenPos = UnityEngine.Event.current.mousePosition + scrollOffset;
			
			return rect.Contains(screenPos);
		}

		private bool IsMouseOverNode(ActionEditorNode node)
		{
			if(node._owner == null) 
				return false;

			return IsMouseIn(new Rect(node._owner.WindowRect.x + node._center.x - 5, 
			                          node._owner.WindowRect.y + node._center.y - 5, 10,10));
		}

		#endregion
	}


}