using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(ResourceObject))]
public class ResourceObjectDrawer : PropertyDrawer
{
    /*
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var resourceType = new PropertyField(property.FindPropertyRelative("ResourceType"));
        var resourceCount = new PropertyField(property.FindPropertyRelative("Count"));

        // Add fields to the container.
        container.Add(resourceType);
        container.Add(resourceCount);

        return container;
    }*/

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var resourceIcon = new Rect(position.x, position.y, position.height, position.height);
        var resourceType = new Rect(position.x + position.height + 5, position.y, 200, position.height);
        var resourceCount = new Rect(position.x + position.height + 210, position.y, 45, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        var res = property.FindPropertyRelative("ResourceType").objectReferenceValue as ResourceType;
        if (res != null)
        {
            var sprite = res.Icon;
            if (sprite != null)
            {
                Rect c = sprite.rect;
                float spriteW = c.width;
                float spriteH = c.height;
                if (Event.current.type == EventType.Repaint)
                {
                    var tex = sprite.texture;
                    c.xMin /= tex.width;
                    c.xMax /= tex.width;
                    c.yMin /= tex.height;
                    c.yMax /= tex.height;
                    GUI.DrawTextureWithTexCoords(resourceIcon, tex, c);
                }
            }
        }
        EditorGUI.PropertyField(resourceType, property.FindPropertyRelative("ResourceType"), GUIContent.none);
        EditorGUI.PropertyField(resourceCount, property.FindPropertyRelative("Count"), GUIContent.none);
        //EditorGUI.PropertyField(resourceName, property.stringValue(), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
