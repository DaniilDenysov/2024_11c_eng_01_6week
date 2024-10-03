using System;
using UnityEngine;
using UnityEditor;

namespace CustomTools
{

    [CustomEditor(typeof(MonoBehaviour), false)]
    public class Vector2DraggablePointDrawer : Editor
    {

        /*  readonly GUIStyle style = new GUIStyle();

          void OnEnable()
          {
              style.fontStyle = FontStyle.Bold;
              style.normal.textColor = Color.white;
          }

          public void OnSceneGUI()
          {
              serializedObject.Update();

              var property = serializedObject.GetIterator();
              while (property.NextVisible(true))
              {
                  if (property.propertyType == SerializedPropertyType.Generic)
                  {
                      var field = serializedObject.targetObject.GetType().GetField(property.name);
                      if (field == null)
                      {
                          continue;
                      }
                      var draggablePoints = field.GetCustomAttributes(typeof(Vector2DraggablePoint), false);
                      if (draggablePoints.Length > 0)
                      {
                          if (property.isArray)
                          {
                              for (int i = 0; i < property.arraySize; i++)
                              {
                                  var element = property.GetArrayElementAtIndex(i);
                                  if (property.propertyType != SerializedPropertyType.Vector2) break;
                                  DrawHandleForVector2Property(element);
                              }
                          }
                      }
                  }
                  else if (property.propertyType == SerializedPropertyType.Vector2)
                  {
                      //Debug.Log("Is not array");
                      DrawHandleForVector2Property(property);
                  }
              }

              serializedObject.ApplyModifiedProperties();
          }

          private void DrawHandleForVector2Property(SerializedProperty property)
          {
              Handles.Label(property.vector2Value, property.displayName);
              property.vector2Value = Handles.PositionHandle(property.vector2Value, Quaternion.identity);
          }*/
    }
}

