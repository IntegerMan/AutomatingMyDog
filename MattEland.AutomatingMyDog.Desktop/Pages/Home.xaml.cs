using System.IO;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SyntaxEditor.Palettes;
using Telerik.Windows.Controls.SyntaxEditor.Taggers;
using Telerik.Windows.Controls.SyntaxEditor.Tagging.Taggers;
using Telerik.Windows.SyntaxEditor.Core.Text;

namespace MattEland.AutomatingMyDog.Desktop.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home() {
            InitializeComponent();

            SyntaxHelpers.ConfigureSyntaxEditor(syntaxTextToSpeech, "ExampleCode/TextToSpeech.cs");
            SyntaxHelpers.ConfigureSyntaxEditor(syntaxSpeechToText, "ExampleCode/SpeechToText.cs");
        }
    }
}
