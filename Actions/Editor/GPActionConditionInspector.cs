//
// GPActionEnableComponent.cs
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
	[GPActionInspectorAttribute(typeof(GPActionIf))]
	public class GPActionConditionInspector : GPActionDefaultInspector
	{
		private int m_creationIndex;
		private GPConditionInspector m_conditionInsp;

		protected override void OnInspectorGUI()
		{
			GPActionIf actionCondition = (GPActionIf) TargetAction;

			if(actionCondition._condition == null)
				m_creationIndex = GPConditionInspector.CreateConditionField(ref actionCondition._condition,
																			m_creationIndex,
																			TargetAction.ParentHandler);

			if(m_conditionInsp == null && actionCondition._condition != null)
				m_conditionInsp = GPConditionInspector.CreateInspector(actionCondition._condition);

			if(m_conditionInsp != null)
				m_conditionInsp.DrawInspector();
		}
	}
}
