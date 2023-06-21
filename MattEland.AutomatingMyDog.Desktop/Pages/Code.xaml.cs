using MattEland.AutomatingMyDog.Core;
using MattEland.AutomatingMyDog.Desktop.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.ConversationalUI;

namespace MattEland.AutomatingMyDog.Desktop.Pages;

/// <summary>
/// Interaction logic for Chat.xaml
/// </summary>
public partial class Code : UserControl
{
    
    public Code()
    {
        InitializeComponent();

        SyntaxHelpers.ConfigureSyntaxEditor(syntaxTextToSpeech, "ExampleCode/TextToSpeech.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxSpeechToText, "ExampleCode/SpeechToText.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxCropping, "ExampleCode/Cropping.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxVision, "ExampleCode/ComputerVision.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxText, "ExampleCode/TextAnalysis.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxLUIS, "ExampleCode/LUIS.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxCLU, "ExampleCode/CLU.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxOpenAI, "ExampleCode/OpenAI.cs");
    }
}
