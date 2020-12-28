using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTextTemplate
{
    class BulletIndent
    {
		/*
			public level : number;
			public index : number;
			public indent : string;
			public parent : BulletIndent;
			public lastBullet = '';
			public bulletStyles = null;
			// the next is used to indicate that we've returned from a level where we could have honored a style 
			// initializer (e.g., "I:IV") so don't do that the next time we visit levels above this one
			public styleInitializerLevel : number = null; 
			constructor(indent : string = null, currentBulletIndent : BulletIndent = null, level = null, bulletStyles = null){
				if (indent == null){
					// indicates an empty BulletIndent
					return;
				}
				this.bulletStyles = bulletStyles;
				let currentIndent = currentBulletIndent == null ? '' : currentBulletIndent.indent;
				this.indent = indent;
				if (currentBulletIndent == null ){
					// establish the first level
					this.level = level == null ? 0 : level;
					this.index = 0;
					this.parent = null;
				} else if (this.level == level || indent == currentIndent){
					// staying on the same level
					this.level = currentBulletIndent.level;
					this.index = currentBulletIndent.index + 1;
					this.parent = currentBulletIndent.parent;
					this.styleInitializerLevel = currentBulletIndent.styleInitializerLevel;
				} else {
					// search for the same level
					let matchingLevel : BulletIndent = currentBulletIndent.parent; // used to find a previous level
					while (matchingLevel != null){
						if ((level != null && matchingLevel.level == level) || indent == matchingLevel.indent){
							// found a matching level, so this one is a continuation
							this.level = matchingLevel.level;
							this.index = matchingLevel.index + 1;
							this.parent = matchingLevel.parent;
							this.styleInitializerLevel = this.level; // don't honor style initializers above this level
							break;
						} else {
							matchingLevel = matchingLevel.parent;
						}
					} 
					if (matchingLevel == null){
						// create a new level
						this.level = level == null ? (currentBulletIndent.level + 1) : level;
						this.index = 0;
						this.parent = currentBulletIndent;
						this.styleInitializerLevel = currentBulletIndent.styleInitializerLevel;
					}
				}
			}
			clone(){
				// clone a bulletIndent that reflects the state 
				let cloneBulletIndent = new BulletIndent();
				cloneBulletIndent.level = this.level;
				cloneBulletIndent.index = this.index;
				cloneBulletIndent.indent = this.indent;
				cloneBulletIndent.parent = this.parent;
				cloneBulletIndent.lastBullet = this.lastBullet;
				cloneBulletIndent.bulletStyles = this.bulletStyles;
				cloneBulletIndent.styleInitializerLevel = this.styleInitializerLevel;
				return cloneBulletIndent;
			}
			getBullet(){
				let bullet;
				let bulletStyles = this.bulletStyles;
				if (bulletStyles == null || bulletStyles.length == 0){
					bullet = '(' + this.level + '.' + this.index + ')'; // TODO: default bullet style
				} else {
					let bulletStyleText = bulletStyles[this.level < bulletStyles.length ? this.level : bulletStyles.length - 1];
					// support multiple numbers at one level by creating an array of styles that contain 0 or 1 number/letter/roman
					let concatenatedBullet = '';
					let styleArray = bulletStyleText.replace(/(.*?(i\:[ivxldcm]+|i|I\:[IVXLDCM]+|I|1\:\d+|1|a\:[a-z]+|a|A\:[A-Z]+|A).\S*?)/g,'$1\x02').split('\x02');
					let currentBulletLevel : any = this;
					for (let i = styleArray.length - 1; i >= 0 && currentBulletLevel != null; i--){
						let bulletStyle = styleArray[i];
						let padding = '';
						if (/^ +/.test(bulletStyle)){
							padding = bulletStyle.replace(/^( +).*$/, '$1');
							bulletStyle = bulletStyle.substr(padding.length);
						}
						let prefix = '';
						let postfix = '';
						let bulletType = '';
						// TODO: support styles like 'I.a.1.i', '1.1.1.1' or even '1:10.1.1.1'
						// TODO: consider allowing \: and \\ for legitimate :
						// support styles like 'i', 'i:iv', 'I', 'I:LV', '1', '1:13', 'a', 'a:d', 'A', 'A:AF'
						if (/^.*(i\:[ivxldcm]+|i|I\:[IVXLDCM]+|I|1\:\d+|1|a\:[a-z]+|a|A\:[A-Z]+|A).*$/.test(bulletStyle)){
							prefix = bulletStyle.replace(/^(.*?)(i\:[ivxldcm]+|i|I\:[IVXLDCM]+|I|1\:\d+|1|a\:[a-z]+|a|A\:[A-Z]+|A).*$/,'$1');
							postfix = bulletStyle.replace(/^.*?(i\:[ivxldcm]+|i|I\:[IVXLDCM]+|I|1\:\d+|1|a\:[a-z]+|a|A\:[A-Z]+|A)(.*)$/,'$2');
							bulletStyle = bulletStyle.replace(/^.*?(i\:[ivxldcm]+|i|I\:[IVXLDCM]+|I|1\:\d+|1|a\:[a-z]+|a|A\:[A-Z]+|A).*$/,'$1');
							bulletType = bulletStyle.substr(0, 1);
							if (bulletStyle.includes(':')){
								if (this.styleInitializerLevel != null && currentBulletLevel.level > this.styleInitializerLevel){
									// ignore the style initializer because we've already popped back to a level above this
									bulletStyle = bulletType; 
								} else {
									// capture the style initializer, which is the value after the ':'
									bulletStyle = bulletStyle.substr(bulletStyle.indexOf(':') + 1);
								}
							}
						} else if (bulletStyle.length > 1){
							if ('(<#$%*.-=+`~[{_=+|\'"'.includes(bulletStyle.substr(0,1))){
								prefix = bulletStyle[0];
								bulletStyle = bulletStyle.substr(1);
							}
							if (')>*]}.`~*-_=+|:\'"'.includes(bulletStyle.substr(bulletStyle.length - 1, 1))){
								postfix = bulletStyle[bulletStyle.length - 1];
								bulletStyle = bulletStyle.substr(0, bulletStyle.length - 1);
							}
						}
						bullet = bulletStyle;
						if (bulletType.length == 1){
							switch (bulletType){
								case 'I':
									bullet = this.numberToRoman(currentBulletLevel.index + (bulletStyle != 'I' ? this.romanToNumber(bulletStyle) : 1));
									break;
						
								case 'i':
									bullet = this.numberToRoman(currentBulletLevel.index + (bulletStyle != 'i' ? this.romanToNumber(bulletStyle) : 1)).toLowerCase();
									break;
						
								case '1':
									bullet = (currentBulletLevel.index + (bulletStyle != '1' ? parseInt(bulletStyle) : 1)).toString();
									break;
						
								case 'A':
								case 'a':
									bullet = this.numberToAlphabet(currentBulletLevel.index + (bulletStyle.toLowerCase() != 'a' ? this.alphabetToNumber(bulletStyle) : 1));
									if (bulletType == 'a'){
										bullet = bullet.toLowerCase();
									}
									break;
							}
							if (padding.length > 0 && bullet.length < (padding.length + 1)){
								prefix = padding.substr(0, padding.length - bullet.length + 1) + prefix;
							}
							bullet = prefix + bullet + postfix;
							currentBulletLevel = currentBulletLevel.parent; 
						}
						concatenatedBullet = bullet + concatenatedBullet;
					}
					bullet = concatenatedBullet;
				}
				this.lastBullet = this.indent + bullet.replace('\x01', '');
				return bullet;	
			}
			numberToRoman(n : number) : string{
				// from vetalperko via Brendon Shaw
				let b = 0;
				let s = '';
				for (let a = 5; n != 0; b++,a ^= 7){
					let o = n % a;
					for(n = n/a^0; o--;){
						s = 'IVXLCDM'[o > 2 ? b + n - (n &= -2) + (o = 1) : b] + s;
					}
				}
				return s;
			}
			romanToNumber(romanNumeral : string) : number {
			  let DIGIT_VALUES = {I: 1,V: 5,X: 10,L: 50,C: 100,D: 500,M: 1000};
			  let result = 0;
			  let input = romanNumeral.toUpperCase().split('');
			  for (let i = 0; i < input.length; i++) {
				let currentLetter = DIGIT_VALUES[input[i]];
				let nextLetter = DIGIT_VALUES[input[i + 1]];
				if (currentLetter == null) {
				  return -1;
				} else {
				  if (currentLetter < nextLetter) {
					result += nextLetter - currentLetter;
					i++;
				  } else {
					result += currentLetter;
				  }
				}
			  }
			  return result;
			}
			numberToAlphabet (num : number) {
				// from Chris West's routine to convert spreadsheet columns to numbers and back
				let ret = '';
				let b = 26;
				for (let a = 1; (num -a) >= 0; b *= 26) {
					num -= a;
					ret = String.fromCharCode(((num % b) / a) + 65) + ret;
					a = b;
				}
				return ret;
			}
			alphabetToNumber(alpha : string) : number {
				let number = 0;
				for (let i = alpha.length, j = 0; i--; j++) {
					number += Math.pow(26, i) * (alpha.toUpperCase().charCodeAt(j) - 64);
				}
				return number;
			}	

         */
	}
}
