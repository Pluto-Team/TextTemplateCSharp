using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using TextTemplate;
using System.Diagnostics;

namespace TestTextTemplates
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				/*var text = new StringBuilder();
				Console.WriteLine("Input the chat.");

				// to end the input, enter a blank line then press <enter> 
				while ((input = Console.ReadLine()) != "")
				{
					text.AppendLine(input);
				}

				var inputStream = new AntlrInputStream(text.ToString());
				var speakLexer = new SpeakLexer(inputStream);
				var commonTokenStream = new CommonTokenStream(speakLexer);
				var speakParser = new SpeakParser(commonTokenStream);
				var chatContext = speakParser.chat();
				var visitor = new SpeakVisitor();
				visitor.Visit(chatContext);
				foreach (var line in visitor.Lines)
				{
					Console.WriteLine("{0} has said {1}", line.Person, line.Text);
				}
				*/
				string input = @"{'{""hello"":""world""}':[{""hello""}:{hello}]}";
				AntlrInputStream inputStream = new AntlrInputStream(input);
				TextTemplateLexer textTemplateLexer = new TextTemplateLexer(inputStream);
				CommonTokenStream commonTokenStream = new CommonTokenStream(textTemplateLexer);
				TextTemplateParser textTemplateParser = new TextTemplateParser(commonTokenStream);
				TextTemplateParser.CompilationUnitContext compilationUnitContext = textTemplateParser.compilationUnit();
				TextTemplateVisitor visitor = new TextTemplateVisitor();
				string interpolated = (string)visitor.Visit(compilationUnitContext);
				Debug.Write(interpolated);
				Console.Write(interpolated);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex);
			}
		}
	}
}