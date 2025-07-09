using System;

namespace BehaviourTree
{
    public interface IHasPopup
    {
        MyEditorLayoutPopup Popup { get; set; }
        public string PopUpName { get; }
        public string[] PopUpOptions { get; }
        public int InitialSelectedIndex { get; }
        public void DrawPopup() => Popup.DrawPopup();
        public Action<int> SaveSelectedOption { get; }

        public void Init()
        {
            Popup = new(PopUpName, PopUpOptions, InitialSelectedIndex);
            Popup.SelectedIndex.OnValueChangedAfter += x => SaveSelectedOption(x);
        }
    }
}