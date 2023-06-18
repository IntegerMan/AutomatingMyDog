using System.IO;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SyntaxEditor.Palettes;
using Telerik.Windows.Controls.SyntaxEditor.Taggers;
using Telerik.Windows.Controls.SyntaxEditor.Tagging.Taggers;
using Telerik.Windows.SyntaxEditor.Core.Text;

namespace MattEland.AutomatingMyDog.Desktop.Pages {
    internal static class SyntaxHelpers {

        public static void ConfigureSyntaxEditor(RadSyntaxEditor syntaxEditor, string filePath) {
            using (StreamReader reader = new(filePath)) {
                syntaxEditor.Document = new TextDocument(reader);
            }
            syntaxEditor.TaggersRegistry.RegisterTagger(new CSharpFoldingTagger(syntaxEditor));
            syntaxEditor.TaggersRegistry.RegisterTagger(new CSharpTagger(syntaxEditor));
            syntaxEditor.Palette = SyntaxPalettes.NeutralDark;
            syntaxEditor.ZoomTo(1.25); // 125%
        }
    }
}