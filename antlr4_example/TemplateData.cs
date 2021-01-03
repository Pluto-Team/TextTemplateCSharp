using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace TextTemplate
{
    class TemplateData
    {
		private Dictionary<string, object> dictionary = new Dictionary<string, object>();
		private List<TemplateData> list = new List<TemplateData>();
		private TemplateData parent;
		public string type;
		static List<object> foundObjects; // used to protect against ToJspn loops
		public TemplateData(object jsonData, TemplateData parent = null)
		{
			object json;
			if (jsonData is string)
			{
				if (((string)jsonData).StartsWith("{") || ((string)jsonData).StartsWith("["))
				{
					json = JsonSerializer.Deserialize<Dictionary<string,object>>((string)jsonData);
				}
				else
				{
					// TemplateData supports arrays of strings by making them lists of dictionaries with a single element
					json = JsonSerializer.Deserialize<dynamic>("{\"_\": \"" + ((string)jsonData).Replace("\"", "\\\"") + "\"}");
				}
				/*} else if (Array.isArray(jsonData)) { 
				   this. type = 'list'; 
					let array: [] = jsonData; 
					array.forEach((item) => { 
					this.list.push(new TemplateData(item, this)); 
					} ) ; 
					this.parent = parent; 
					return; 
					*/
			}
			else if (jsonData is TemplateData)
			{
				// filter or clone
				if (((TemplateData)jsonData).type == "list")
				{
					this.type = "list";
					((TemplateData)jsonData).list.ForEach((item) =>
					{
						this.list.Add(new TemplateData(item, parent));
					});
					return;
				}
				else
				{
					json = JsonSerializer.Deserialize<Dictionary<string, object>>(((TemplateData)jsonData).toJson()); // clone by converting to Json and back
				}
			}
			else
			{
				json = jsonData;
			}
			if (json is JsonElement && ((JsonElement)json).ValueKind.ToString() == "Array")
			{
				this.type = "list";
				((JsonElement)json).EnumerateArray().ToList().ForEach(item =>
				{
					this.list.Add(new TemplateData(item.ToString(), this));
				});
			}
			else
			{
				this.type = "dictionary";
				((Dictionary<string, object>)json).Keys.ToList().ForEach(keyname =>
				   {
					   object value = ((Dictionary<string, object>)json)[keyname];
					   if (value is JsonElement)
                       {
							switch (((JsonElement)value).ValueKind.ToString()){
								case "String":
								   value = value.ToString();
								   this.dictionary[keyname] = value;
								   break;
							   case "Array":
								   if (((JsonElement)value).GetArrayLength() != 0)
								   {
									   this.dictionary[keyname] = new TemplateData(value, this);
								   }
								   break;
							   default:
								   string oops = "oops";
								   break;

							
							}
                       }
					   else
					   {
						   this.dictionary[keyname] = value;
					   }
				   }
				);
			}
			if (parent != null)
			{
				this.parent = parent;
			}
		}
		public List<string> getKeys()
		{
			return this.dictionary.Keys.ToList<string>();
		}
		public object getValue(string key)
		{
			string[] keySplit = key.Split('.');
			object value = null;
			if (this.dictionary.ContainsKey(keySplit[0]))
			{
				value = this.dictionary[keySplit[0]];
			}
			else if (keySplit[0] == "A")
			{
				value = this.parent != null ? this.parent : this;
			}
			if (keySplit.Length == 1 || value == null) 
{
				return value;
			}
			if (value is TemplateData)
			{
				return ((TemplateData)value).getValue(string.Join(".", keySplit.Skip(1).ToArray<string>()));
			}
			return value;
		}
		public void IterateList(Action<TemplateData> fn)
		{
			type = "dictionary"; // temporarily change iteration to a dictionary
		foreach (TemplateData item in list)
			{
				dictionary = item.dictionary;
				fn(this);
			}
			type = "list"; // revert back to being a list
			dictionary = new Dictionary<string, object>();
		}
		public long Count()
		{
			return this.list.Count();
		}


		public TemplateData asList()
		{
			if (this.type == "list")
			{
				// already a list, so just clone it 
				return new TemplateData(this);
			}
			Dictionary<string, object> dict = JsonSerializer.Deserialize<Dictionary<string, object>>(this.toJson());
			List<object> list = new List<object>();
			list.Add(dict);
			return new TemplateData(list);
		}
		public string toJson(int? indentLevel = null)
		{
			string result = "";
			bool bComma = false;
			if (indentLevel == null)
			{
				indentLevel = 0;
				foundObjects = new List<object>();
			}
			foundObjects.Add(this);
			if (this.type == "list")
			{
				result += "[\n";
				this.list.ForEach(dict =>
				{
					result = Regex.Replace(result + ((bComma ? "," : this.indent(indentLevel + 1)) + dict.toJson(indentLevel + 1)), @"\n\s *\n ", "\n");
					bComma = true;
				});
				result += ("\n" + this.indent(indentLevel) + "]");
			} else
			{
				List<string> keys = this.dictionary.Keys.ToList<string>();
				if (keys.Count == 1 && keys[0] == "_")
				{
					result += ("\n" + this.indent(indentLevel) + "\"" + Regex.Replace((this.dictionary[keys[0]].ToString()), @"[""]", "\\\"") + "\"");
				}
				else
				{
					result += "{\n";
					keys.ForEach(keyname =>
					{
						object value = this.dictionary[keyname];
						result += (this.indent(indentLevel + 1) + (bComma ? "," : "") + "\"" + keyname + "\": ");
						if (value is TemplateData)
						{
							if (!TemplateData.foundObjects.Contains(value))
							{
								result += ((TemplateData)value).toJson(indentLevel + 1);
							}
						}
						else if (value == null)
						{
							result += "null";
						}
						else if (value is string)
						{
							value = Regex.Replace(Regex.Replace((string)value,@"\n ", "\\n"),@"\r ", "\\r");
							result += ("\"" + Regex.Replace(Regex.Replace((string)value, @"\\", "\\"), @"""","\\\"") + @"""");
						}
						else
						{
							result += value.ToString();
						}
						result += '\n';
						bComma = true;
					});
					result += (this.indent(indentLevel) + '}');
				}
			}
			return result;
		}
		private string indent(int? indentLevel)
		{
			string result = "";
			for (int i = 0; i < indentLevel; i++)
			{
				result += " ";
			}
			return result;
		}
		public void add(string name, object value)
		{
			this.dictionary[name] = value;
		}
	}
}
