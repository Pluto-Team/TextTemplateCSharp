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
			//try
			//{
				string input;
				//input = @"{'{""hello"":""world""}':[{""hello""}:{hello}]}";
				input = @"// data context:
{'{""firstName"": ""Robert"", ""lastName"": ""Smith"", ""pets"":[
{""type"":""dog"", ""name"": ""Toto""}
,{""type"":""cat"", ""name"": ""Dolly""}
,{""type"":""zebra"", ""name"": ""Stripes""}
]}':
// Template:
[{""Hello""} {lastName}, {firstName} with {pets=>
   [{pets:#formatPet.#anded()}]
   ,[no pets]
}!!]}

Subtemplates:

{#formatPet:[a {type} named {name}]}
{#anded:[{$0.Join(', ',' and ')}]}";
			input = input.Replace("\r", "");
				AntlrInputStream inputStream = new AntlrInputStream(input);
				TextTemplateLexer textTemplateLexer = new TextTemplateLexer(inputStream);
				CommonTokenStream commonTokenStream = new CommonTokenStream(textTemplateLexer);
				TextTemplateParser textTemplateParser = new TextTemplateParser(commonTokenStream);
				TextTemplateParser.CompilationUnitContext compilationUnitContext = textTemplateParser.compilationUnit();
				TextTemplateVisitor visitor = new TextTemplateVisitor();
				string result = visitor.interpret(input);
				Debug.Write(result);
				Console.Write(result);
			//object interpolated = (string)visitor.Visit(compilationUnitContext);
			//Debug.Write(interpolated);
			//Console.Write(interpolated);
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine("Error: " + ex);
			//}
		}
	}
}
