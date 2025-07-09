using System;
using UnityEditor;

namespace BehaviourTree
{
    [Serializable]
    public class MyEditorLayoutPopup
    {
        public readonly Observable<int> SelectedIndex = new (0);
        public string SelectedOption;
        string name;
        string[] options;

        public MyEditorLayoutPopup(string name, string[] options, int selectedIndex)
        {
            this.name = name;
            this.options = options;
            this.SelectedIndex.Value = selectedIndex;
            this.SelectedIndex.OnValueChangedAfter += OnOptionChanged;
            OnOptionChanged(this.SelectedIndex.Value);
        }
        
        public void DrawPopup()
        {
            if((options?.Length ?? 0) == 0 || SelectedIndex == null)
            {
                options = new[] { "No Fields Available" }; 
                return;
            }
            if (SelectedIndex >= options.Length)
            {
                SelectedIndex.Value = 0;
            }
            SelectedIndex.Value = EditorGUILayout.Popup(name, SelectedIndex, options);
        }
        
        void OnOptionChanged(int i)
        {
            SelectedOption = options.Length != 0 ? options[i] : string.Empty;
            // MyDebug.Log($"OnOptionChanged {i} {selectedOption}");
        }
    }
}