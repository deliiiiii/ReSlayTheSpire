// using System;
// using UnityEditor;
//
// namespace BehaviourTree
// {
//     [Serializable]
//     public class MyEditorLayoutPopup
//     {
//         public readonly Observable<int> SelectedIndex = new (0);
//         string SelectedOption;
//         string name;
//         string[] options => GetOptions?.Invoke() ?? new[] { "No Fields Available" };
//         Func<string[]> GetOptions;
//
//         public MyEditorLayoutPopup(string name, Func<string[]> GetOptions, Func<int> GetSelectedIndex)
//         {
//             this.name = name;
//             this.GetOptions = GetOptions;
//             this.SelectedIndex.Value = GetSelectedIndex?.Invoke() ?? 0;
//             this.SelectedIndex.OnValueChangedAfter += OnOptionChanged;
//             OnOptionChanged(this.SelectedIndex.Value);
//         }
//         
//         public void DrawPopup()
//         {
//             if((options?.Length ?? 0) == 0 || SelectedIndex == null)
//             {
//                 return;
//             }
//             if (SelectedIndex >= options.Length)
//             {
//                 SelectedIndex.Value = 0;
//             }
//             SelectedIndex.Value = EditorGUILayout.Popup(name, SelectedIndex, options);
//         }
//         
//         void OnOptionChanged(int i)
//         {
//             SelectedOption = options.Length != 0 ? options[i] : string.Empty;
//             // MyDebug.Log($"OnOptionChanged {i} {selectedOption}");
//         }
//     }
// }