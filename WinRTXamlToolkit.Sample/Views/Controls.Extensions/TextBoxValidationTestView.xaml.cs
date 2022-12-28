using System.Threading.Tasks;
using WinRTXamlToolkit.Sample.ViewModels.Controls.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Sample.Views
{
    public sealed partial class TextBoxValidationTestView : UserControl
    {
        public TextBoxValidationTestView()
        {
            this.InitializeComponent();
            this.DataContext = new TextBoxValidationTestPageViewModel();
        }
    }
}
