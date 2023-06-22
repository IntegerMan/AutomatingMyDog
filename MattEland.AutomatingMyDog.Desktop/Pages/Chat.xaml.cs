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
public partial class Chat : UserControl
{
    public static Author UserAuthor = new("User");
    public static Author DogOSAuthor = new("DogOS");
    public static Author CLUAuthor = new("Conversational Language Understanding (CLU)");
    public static Author TextAnalysisAuthor = new("Text Analysis");
    public static Author ComputerVisionAuthor = new("Computer Vision");
    public static Author LuisAuthor = new("Language Understanding (LUIS)");
    public static Author ErrorAuthor = new("App Error Handler");
    public static Author OpenAIAuthor = new("OpenAI Prompt");

    public Chat()
    {
        InitializeComponent();

        SyntaxHelpers.ConfigureSyntaxEditor(syntaxText, "ExampleCode/TextAnalysis.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxLUIS, "ExampleCode/LUIS.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxCLU, "ExampleCode/CLU.cs");
        SyntaxHelpers.ConfigureSyntaxEditor(syntaxOpenAI, "ExampleCode/OpenAI.cs");
    }

    internal static Author GetAuthor(MessageSource source)
    {
        return source switch
        {
            MessageSource.User => UserAuthor,
            MessageSource.DogOS => DogOSAuthor,
            MessageSource.CLU => CLUAuthor,
            MessageSource.LanguageUnderstanding => LuisAuthor,
            MessageSource.TextAnalytics => TextAnalysisAuthor,
            MessageSource.Error => ErrorAuthor,
            MessageSource.ComputerVision => ComputerVisionAuthor,
            MessageSource.OpenAI => OpenAIAuthor,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
        };
    }
}
