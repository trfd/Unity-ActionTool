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
using System.Collections;


namespace ActionTool
{	
	[GPActionAlias("Basic/Enable Component")]
	public class GPActionEnableComponent : GPAction
	{
		public enum ActivationKind
		{
			ACTIVATE,
			DEACTIVATE,
			SWITCH_ACTIVATION
		}
		
		#region Public Members
		
		public ActivationKind _kind;

		[HideInInspector]
		public bool _thisObject;

		[HideInInspector]
		public Component _component;
		
		#endregion
		
		#region GPAction Override
		
		/// <summary>
		/// Raised each time action is triggered
		/// </summary>
		protected override void OnTrigger()
		{
			if(_component == null)
			{
				Debug.LogWarning("Null Component can not be enabled");
				return;
			}

			System.Reflection.PropertyInfo enableProperty = _component.GetType().GetProperty("enabled");

			if(enableProperty == null)
			{
				Debug.LogWarning("Component of type: "+_component.GetType().FullName+" has not property 'enabled'");
				return;
			}

			switch(_kind)
			{
			case ActivationKind.ACTIVATE:
				enableProperty.SetValue(_component,true,null);
				break;
			case ActivationKind.DEACTIVATE:
				enableProperty.SetValue(_component,false,null);
				break;
			case ActivationKind.SWITCH_ACTIVATION:
				enableProperty.SetValue(_component,enableProperty.GetValue(_component,null),null);
				break;
			}

			End();
		}
		
		#endregion
	}
}