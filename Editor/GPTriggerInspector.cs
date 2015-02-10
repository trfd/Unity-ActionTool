using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ActionTool
{
    [CustomEditor(typeof(GPTrigger))]
    public class GPTriggerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GPTrigger trigger = (GPTrigger)target;

            ObjectFilterDrawer.Display("Filter",trigger._filter);
        }
    }

}