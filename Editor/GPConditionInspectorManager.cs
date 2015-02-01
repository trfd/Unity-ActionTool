//
// GPConditionInspectorManager.cs
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
using System.Collections;
using System.Collections.Generic;

using Utils.Reflection;

namespace ActionTool
{
	public static class GPConditionInspectorManager 
	{
		#region Private Static Members
		
		private static Dictionary<System.Type,System.Type> s_gpconditionInspectorMap;
		
		#endregion
		
		#region Static Constructor
		
		static GPConditionInspectorManager()
		{
			s_gpconditionInspectorMap = new Dictionary<System.Type, System.Type>();
			
			System.Type[] types = TypeManager.Instance.ListTypesWithAttribute(typeof(GPConditionInspectorAttribute));
			
			foreach(System.Type type in types)
			{
				System.Object[] attrs = type.GetCustomAttributes(typeof(GPConditionInspectorAttribute),false);
				
				foreach(System.Object attr in attrs)
				{
					s_gpconditionInspectorMap.Add(((GPConditionInspectorAttribute)attr)._targetClass,type);
				}
			}
		}
		
		#endregion
		
		#region Accessors
		
		public static System.Type InspectorTypeForCondition(GPCondition condition)
		{
			System.Type conditionType = condition.GetType();
			System.Type inspectorType = null;
			
			while(inspectorType == null && conditionType != typeof(GPCondition))
			{
				if(s_gpconditionInspectorMap.TryGetValue(conditionType,out inspectorType))
				{
					return inspectorType;
				}
				
				conditionType = conditionType.BaseType;
			}

            return typeof(GPConditionDefaultInspector);
		}
		
		#endregion
		
	}
}

