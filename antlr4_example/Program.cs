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
				//input = @"{'{""hello"":""world""}':[{""hello""} {hello}]}";
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
			input = @"People:{#data:[
   {.} {[{ 
      lastName}{ 
      firstName.#Prepend(', ')}{    
      middleName.#Prepend(' ')
   }].ToUpper().#Quoted()} 
      {.} Lastname: {lastName.IfMissing('No Last Name')}   
      {.} FirstName: {firstName}  
      {.} Pets {pets=>[({pets.Count()}):  
         {.} Domesticated ({pets.#Domesticated().Count()}): 
            {.} {pets.#Domesticated():[{#formatPet}]}  
         {.} Non-domesticated ({pets.#NonDomesticated().Count()}): 
            {.} {pets.#NonDomesticated():[{#formatPet}]}    
   ],[no pets]
   } //
].@MissingValue('No Information Provided')}       
 
Subtemplates:  

{#formatPet:[  
   a {type.Case('zebra',[{[{name}y].ToLower()}],'dog','friskie','cat','fishy','nice')}  `   
   {type} named {name} 
]}
{#Domesticated: [{$0.Where(type.#IsDomesticated())}]}
{#NonDomesticated: [{$0.Where(!type.#IsDomesticated())}]} 
{#IsDomesticated:[{$0.Matches('dog','cat')}]}  
{#Prepend:[{$0->[{$1}{$0}]}]}
{#Surround:[{$0->[{$1}{$0}{$1}]}]}
{#Quoted:[{$0->[{$0.#Surround('""')}]}]}
{#data:[/data/people]}";
			input = input.Replace("\r", "");
				AntlrInputStream inputStream = new AntlrInputStream(input);
				TextTemplateLexer textTemplateLexer = new TextTemplateLexer(inputStream);
				CommonTokenStream commonTokenStream = new CommonTokenStream(textTemplateLexer);
				TextTemplateParser textTemplateParser = new TextTemplateParser(commonTokenStream);
				TextTemplateParser.CompilationUnitContext compilationUnitContext = textTemplateParser.compilationUnit();
				TextTemplateVisitor visitor = new TextTemplateVisitor();
				Func<string, string> urlCallback = url =>
				{
					return @"[
	{
	   ""firstName"": ""Doris""
	   ,""lastName"": ""Johnson""
		   ,""pets"": [
			  {
			 ""type"": ""dog""
			 ,""name"": ""Buddy""
		  },{
			 ""type"": ""lion""
			 ,""name"": ""Ralph""
		  },{
			 ""type"": ""tiger""
			 ,""name"": ""Stripes""
		  }
	   ]
	},
	{
	   ""firstName"": ""John""
	   ,""lastName"": ""Smith""
	   ,""pets"": [
		  {
			 ""type"": ""dog""
			 ,""name"": ""Toto""
		  },{
			 ""type"": ""cat""
			 ,""name"": ""Dolly""
		  },{
			 ""type"": ""zebra""
			 ,""name"": ""Stripes""
		  }
	   ]
	}
]";
				};
				string result = visitor.interpret(input, urlCallback);
				Debug.Write(result + "\n");
				Console.Write(result + "\n");
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
