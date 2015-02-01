//
// GPActionLoopInspector.cs
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
	[GPActionInspectorAttribute(typeof(GPActionLoop))]
	[System.Serializable]
	public class GPActionLoopInspector : GPActionDefaultInspector
	{
		protected override void OnInspectorGUI()
		{
			GPActionLoop loopAction = (GPActionLoop) TargetAction;

			GPActionLoop.LoopType newType = (GPActionLoop.LoopType) EditorGUILayout.EnumPopup("Loop",loopAction._type);

			if(newType != loopAction._type &&  EditorApplication.isPlaying)
				Debug.LogWarning("Loop type can not be changed during runtime");
			else
				loopAction._type = newType;

			if(loopAction._type == GPActionLoop.LoopType.FIXED_COUNT)
			{
				loopAction._maxloopCount = EditorGUILayout.IntField("Max Loops",loopAction._maxloopCount);
			}
			else if(loopAction._type == GPActionLoop.LoopType.STOP_EVENT)
			{
				loopAction._stopEvent = EditorGUILayout.TextField("Stop Event",loopAction._stopEvent);
			}

			base.OnInspectorGUI();
		}
	}
}
