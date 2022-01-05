using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using CSharpExtentions;

using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorGUIUtility;
using static UnityEditor.EditorStyles;
using static UnityEditor.EditorUtility;
using static UnityEditor.Selection;
using static UnityEngine.GUI;
using static UnityEngine.GUILayout;
using static UnityEditor.ImportAssetOptions;

namespace Universe.Editor
{
    public abstract class SignalEditorBase : UnityEditor.Editor
    {
        #region Unity API

        protected virtual void OnEnable()
        {
            _buttonDefaultBackgroundColor = backgroundColor;
        }

        public override void OnInspectorGUI()
        {
            var selection = activeObject as SignalBase;

            Space(_guiSpace);

            BeginVertical(helpBox);
            Label("Editor");
            if (selection is null)
            {
                EndVertical();
                return;
            }
                
            DrawToggle("Is Favorite", ref selection.m_isFavorite, Color.yellow);
            DrawToggle("Use Verbose Log On Emit", ref selection.m_useVerboseLog, Color.green);

            BeginHorizontal();
            if (Button("Clone")) Clone(selection);
            EndHorizontal();

            DrawEmitButton(selection);

            EndVertical();

            BeginVertical(helpBox);
            Label("Options");
            DrawToggle("Is Analytics", ref selection.m_isAnalytics, Color.green);
            DrawToggle("Is Achievement", ref selection.m_isAchievement, Color.green);
            DrawToggle("Use Once", ref selection.m_triggerOnce, Color.green);
            EndVertical();

            BeginVertical(helpBox);
            if (Button("Find Receptor In Scene")) RefreshReceptors();
            ShowReceptors();
            Space(_guiSpace);
            EndVertical();
        }

        #endregion


        #region Utilities

        protected virtual void DrawEmitButton(SignalBase target)
        {
            if (Button("Emit")) target.Emit();
        }

        private void DrawToggle(string label, ref bool target, Color enabledColor)
        {
            backgroundColor = target ? enabledColor : _buttonDefaultBackgroundColor;

            BeginHorizontal(helpBox);
            target = Toggle(target, label);
            EndHorizontal();

            backgroundColor = _buttonDefaultBackgroundColor;
        }


        private static void Clone(ScriptableObject objectToDuplicate)
        {
            const string title = "Save Cloned ScriptableObject";
            const string extension = "asset";
            var defaultName = $"Cloned {objectToDuplicate.name}.{extension}";
            const string message = "Enter a file name for the ScriptableObject.";
            var pathOfDuplicationTarget = AssetDatabase.GetAssetPath(objectToDuplicate);

            var path = SaveFilePanelInProject(title, defaultName, extension, message,
                pathOfDuplicationTarget);
            if (path == "") return;

            var asset = Instantiate(objectToDuplicate);

            CreateAsset(asset, path);
            SaveAssets();
            Refresh();
            ImportAsset(path, ForceUpdate);
            PingObject(asset);
        }

        private void RefreshReceptors()
        {
            _receptorLinked = new Dictionary<string, GameObject>();
            var _oldSelection = activeObject.name;

            var eventName = _oldSelection.Split('_').Last();
            var receptors = FindObjectsOfType<GameObject>().Where(obj => obj.name == eventName).ToArray();

            foreach (var receptor in receptors)
            {
                _receptorLinked.Add(receptor.GetScenePath(), receptor);
            }
        }

        private void ShowReceptors()
        {
            if (_receptorLinked.Count == 0) return;

            BeginVertical(helpBox);
            Label("Receptors In Scene");
            foreach (var listenerPair in _receptorLinked)
            {
                if (Button(listenerPair.Key))
                {
                    PingObject(listenerPair.Value);
                }
            }

            EndVertical();
        }

        #endregion


        #region Private And Protected

        private int _guiSpace = 5;
        private Color _buttonDefaultBackgroundColor;
        private Dictionary<string, GameObject> _receptorLinked = new Dictionary<string, GameObject>();

        #endregion
    }
}