using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EagleEye.Models;
namespace EagleEye.Tests
{
	public static class Tests
	{
		public static void RunTests() {
			EagleEye.Models.Vector2 vector = new Models.Vector2(1, 0);
			JsonImport();
		}

		static void JsonImport()
		{
			Json json = Json.Import(new System.IO.StreamReader(HttpRuntime.AppDomainAppPath + "/App_Data/test.json"));
			if (json.Type != Models.Json.JsonType.Object)
			{
				throw new Exception("Json did not parse root as Object type");
			} else
			{
				if (!json.ContainsKey("test"))
				{
					throw new Exception("Json did not contain the correct key");
				} else
				{
					Json value = json["test"];
					if (value == null)
					{
						throw new Exception();
					} else
					{
						if (value.Count != 5)
						{
							throw new Exception();
						}
					}
				}
			}
			using (System.IO.StreamWriter stream = new System.IO.StreamWriter(HttpRuntime.AppDomainAppPath + "/App_Data/test_export.json")) {
				json.Export(stream);
			}
		}
	}
}