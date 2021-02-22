This is the C# version of Text Templates (see https://github.com/eisnerw/texttemplate-editor).

To enter the working directory, execute "bash chdir".  The source files are in the antlr4_example subdirectory.  

To generate new ANTLR javascript files (in /generated):
python generate_grammar.py TextTemplateLexer.G4
python generate_grammar.py TextTemplateParser.G4

To run the program, execute "dotnet run" from the working directory.

