using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using EagleEye.Models;
using System.Drawing;
namespace EagleEye
{
	public class EagleEyeConfig
	{
		private static string DatabasePath = $"{HttpRuntime.AppDomainAppPath}/App_Data/database.json";
		public static void ImportDatabase()
		{
			using (var stream = new StreamReader(DatabasePath))
			{
				Json database = Json.Import(stream);
				Json cameras = database["Cameras"];
				for (int i = 0; i < cameras.Count; i++)
				{
					Json cameraJson = cameras[i];
					Camera camera = new Camera(cameraJson["ID"], cameraJson["Name"]);
					Repository<Camera>.Add(camera);
				}
				Json parkingLots = database["ParkingLots"];
				for (int i = 0; i < parkingLots.Count; i++)
				{
					Json lotJson = parkingLots[i];
					ParkingLot lot = new ParkingLot(lotJson["ID"], lotJson["Name"], Repository<Camera>.Get(lotJson["Camera"]), Bitmap.FromFile($"{HttpRuntime.AppDomainAppPath}App_Data\\{(int)lotJson["ID"]}.bmp") as Bitmap);
					for (int a = 0; a < parkingLots["Annotations"].Count; a++)
					{
						Json ann = parkingLots["Annotations"][a];
						Annotation annotation = new Annotation(ann["ID"],(Annotation.AnnotationType)(int)ann["Type"]);
						for (int p = 0; p < ann["Points"].Count; p++)
						{
							Json point = ann["Points"][p];
							Vector2 pnt = new Vector2(point["X"], point["Y"]);
							annotation.Add(pnt);
						}
						lot.Annotations.Add(annotation);
					}
					Repository<ParkingLot>.Add(lot);
				}
			}
		}
		public static void ExportDatabase()
		{
			Json database = Json.Object;
			Json cameras = Json.Array;
			foreach (Camera camera in Repository<Camera>.Models.Values)
			{
				Json cameraJson = Json.Object;
				cameraJson["ID"] = camera.ID;
				cameraJson["Name"] = camera.Name;
				cameras.Add(cameraJson);
			}
			database["Cameras"].Add(cameras);
			Json lots = Json.Array;
			foreach (ParkingLot lot in Repository<ParkingLot>.Models.Values)
			{
				Json lotJson = Json.Object;
				lotJson["ID"] = lot.ID;
				lotJson["Name"] = lot.Name;
				lotJson["Camera"] = lot.Camera.ID;
				Json annotations = Json.Array;
				foreach (var annotation in lot.Annotations)
				{
					Json ann = Json.Object;
					ann["ID"] = annotation.ID;
					ann["Type"] = (int)annotation.Type;
					Json points = Json.Array;
					foreach (var point in annotation.Points)
					{
						Json p = Json.Object;
						p["X"] = point.X;
						p["Y"] = point.Y;
						points.Add(p);
					}
					ann["Points"] = points;
					annotations.Add(ann);
				}
				lotJson["Annotations"] = annotations;
				lot.Baseline.Save($"{HttpRuntime.AppDomainAppPath}/App_Data/{lot.ID}.bmp");
			}
			database["ParkingLots"] = lots;
			using (StreamWriter stream = new StreamWriter(DatabasePath))
			{
				database.Export(stream);
			}
		}
	}
}