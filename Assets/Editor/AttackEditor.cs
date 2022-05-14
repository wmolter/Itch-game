using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Itch.Behavior;

namespace Itch.EditorScripts {
    //[CustomPropertyDrawer(typeof(List<AttackEffect>))]
    public class AttackEditor : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            // Create property container element.
            var container = new VisualElement();

            container.Add(new TextField("Test"));

            return container;
        }
    }
}