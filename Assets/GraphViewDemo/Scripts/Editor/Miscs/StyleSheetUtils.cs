using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public static class StyleSheetUtils
    {
        public static void AddStyleSheets(VisualElement visualElement, string ussFileName)
        {
            visualElement.styleSheets.Add(Resources.Load<StyleSheet>(ussFileName));
        }
    }
}
