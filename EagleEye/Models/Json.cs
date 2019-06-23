using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
namespace EagleEye.Models
{
	public class Json
	{
		static public Json Import(StreamReader stream) {
			int i = 0;
			string json = stream.ReadToEnd();
			return ParseTree(ref json, ref i);
		}
		public void Export(StreamWriter stream)
		{
			switch (Type)
			{
				case JsonType.Array:
					{
						stream.Write('[');
						int i = 0;
						foreach (Json child in Children)
						{
							child.Export(stream);
							if (i != Children.Count - 1)
							{
								stream.Write(',');
							}
							i++;
						}
						stream.Write(']');
						break;
					}
				case JsonType.Object:
					{
						stream.Write('{');
						int i = 0;
						foreach (Json child in Children)
						{
							child.Export(stream);
							stream.Write(':');
							child.Children.First().Export(stream);
							if (i != Children.Count - 1)
							{
								stream.Write(',');
							}
							i++;
						}
						stream.Write('}');
						break;
					}
				case JsonType.String:
					stream.Write('"' + Data + '"');
					break;
				default:
					stream.Write(Data);
					break;
			}
		}
		public Json(string str)
		{
			Type = JsonType.String;
			Data = str;
		}
		public Json(int number)
		{
			Type = JsonType.Number;
			Data = number.ToString();
		}
		public Json(double number)
		{
			Type = JsonType.Number;
			Data = number.ToString();
		}
		public Json(bool boolean)
		{
			Type = JsonType.Boolean;
			Data = boolean ? "true" : "false";
		}
		private Json()
		{
			
		}
		public static Json Null
		{
			get
			{
				Json json = new Json();
				json.Type = JsonType.Null;
				json.Data = "null";
				return json;
			}
		}
		public static Json Array
		{
			get
			{
				Json json = new Json();
				json.Type = JsonType.Array;
				json.Children = new List<Json>();
				return json;
			}
		}
		public static Json Object
		{
			get
			{
				Json json = new Json();
				json.Type = JsonType.Object;
				json.Children = new List<Json>();
				return json;
			}
		}
		private string Data { get; set; }
		public JsonType Type { get; private set; }
		private List<Json> Children { get; set; }
		private enum ParsingState
		{
			None,
			String,
			Number,
			True,
			False,
			Null,
			Undefined,
			Object,
			Array,
		}
		public enum JsonType
		{
			Null,
			String,
			Number,
			Boolean,
			Object,
			Array,
		}
		static private Json ParseTree(ref string json, ref int i)
		{
			ParsingState state = ParsingState.None;
			Json root = new Json();
			for (; i < json.Length; i++)
			{
				char ch = json[i];
				if (state == ParsingState.None)
				{
					switch (ch)
					{
						case '"':
							state = ParsingState.String;
							root.Type = JsonType.String;
							root.Data = "";
							break;
						case char c when c >= '0' && c <= '9':
							state = ParsingState.Number;
							root.Type = JsonType.Number;
							root.Data = c.ToString();
							break;
						case 't':
							state = ParsingState.True;
							root.Type = JsonType.Boolean;
							root.Data = ch.ToString();
							break;
						case 'f':
							state = ParsingState.False;
							root.Type = JsonType.Boolean;
							root.Data = ch.ToString();
							break;
						case 'n':
							state = ParsingState.Null;
							root.Type = JsonType.Null;
							root.Data = ch.ToString();
							break;
						case '{':
							state = ParsingState.Object;
							root.Type = JsonType.Object;
							root.Children = new List<Json>();
							break;
						case '[':
							state = ParsingState.Array;
							root.Type = JsonType.Array;
							root.Children = new List<Json>();
							break;
						case ']':
							i++;
							return null;
						case '}':
							i++;
							return null;
					}
				}
				else if (state == ParsingState.String)
				{
					if (ch != '"')
					{
						root.Data += ch;
					}
					else
					{
						i++;
						return root;
					}
				}
				else if (state == ParsingState.Number)
				{
					if (ch >= '0' && ch <= '9' || ch == '.')
					{
						root.Data += ch;
					}
					else
					{
						return root;
					}
				}
				else if (state == ParsingState.Array)
				{
					Json child = ParseTree(ref json, ref i);
					if (child != null)
					{
						root.Children.Add(child);
					}
					else
					{
						return root;
					}
				}
				else if (state == ParsingState.Object)
				{
					Json child = ParseTree(ref json, ref i);
					if (child != null)
					{
						child.Children = new List<Json> { ParseTree(ref json, ref i) };
						root.Children.Add(child);
						
					}
					else
					{
						return root;
					}
				}
				else if (Char.IsLetter(ch))
				{
					root.Data += ch;
				}
				else
				{
					return root;
				}
			}
			return root;
		}
		public bool ContainsKey(string key)
		{
			if (Type != JsonType.Object)
				return false;

			return Children.Any(c => c.Data == key);
		}
		public Json this[string key]
		{
			get
			{
				return Children.First(c => c.Data == key).Children.First();
			}
			set
			{

				Json jsonKey = Children.Find(c => c.Data == key);
				if (jsonKey != null)
				{
					jsonKey.Children[0] = value;
				} else
				{
					jsonKey = new Json(key);
					jsonKey.Children.Add(value);
					Children.Add(jsonKey);
				}
			}
		}
		public Json this[int index]
		{
			get
			{
				return Children.ElementAt(index);
			}
			set
			{
				Children[index] = value;
			}
		}
		public int Count
		{
			get
			{
				if (Type == JsonType.Array || Type == JsonType.Object)
					return Children.Count;
				return 0;
			}
		}
		public void Add(Json json)
		{
			if (Type == JsonType.Array)
			{
				Children.Add(json);
			}
		}
		public static implicit operator Json(int number)
		{
			return new Json(number);
		}
		public static implicit operator Json(double number)
		{
			return new Json(number);
		}
		public static implicit operator Json(string str)
		{
			return new Json(str);
		}
		public static implicit operator Json(bool boolean)
		{
			return new Json(boolean);
		}
		public static implicit operator int(Json json)
		{
			return Int32.Parse(json.Data);
		}
		public static implicit operator double(Json json)
		{
			return Double.Parse(json.Data);
		}
		public static implicit operator string(Json json)
		{
			return json.Data;
		}
		public static implicit operator bool(Json json)
		{
			return Boolean.Parse(json.Data);
		}
	}
}