//
// GPConditionManager.cs
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

using Utils.Reflection;

namespace ActionTool
{
    public static class GPConditionManager 
    {
	    #region Public Static Members

        public static System.Type[] s_gpconditionTypes;

        public static string[] s_gpconditionTypeNames;

		public static Dictionary<System.Type,string> s_gpconditionNameMap;

        #endregion

        #region Static Constructor

		static GPConditionManager()
		{
			s_gpconditionNameMap = new Dictionary<System.Type, string>();

            System.Type[] types = TypeManager.Instance.ListChildrenTypesOf(typeof(GPCondition),false);

            List<System.Type> visibleTypes = new List<System.Type>();
            List<string> visibleTypeNames = new List<string>();

            foreach (System.Type type in types)
            {
                if(type.GetCustomAttributes(typeof (GPConditionHideAttribute), false).Length == 0)
                {
                    visibleTypes.Add(type);

					System.Object[] attrs = type.GetCustomAttributes(typeof(GPConditionAliasAttribute),false);

					// Add Name to list
					// Note that if several 

					if(attrs.Length == 0)
					{
						visibleTypeNames.Add(type.Name);
						s_gpconditionNameMap.Add(type,type.Name);
					}
					else
					{
						string alias = ((GPConditionAliasAttribute) attrs[0])._aliasName;
						visibleTypeNames.Add(alias); 
						s_gpconditionNameMap.Add(type,alias);
					}
                }
            }

            s_gpconditionTypes = visibleTypes.ToArray();
            s_gpconditionTypeNames = visibleTypeNames.ToArray();
        }

        #endregion

    }
}

