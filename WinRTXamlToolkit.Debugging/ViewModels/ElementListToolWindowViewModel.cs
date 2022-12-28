using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace WinRTXamlToolkit.Debugging.ViewModels
{
    internal class ElementListToolWindowViewModel : ToolWindowViewModel
    {
        public List<object> Elements { get; private set; }

        #region SelectedElement
        private object selectedElement;
        public object SelectedElement
        {
            get { return this.selectedElement; }
            set
            {
                if (this.SetProperty(ref this.selectedElement, value))
                {
                    var dobvm = this.selectedElement as DependencyObjectViewModel;

                    if (dobvm != null)
                    {
                        var uiElement = dobvm.Model as UIElement;

                        if (uiElement != null)
                        {
#pragma warning disable 4014
                            DebugConsoleViewModel.Instance.VisualTreeView.SelectItemAsync(uiElement);
#pragma warning restore 4014
                        }
                    }
                }
            }
        }
        #endregion

        public string Header { get; private set; }

        public ElementListToolWindowViewModel(List<object> elements, string header)
        {
            this.Elements = elements;
            this.Header = header;
            //DebugConsoleViewModel.Instance.ToolWindows.Add(this);
        }
    }
}