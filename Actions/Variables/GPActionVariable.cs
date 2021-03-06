﻿//
// GPActionVariable.cs
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
    [GPActionHide]
    public class GPActionVariable : GPAction
    {
        #region Public Members

        public string _varName;

        #endregion

        public virtual System.Object GetValue()
        {
            return null;
        }

        public virtual void SetValue(System.Object value)
        {
        }

		#if UNITY_EDITOR
		
		public override void DrawWindowContent()
		{
			base.DrawWindowContent();

#if UNITY_EDITOR

			GUILayout.Label(_varName, UnityEditor.EditorStyles.miniLabel);
			object value = GetValue();
			GUILayout.Label((value == null) ? "Null" : value.ToString(), UnityEditor.EditorStyles.miniLabel);
#endif
		}
		
		#endif
    }
}
