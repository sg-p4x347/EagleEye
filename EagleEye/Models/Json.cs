using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
namespace EagleEye.Models
{
	/// <summary>
	//Developer:
	//	Gage Coates

	//Purpose:
	//	Parses a json formated stream of characters into
	//	an tree structure of Json instances

	//Notes:
	//	Every node in the tree, including primives, are
	//	stored as Json instances differentiated by
	//	the Type property
	/// </summary>
	public class Json
	{
		/// <summary>
		//Purpose:
		//	Parses a stream into a Json tree

		//Returns:
		//	A Json object representing the entire stream
		/// </summary>
		static public Json Import(StreamReader stream) {
			int i = 0;
			string json = stream.ReadToEnd();
			return ParseTree(ref json, ref i);
		}
		/// <summary>
		//Purpose:
		//	Serializes the Json tree to a stream
		/// </summary>
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
		// Creates a new Array instace
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
		// Creates a new Object instance
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
		// The raw data associated with this Json node
		// This is only used for primitive types
		private string Data { get; set; }
		// Differentiates Json instance types
		public JsonType Type { get; private set; }
		// The child instances, primitives do not use this
		private List<Json> Children { get; set; }
		// Defines different parsing states
		private enum ParsingState
		{

			None,
			//--------------------------------------------------
			// Primitive types (has a value for Data)
			String,
			Number,
			True,
			False,
			Null,
			Undefined,
			//--------------------------------------------------
			// Complex types (contains children)
			Object,
			Array,
		}
		public enum JsonType
		{
			//--------------------------------------------------
			// Primitive types (has a value for Data)
			Null,
			String,
			Number,
			Boolean,
			//--------------------------------------------------
			// Complex types (contains children)
			Object,
			Array,
		}
		/// <summary>
		//Purpose:
		//	Recursively parses the input string as a Json node starting at
		//	index i

		//Returns:
		//	A Json node
		/// </summary>
		static private Json ParseTree(ref string json, ref int i)
		{
			ParsingState state = ParsingState.None;
			Json root = new Json();
			for (; i < json.Length;)
			{
				char ch = json[i];
				if (state == ParsingState.None)
				{
					i++;
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
							return null;
						case '}':
							return null;
					}
				}
				else if (state == ParsingState.String)
				{
					i++;
					if (ch != '"')
					{
						root.Data += ch;
					}
					else
					{
						return root;
					}
				}
				else if (state == ParsingState.Number)
				{
					if (ch >= '0' && ch <= '9' || ch == '.')
					{
						root.Data += ch;
						i++;
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
					i++;
				}
				else
				{
					return root;
				}
			}
			return root;
		}
		/// <summary>
		//Purpose:
		//	Tests whether this Object instance contains a key

		//Returns:
		//	true if the key is present, else false
		/// </summary>
		public bool ContainsKey(string key)
		{
			if (Type != JsonType.Object)
				return false;

			return Children.Any(c => c.Data == key);
		}
		/// <summary>
		//Purpose:
		//	Returns the value assocated with the given key
		//	if this is an Object

		//Returns:
		//	A Json node if the value is found, 
		//	else a null (not a Null instance)
		/// </summary>
		public Json this[string key]
		{
			get
			{
				if (Type == JsonType.Object)
					return Children.First(c => c.Data == key).Children.First();
				return null;
			}
			set
			{
				if (Type == JsonType.Object)
				{
					Json jsonKey = Children.Find(c => c.Data == key);
					if (jsonKey != null)
					{
						jsonKey.Children[0] = value;
					}
					else
					{
						jsonKey = new Json(key);
						jsonKey.Children = new List<Json> { value };
						Children.Add(jsonKey);
					}
				}
			}
		}
		/// <summary>
		//Purpose:
		//	Returns the value at the given index if this is an
		//	Array

		//Returns:
		//	A Json node if found, else null (not a Null instance)
		/// </summary>
		public Json this[int index]
		{
			get
			{
				if (Type == JsonType.Array)
					return Children.ElementAt(index);
				return null;
			}
			set
			{
				if (Type == JsonType.Array)
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
		/// <summary>
		//Purpose:
		//	Adds the Json node to an Array
		/// </summary>
		public void Add(Json json)
		{
			if (Type == JsonType.Array)
			{
				Children.Add(json);
			}
		}
		//--------------------------------------------------
		// Implict conversions from .NET primitives 
		// to Json primitives
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
		//--------------------------------------------------
		// Implict conversions from Json primitives
		// to .NET primitives
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