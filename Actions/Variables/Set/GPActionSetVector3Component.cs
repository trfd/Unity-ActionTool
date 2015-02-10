﻿//
// GPActionSetVector3Component.cs
//
// Author:
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

namespace ActionTool
{
    [GPActionAlias("Variable/Vector3/Set Vector3 Component")]
	public class GPActionSetVector3Component : GPAction
    {
	
		public Vector3Component _component;
        public Vector3ValueProvider _variable;
        public FloatValueProvider _newValue;

        protected override void OnTrigger()
        {
			Vector3 v = _variable.GetValue();

			switch(_component)
			{
			case Vector3Component.X:
				v.x = _newValue.GetValue();
				break;
			case Vector3Component.Y:
				v.y = _newValue.GetValue();
				break;
			case Vector3Component.Z:
				v.z = _newValue.GetValue();
				break;
			}

			_variable.SetValue(v);
      		
			End();
        }
    }
}
