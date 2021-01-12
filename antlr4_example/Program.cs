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
			input = @"Shows use of bullet styles as well as modular subtemplates, automatic bulleting,`
and indenting only for multiple values.  You can see different bullet styles `
by changing the parameter to #showStyles from 1 through 5

{''.#showStyles(1)}

 
Subtemplates:
{#styleNumber:[1]}
{#showStyles:[People:{#data:[
   {.}{^.^.$1 = 2->[.0]} {[{ 
      lastName}{ 
      firstName.#Prepend(', ')}{    
      middleName.#Prepend(' ')
   }].ToUpper().#Quoted()} 
      {.} Lastname: {lastName.IfMissing('No Last Name')}   
      {.} FirstName: {firstName}  
      {.} Pets {pets=>[({pets.Count()}):  
         {.} Domesticated{#dpetcnt > 1->[ ({#dpetcnt})]}: {pets.#Domesticated():[{#formatPet}]}  
         {.} Non-domesticated{#ndpetcnt > 1->[({#ndpetcnt})]}: {pets.#NonDomesticated():[{#formatPet}]}
   ],[no pets]
   } //
].@MissingValue('No Information Provided')}].@Include([{'#style'}{$1}])}
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
{#dpetcnt:[{pets.#Domesticated().Count()}]}
{#ndpetcnt:[{pets.#NonDomesticated().Count()}]}
{#style1:[].@BulletStyle('I.','(a)','•','i.')}
{#style2:[].@BulletStyle('1','1.1','1.1.1','1(a)')}
{#style3:[].@BulletStyle('-')}
{#style4:[].@BulletStyle('')}
{#style5:[].@BulletStyle(' I', '•', 'A.', '(1)')}
{#data:[/data/people]}";
			input = @"{'/data/events'.GroupBy(start.ToDate('YYYY-MM-DDT00:00:00'), 'group', 'start'):[ `
{start.ToUpper()} 
   {group.OrderBy(start.ToDate('HHmm')):[{start.#formatTimes(end)}: {summary} 
      {.} Notes: {description}
      {.} Location {location}]}
].@DateTest(/(star|end)/i).@BulletStyle('•').@DateFormat('dddd MMM D')}
Subtemplates:
{#SameDay:[{$0.ToDate('YYYYMMDD') = $1.ToDate('YYYYMMDD')}]}
{#SameAMPM:[{$0.ToDate('a') = $1.ToDate('a') & $0.#SameDay($1)}]}
{#onHour:[{$0.ToDate('mm') = '00'}]}
{#formatTimes:[
   {
   // Don't put am/pm for the start time if on the hour and same as end
   $0.ToDate([h{!$0.#onHour()->[:mm]}{!$0.#SameAMPM($1)->[a]}])
   }-{
   !$1.#SameDay($0)->[{$1.ToDate().ToUpper()} at ]}{$1.ToDate('h:mma')
   }
]}";
			input = input.Replace("\r", "");
			AntlrInputStream inputStream = new AntlrInputStream(input);
			TextTemplateLexer textTemplateLexer = new TextTemplateLexer(inputStream);
			CommonTokenStream commonTokenStream = new CommonTokenStream(textTemplateLexer);
			TextTemplateParser textTemplateParser = new TextTemplateParser(commonTokenStream);
			TextTemplateParser.CompilationUnitContext compilationUnitContext = textTemplateParser.compilationUnit();
			TextTemplateVisitor visitor = new TextTemplateVisitor();
			Func<string, string> urlCallback = url =>
			{
				string ret = "Bad Url";
				switch (url)
				{
					case "/data/people":
						ret = @"[
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
						break;
					case "/data/events":
						ret = @"[
	{""start"": ""2020-05-20T19:00:00"", ""end"": ""2020-05-20T22:30:00"",""summary"": ""Dinner with Mom"", ""description"":""Dresscode: Elegant and ironed!"", ""location"": ""800 Howard St., San Francisco, CA 94103""}, 
	{""start"": ""2020-06-20T15:00:00"", ""end"": ""2020-06-22T15:30:00"",""summary"": ""Hotdog eating competition"", ""location"": ""43 Chapel Ave, Jersey City, NJ 07305""}, 
	{""start"": ""2020-05-28T10:00:00"", ""end"": ""2020-05-28T12:15:00"",""summary"": ""Vet"", ""description"":""Brush the dog's teeth"", ""location"": ""3771 Van Dyke Ave San Diego, CA 92105""}, 
	{""start"": ""2020-05-28T08:30:00"", ""end"": ""2020-05-28T10:00:00"",""summary"": ""Meet with Paul"", ""description"":""Discussion of future plans"", ""location"": ""1200 Railing St., Brunswick, Md.""}, 
	{""start"": ""2020-06-30T10:00:00"", ""end"": ""2020-06-30T11:30:00"",""summary"": ""Jogging class"", ""description"":""Bring your inhaler"", ""location"": ""3014 Rosalinda San Clemente, CA 92673""}
]";
						break;
				}
				return ret;
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
