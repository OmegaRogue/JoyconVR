using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Innovative.Geometry;
using Joycon4CS;

using Math3D;
using Vector3 = Joycon4CS.Vector3;

namespace JoyconVR
{
	internal class Program
	{
		public static JoyconManager joyconManager;

		public static CancellationTokenSource cancelSource = new CancellationTokenSource();
		
		public static CancellationToken cancellation = cancelSource.Token;

		public static Vector3 LPos = new Vector3(1,-1,0);
		public static Vector3 RPos = new Vector3(-1,-1,0);
		public static Vector3 LAcc = Vector3.Zero;
		public static Vector3 RAcc = Vector3.Zero;
		public static Vector3 LRot = Vector3.Zero;
		public static Vector3 RRot = Vector3.Zero;

		public static Process CmdPro;

		public static ProcessStartInfo startInfo;
		
		private static int _heartbeatCount = 0;

		public static void Main(string[] args)
		{
			MainAsync(args);
			
			
		}

		public static async Task MainAsync(string[] args)
		{
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
			
			Process CmdPro;
			ProcessStartInfo startInfo;
			startInfo = new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = false,
				RedirectStandardError = false,
				FileName = "CMD.exe"
			};

			CmdPro = new Process {StartInfo = startInfo};
			CmdPro.Start();
			Console.SetOut(CmdPro.StandardInput);
			
			joyconManager = new JoyconManager();
			
			
			
			Console.WriteLine("left.bat");
			Console.WriteLine("right.bat");

			await ScanAsync();
			await LoopAsync(TimeSpan.MinValue, cancellation);
		}
		
		
		public static async Task LoopAsync(TimeSpan interval, CancellationToken cancellationToken)
		{
			while (true)
			{
				await UpdateAsync();
				await Task.Delay(interval, cancellationToken);
			}
		}

		private static async Task UpdateAsync()
		{
			joyconManager.Update();

			await UpdateInfoAsync();
			await UpdatePositionDataAsync();
			await UpdateDeviceDataAsync();
		}


		private static async Task UpdatePositionDataAsync()
		{
			LPos = LPos + Rotate3D(LAcc, LRot);
			RPos = RPos + Rotate3D(RAcc, RRot);
		}

		private static Vector3 Rotate3D(Vector3 point, Vector3 rotation)
		{
			Math3D.Math3D.Vector3D temp = new Math3D.Math3D.Vector3D(point.X,point.Y,point.Z);
			Math3D.Math3D.RotateX(temp, (float) rotation.X);
			Math3D.Math3D.RotateY(temp, (float) rotation.Y);
			Math3D.Math3D.RotateZ(temp, (float) rotation.Z);
			return new Vector3(temp.x,temp.y,temp.z);
		}


		private static async Task UpdateDeviceDataAsync()
		{
			Console.WriteLine("client_commandline.exe setdeviceposition 0 " + VectorToString(LPos));
			Console.WriteLine("client_commandline.exe setdeviceposition 1 " + VectorToString(RPos));
			Console.WriteLine("client_commandline.exe setdevicerotation 0 " + VectorToString(LRot));
			Console.WriteLine("client_commandline.exe setdevicerotation 1 " +  VectorToString(RRot));
		}


		private static string VectorToString(Vector3 v)
		{
			return v.X + " " + v.Y + " " + v.Z;
		}

		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			cancelSource.Cancel();
			joyconManager.OnApplicationQuit();
		}
		private static async Task ScanAsync()
		{
			joyconManager.Scan();

			UpdateDebugType();
			await UpdateInfoAsync();

		}

		private static void UpdateDebugType()
		{
			foreach (var j in joyconManager.j)
				j.debug_type = Joycon.DebugType.NONE;
		}






		private static async Task UpdateInfoAsync()
		{
			if (joyconManager.j.Count > 0)
			{
				var j = joyconManager.j[0];

				LRot.X = (float)(j.GetVector().eulerAngles.Y * 180.0f / Math.PI);
				LRot.Y = (float)(j.GetVector().eulerAngles.Z * 180.0f / Math.PI);
				LRot.Z = (float)(j.GetVector().eulerAngles.X * 180.0f / Math.PI);

				LAcc.X = (float) j.GetAccel().X;
				LAcc.Y = (float) j.GetAccel().Y;
				LAcc.Z = (float) j.GetAccel().Z;

			}

			if (joyconManager.j.Count > 1)
			{
				var j = joyconManager.j[1];
				
				RRot.X = (float)(j.GetVector().eulerAngles.Y * 180.0f / Math.PI);
				RRot.Y = (float)(j.GetVector().eulerAngles.Z * 180.0f / Math.PI);
				RRot.Z = (float)(j.GetVector().eulerAngles.X * 180.0f / Math.PI);
				
				RAcc.X = (float) j.GetAccel().X;
				RAcc.Y = (float) j.GetAccel().Y;
				RAcc.Z = (float) j.GetAccel().Z;
			}



		}

		
	}
	
}