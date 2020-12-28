using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTextTemplate
{
    class TemplateData
    {
		/*
			private dictionary = {};
			private list: TemplateData[] = [];
			private parent : TemplateData;
			static foundObjects : any; // used to protect against ToJson loops
			type: string;
			constructor(jsonData: string | {} | [], parent?: TemplateData) {
				let json: {};
				if (typeof jsonData == 'string') {
					if (jsonData.startsWith('{') || jsonData.startsWith('[')){
						json = JSON.parse(jsonData);
					} else {
						// TemplateData supports arrays of strings by making them lists of dictionaries with a single 
						json = JSON.parse('{"_": "' + jsonData.replace('"','\\"') + '"}');
					}
				} else if (Array.isArray(jsonData)) {
					this.type = 'list';
					let array: [] = jsonData;
					array.forEach((item) => {
						this.list.push(new TemplateData(item, this));
					});
					this.parent = parent;
					return;
				} else if (jsonData instanceof TemplateData){ // filter or clone
					if ((<TemplateData>jsonData).type == 'list'){
						this.type = 'list';
						(<TemplateData>jsonData).list.forEach((item)=>{
							this.list.push(new TemplateData(item, parent));
						});
						return;
					} else {
						json = JSON.parse((<TemplateData>jsonData).toJson()); // clone by converting to Json and back
					}
				} else {
					json = jsonData;
				}
				if (Array.isArray(json)){
					this.type = 'list';
					json.forEach((item) => {
						this.list.push(new TemplateData(item, this));
					});
				} else {
					this.type = 'dictionary';
					Object.keys(json).forEach((keyname) => {
						let value: any = json[keyname];
						if (typeof value == 'object' && value != null) {
							if (value != null && (!Array.isArray(value) || value.length > 0)){ // don't add null values or empty arrays
								this.dictionary[keyname] = new TemplateData(value, this);
							}
						} else {
							this.dictionary[keyname] = value;
						}
					});
				}
				if (parent){
					this.parent = parent;
				}
			}
			getKeys() : string[] {
				return Object.keys(this.dictionary);
			}
			getValue(key : string) : any {
				let keySplit = key.split('.');
				let value = this.dictionary[keySplit[0]];
				if (value == undefined && keySplit[0] == '^'){
					value = this.parent; 
					if (value == undefined){
						value = this; // allows ^.^... to get to the top
					}
				}
				if (keySplit.length == 1 || value === undefined){
					return value;
				}
				if (value instanceof TemplateData){
					return <TemplateData>value.getValue(keySplit.slice(1).join('.'));
				}
			}
			iterateList(fn: (TemplateData) => any) {
				this.list.forEach((item : TemplateData)=>{
					fn(item);
				});
			}
			count(){
				return this.list.length;
			}
			asList() : TemplateData {
				if (this.type == 'list'){
					// already a list, so just clone it
					return new TemplateData(this);
				}
				return new TemplateData([JSON.parse(this.toJson())]);
			}
			toJson(indentLevel? : number) : string {
				let result : string = '';
				let bComma = false;
				if (indentLevel == null){
					indentLevel = 0;
					TemplateData.foundObjects = [];
				}
				TemplateData.foundObjects.push(this);
				if (this.type == 'list'){
					result += '[\n';
					this.list.forEach((dict) =>{
						result = (result + ((bComma ? ',' : this.indent(indentLevel + 1)) + dict.toJson(indentLevel + 1))).replace(/\n\s*\n/,'\n');
						bComma = true;
					});
					result += ('\n' + this.indent(indentLevel) + ']');
				} else {
					let keys = Object.keys(this.dictionary);
					if (keys.length == 1 && keys[0] == '_'){
						result += ('\n' + this.indent(indentLevel) + '"' + this.dictionary[keys[0]].replace(/["]/g,"\\\"") + '"');
					} else {
						result += '{\n';
						keys.forEach((keyname) => {
							let value : any = this.dictionary[keyname];
							result += (this.indent(indentLevel + 1) + (bComma ? ',' : '') + '"' + keyname + '": ');
							if (value instanceof TemplateData){
								if (!TemplateData.foundObjects.includes(value)){
									result += (<TemplateData>value).toJson(indentLevel + 1);
								}
							} else if (value == null) {
								result += 'null';
							} else if (typeof value == 'string') {
								value = value.replace(/\n/g,'\\n').replace(/\r/g,'\\r');
								result += ('"' + value.replace(/\\/g,'\\').replace(/"/g,'\\"') + '"');
							} else {
								result += value.toString();
							}
							result += '\n';
							bComma = true;
						});
						result += (this.indent(indentLevel) + '}');
					}
				}
				return result;
			}
			add(name : string, value : any){
				this.dictionary[name] = value;
			}
			private indent(indentLevel : number) : string {
				let result : string = '';
				for (let i = 0; i < indentLevel; i++){
					result += '   ';
				}
				return result;
			}

        */
	}
}
