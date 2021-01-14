using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using TextTemplate;
using System.Diagnostics;
using System.Collections.Generic;

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
			input = @"{#tests.Assert(#expected,'ALL TESTS PASSED')}

Subtemplates:
{#test:[{$0.Matches(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)->
[Test {$0} {$1.Assert($2,'passed')}]}]}
{#test1:[
Explicit Single Indent{#data:[
   {lastName}]}]}
{#test1Result:[
Explicit Single Indent
   Johnson
   Smith]}
{#test2:[
Implied Single Indent {#data:[{lastName}]}]}
{#test2Result:[
Implied Single Indent 
   Johnson
   Smith]}
{#test3:[
Implied Double Indent{#data:[{lastName}{pets:[{name}]}]}]}
{#test3Result:[
Implied Double Indent
   Johnson
      Buddy
      Ralph
      Stripes
   Smith
      Toto
      Dolly
      Stripes]}
{#test4:[
Bulleted Implied Bulleted Indent{#data:[ //
{.} {lastName}{pets:[{name}]}]}]}
{#test4Result:[
Bulleted Implied Bulleted Indent
(0.0) Johnson
   (1.0) Buddy
   (1.1) Ralph
   (1.2) Stripes
(0.1) Smith
   (1.0) Toto
   (1.1) Dolly
   (1.2) Stripes].Compose()}
{#test5:[
Bulleted Explicit Bulleted Indent{#data:[ //
{.} {lastName}{pets:[ //
    {.} {name}]}]}].Compose()}
{#test5Result:[
Bulleted Explicit Bulleted Indent
(0.0) Johnson
    (1.0) Buddy
    (1.1) Ralph
    (1.2) Stripes
(0.1) Smith
    (1.0) Toto
    (1.1) Dolly
    (1.2) Stripes]}
{#test6:[
Hierarchy{#data:[ //
{.} {lastName}{pets:[ //
    {.} {name} who is owned by 
         {.} {^.^:[{firstName} who owns
              {.} {pets:[{type}]}]}]}]}].Compose()}
{#test6Result:[
Hierarchy
(0.0) Johnson
    (1.0) Buddy who is owned by 
         (2.0) Doris who owns
              (3.0) dog
              (3.1) lion
              (3.2) tiger
    (1.1) Ralph who is owned by 
         (2.0) Doris who owns
              (3.0) dog
              (3.1) lion
              (3.2) tiger
    (1.2) Stripes who is owned by 
         (2.0) Doris who owns
              (3.0) dog
              (3.1) lion
              (3.2) tiger
(0.1) Smith
    (1.0) Toto who is owned by 
         (2.0) John who owns
              (3.0) dog
              (3.1) cat
              (3.2) zebra
    (1.1) Dolly who is owned by 
         (2.0) John who owns
              (3.0) dog
              (3.1) cat
              (3.2) zebra
    (1.2) Stripes who is owned by 
         (2.0) John who owns
              (3.0) dog
              (3.1) cat
              (3.2) zebra]}
{#test7:[
Hierarchy that transitions from indent to bullet {#data:[ //
{lastName}{pets:[ //
    {.} {name} who is owned by 
         {.} {^.^:[{firstName} who owns
            {.} {pets:[{type}]}]}]}]}]}
{#test7Result:[
Hierarchy that transitions from indent to bullet 
Johnson
    (0.0) Buddy who is owned by 
         (1.0) Doris who owns
            (2.0) dog
            (2.1) lion
            (2.2) tiger
    (0.1) Ralph who is owned by 
         (1.0) Doris who owns
            (2.0) dog
            (2.1) lion
            (2.2) tiger
    (0.2) Stripes who is owned by 
         (1.0) Doris who owns
            (2.0) dog
            (2.1) lion
            (2.2) tiger
Smith
    (0.0) Toto who is owned by 
         (1.0) John who owns
            (2.0) dog
            (2.1) cat
            (2.2) zebra
    (0.1) Dolly who is owned by 
         (1.0) John who owns
            (2.0) dog
            (2.1) cat
            (2.2) zebra
    (0.2) Stripes who is owned by 
         (1.0) John who owns
            (2.0) dog
            (2.1) cat
            (2.2) zebra]}
{#test8:[
Hierarchy that also transitions from indent to bullet {#data:[ //
  {firstName} {lastName} who owns {pets:[{name} who is owned by 
         {.} {^.^:[{firstName} who owns
            {.} {pets:[{type}]}]}]}]}]}
{#test8Result:[
Hierarchy that also transitions from indent to bullet 
  Doris Johnson who owns 
     Buddy who is owned by 
         (0.0) Doris who owns
            (1.0) dog
            (1.1) lion
            (1.2) tiger
     Ralph who is owned by 
         (0.0) Doris who owns
            (1.0) dog
            (1.1) lion
            (1.2) tiger
     Stripes who is owned by 
         (0.0) Doris who owns
            (1.0) dog
            (1.1) lion
            (1.2) tiger
  John Smith who owns 
     Toto who is owned by 
         (0.0) John who owns
            (1.0) dog
            (1.1) cat
            (1.2) zebra
     Dolly who is owned by 
         (0.0) John who owns
            (1.0) dog
            (1.1) cat
            (1.2) zebra
     Stripes who is owned by 
         (0.0) John who owns
            (1.0) dog
            (1.1) cat
            (1.2) zebra]}
{#test9:[
Continued list {#data:[{pets:[{name}]}]}]}
{#test9Result:[
Continued list 
   Buddy
   Ralph
   Stripes
   Toto
   Dolly
   Stripes]}
{#test10:[
Bulleted Continued list ` 
{#data:[{pets:[ 
    {.} {name}]}]}]}
{#test10Result:[
Bulleted Continued list 
    (0.0) Buddy
    (0.1) Ralph
    (0.2) Stripes
    (0.3) Toto
    (0.4) Dolly
    (0.5) Stripes]}
{#test11:[
Hierarchy that also transitions from indent to bullet and back {#data:[ //
{lastName} who owns 
    {.} {pets:[{name} who is owned by 
         {^.^:[{firstName} who owns 
             {.} {pets:[{type} // 
            ]}]}]}]}]}
{#test11Result:[
Hierarchy that also transitions from indent to bullet and back 
Johnson who owns 
    (0.0) Buddy who is owned by 
         Doris who owns 
             (1.0) dog
             (1.1) lion
             (1.2) tiger
    (0.1) Ralph who is owned by 
         Doris who owns 
             (1.0) dog
             (1.1) lion
             (1.2) tiger
    (0.2) Stripes who is owned by 
         Doris who owns 
             (1.0) dog
             (1.1) lion
             (1.2) tiger
Smith who owns 
    (0.0) Toto who is owned by 
         John who owns 
             (1.0) dog
             (1.1) cat
             (1.2) zebra
    (0.1) Dolly who is owned by 
         John who owns 
             (1.0) dog
             (1.1) cat
             (1.2) zebra
    (0.2) Stripes who is owned by 
         John who owns 
             (1.0) dog
             (1.1) cat
             (1.2) zebra]}
{#test12:[
Additional variation of hierarchy that also transitions from indent to bullet `
 and back {#data:[ //
{lastName} who owns 
    {.} {pets:[{name} who is owned by 
        {.} {^.^:[{firstName} who owns {pets:[{type} //  
            ]}]}]}]}]}
{#test12Result:[
Additional variation of hierarchy that also transitions from indent to bullet and back 
Johnson who owns 
    (0.0) Buddy who is owned by 
        (1.0) Doris who owns 
           (2.0) dog
           (2.1) lion
           (2.2) tiger
    (0.1) Ralph who is owned by 
        (1.0) Doris who owns 
           (2.0) dog
           (2.1) lion
           (2.2) tiger
    (0.2) Stripes who is owned by 
        (1.0) Doris who owns 
           (2.0) dog
           (2.1) lion
           (2.2) tiger
Smith who owns 
    (0.0) Toto who is owned by 
        (1.0) John who owns 
           (2.0) dog
           (2.1) cat
           (2.2) zebra
    (0.1) Dolly who is owned by 
        (1.0) John who owns 
           (2.0) dog
           (2.1) cat
           (2.2) zebra
    (0.2) Stripes who is owned by 
        (1.0) John who owns 
           (2.0) dog
           (2.1) cat
           (2.2) zebra]}
{#test13:[
Yet another variation of hierarchy that also transitions from indent to bullet `
 and back {#data:[ //
{lastName} who owns 
    {.} {pets:[{name} who is owned by 
        {^.^:[{firstName} who owns {pets:[{type} //  
            ]}]}]}]}]}
{#test13Result:[
Yet another variation of hierarchy that also transitions from indent to bullet and back 
Johnson who owns 
    (0.0) Buddy who is owned by 
        Doris who owns 
           dog
           lion
           tiger
    (0.1) Ralph who is owned by 
        Doris who owns 
           dog
           lion
           tiger
    (0.2) Stripes who is owned by 
        Doris who owns 
           dog
           lion
           tiger
Smith who owns 
    (0.0) Toto who is owned by 
        John who owns 
           dog
           cat
           zebra
    (0.1) Dolly who is owned by 
        John who owns 
           dog
           cat
           zebra
    (0.2) Stripes who is owned by 
        John who owns 
           dog
           cat
           zebra]}
{#test14:[
Indent under indent under bullet {#data:[ //
{lastName} who owns 
    {.} {pets:[{name} who is owned by 
             {^.^:[{firstName} who owns 
                 {pets:[{type} //  
            ]}]}]}]}]}
{#test14Result:[
Indent under indent under bullet 
Johnson who owns 
    (0.0) Buddy who is owned by 
             Doris who owns 
                 dog
                 lion
                 tiger
    (0.1) Ralph who is owned by 
             Doris who owns 
                 dog
                 lion
                 tiger
    (0.2) Stripes who is owned by 
             Doris who owns 
                 dog
                 lion
                 tiger
Smith who owns 
    (0.0) Toto who is owned by 
             John who owns 
                 dog
                 cat
                 zebra
    (0.1) Dolly who is owned by 
             John who owns 
                 dog
                 cat
                 zebra
    (0.2) Stripes who is owned by 
             John who owns 
                 dog
                 cat
                 zebra]}
{#test15:[
Automatic bulleting for multi-values{#data:[ //
{lastName} who owns 
    {.} {pets:[{name} who is owned by 
             {^.^:[{firstName} who owns 
                 {pets:[{type} //  
            ]}]}]}]}]}
{#test15Result:[
Automatic bulleting for multi-values
Johnson who owns 
    (0.0) Buddy who is owned by 
             Doris who owns 
                 dog
                 lion
                 tiger
    (0.1) Ralph who is owned by 
             Doris who owns 
                 dog
                 lion
                 tiger
    (0.2) Stripes who is owned by 
             Doris who owns 
                 dog
                 lion
                 tiger
Smith who owns 
    (0.0) Toto who is owned by 
             John who owns 
                 dog
                 cat
                 zebra
    (0.1) Dolly who is owned by 
             John who owns 
                 dog
                 cat
                 zebra
    (0.2) Stripes who is owned by 
             John who owns 
                 dog
                 cat
                 zebra]}
{#test16:[
{''.Compose() // reset numbering from previous tests
}{[
A:
People:{'/data/people':[
{.} Pets {[
      {.} Domesticated{pets:[{type}]}
   ]
   } //
].Compose()}
B:
People:{'/data/people':[
   {.} {lastName}{pets:[{type}]}
].@MissingValue('No Information Provided').Compose()}
C:
People:{'/data/people':[
   {.} {lastName}{pets:[{type}]}
].Compose()}   
D:
People:{'/data/people':[
          {lastName}{pets:[{type}]}
].@MissingValue('No Information Provided').Compose()}
E:
People:{'/data/people':[
{.} {lastName}{pets:[
 {.} {type}]}
].@MissingValue('No Information Provided').Compose()}]}]}
{#test16Result:[
A:
People:
(0.0) Pets 
      (1.0) Domesticated
         (2.0) dog
         (2.1) lion
         (2.2) tiger
(0.1) Pets 
      (1.0) Domesticated
         (2.0) dog
         (2.1) cat
         (2.2) zebra
B:
People:
   (0.0) Johnson
      (1.0) dog
      (1.1) lion
      (1.2) tiger
   (0.1) Smith
      (1.0) dog
      (1.1) cat
      (1.2) zebra
C:
People:
   (0.0) Johnson
      (1.0) dog
      (1.1) lion
      (1.2) tiger
   (0.1) Smith
      (1.0) dog
      (1.1) cat
      (1.2) zebra   
D:
People:Johnson
   dog
   lion
   tiger
Smith
   dog
   cat
   zebra
E:
People:
(0.0) Johnson
 (1.0) dog
 (1.1) lion
 (1.2) tiger
(0.1) Smith
 (1.0) dog
 (1.1) cat
 (1.2) zebra]}
{#tests:[ //
{'1'.#test(#test1,#test1Result)}
{'2'.#test(#test2,#test2Result)}
{'3'.#test(#test3,#test3Result)}
{'4'.#test(#test4,#test4Result)}
{'5'.#test(#test5,#test5Result)}
{'6'.#test(#test6,#test6Result)}
{'7'.#test(#test7,#test7Result)}
{'8'.#test(#test8,#test8Result)}
{'9'.#test(#test9,#test9Result)}
{'10'.#test(#test10,#test10Result)}
{'11'.#test(#test11,#test11Result)}
{'12'.#test(#test12,#test12Result)}
{'13'.#test(#test13,#test13Result)}
{'14'.#test(#test14,#test14Result)}
{'15'.#test(#test15,#test15Result)}
{'16'.#test(#test16,#test16Result)}
]}
{#expected:[ //
Test 1 passed
Test 2 passed
Test 3 passed
Test 4 passed
Test 5 passed
Test 6 passed
Test 7 passed
Test 8 passed
Test 9 passed
Test 10 passed
Test 11 passed
Test 12 passed
Test 13 passed
Test 14 passed
Test 15 passed
Test 16 passed
]}
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
            Dictionary<string, object> options = new Dictionary<string, object>();
            options["urlCallback"] = urlCallback;
			string result = visitor.interpret(input, options);
			Debug.Write(result + "\n");
			Console.Write(result + "\n");
		}
	}
}
