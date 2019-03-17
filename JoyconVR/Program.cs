using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using SharpJoycon;
using SharpDX.DirectInput;
using SharpJoycon.Interfaces;
using SharpJoycon.Interfaces.Joystick.Controllers;
using Innovative.Geometry;

namespace Joyconlib
{
	internal class Program
	{
		public static List<NintendoController> Nintdev;


		public static NintendoController rightCon;

		public static NintendoController leftCon;


		private static Vector3 lPos;
		private static Vector3 rPos;
		private static Vector3 lAcc;
		private static Vector3 rAcc;
		private static Matrix4x4 lRot;
		private static Matrix4x4 rRot;
		private static Vector3 lRot2;
		private static Vector3 rRot2;
		static Process cmd = new Process();

		public static void Main(string[] args)
		{
			MainAsync(args);
		}

		public static async Task MainAsync(string[] args)
		{
			lPos = Vector3.Zero;
			lRot2 = Vector3.Zero;
			rPos = Vector3.Zero;
			rRot2 = Vector3.Zero;
			
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.CreateNoWindow = false;
			cmd.StartInfo.UseShellExecute = false;
			cmd.Start();
			
			Nintdev = NintendoController.Discover();
			foreach (var controller in Nintdev)
			{
				if (controller.GetController().GetJoystick().GetType() == typeof(LeftJoycon))
					leftCon = controller;
				if (controller.GetController().GetJoystick().GetType() == typeof(RightJoycon))
					rightCon = controller;

			}

			await cmd.StandardInput.WriteLineAsync("cd \"C:/Program Files/OpenVR-InputEmulator\"");
			await cmd.StandardInput.WriteLineAsync("\"D:/Projects/Joyconlib/Joyconlib/left.bat\"");
			await cmd.StandardInput.WriteLineAsync("\"D:/Projects/Joyconlib/Joyconlib/right.bat\"");
			await cmd.StandardInput.FlushAsync();

			await LoopAsync();
		}

		public static async Task LoopAsync()
		{
			await UpdateDataAsync();
			await Task.Delay(-1);
		}

		public static async Task UpdateSensorDataAsync()
		{
			lAcc = new Vector3(
				leftCon.GetIMU().GetData().xAcc, 
				leftCon.GetIMU().GetData().yAcc,
				leftCon.GetIMU().GetData().zAcc
			);
			rAcc = new Vector3(
				rightCon.GetIMU().GetData().xAcc, 
				rightCon.GetIMU().GetData().yAcc,
				rightCon.GetIMU().GetData().zAcc
			);
			lRot2 = new Vector3(
				leftCon.GetIMU().GetData().xGyro, 
				leftCon.GetIMU().GetData().yGyro, 
				leftCon.GetIMU().GetData().zGyro
			);
			lRot2 = new Vector3(
				rightCon.GetIMU().GetData().xGyro, 
				rightCon.GetIMU().GetData().yGyro, 
				rightCon.GetIMU().GetData().zGyro
			);
		}

		public static async Task UpdatePositionDataAsync()
		{
			lPos = Vector3.Add(lPos, Vector3.Transform(lAcc, lRot));
			rPos = Vector3.Add(rPos, Vector3.Transform(rAcc, rRot));
		}

		public static async Task UpdateMatrixAsync()
		{
			lRot = Matrix4x4.CreateFromYawPitchRoll(
				(float) new Angle(Convert.ToDouble(lRot2.Y)).Radians,
				(float) new Angle(Convert.ToDouble(lRot2.X)).Radians,
				(float) new Angle(Convert.ToDouble(lRot2.Z)).Radians
			);
			rRot = Matrix4x4.CreateFromYawPitchRoll(
				(float) new Angle(Convert.ToDouble(rRot2.Y)).Radians,
				(float) new Angle(Convert.ToDouble(rRot2.X)).Radians,
				(float) new Angle(Convert.ToDouble(rRot2.Z)).Radians
			);
		}

		public static async Task UpdateDeviceDataAsync()
		{
			await cmd.StandardInput.WriteLineAsync(
				String.Format("client_commandline.exe setdeviceposition 0 {0} {1} {2}", lPos.X, lPos.Y, lPos.Z));
			await cmd.StandardInput.WriteLineAsync(
				String.Format("client_commandline.exe setdeviceposition 1 {0} {1} {2}", rPos.X, rPos.Y, rPos.Z));
			await cmd.StandardInput.WriteLineAsync(
				String.Format("client_commandline.exe setdevicerotation 0 {0} {1} {2}", lRot2.X, lRot2.Y, lRot2.Z));
			await cmd.StandardInput.WriteLineAsync(
				String.Format("client_commandline.exe setdevicerotation 1 {0} {1} {2}", rRot2.X, rRot2.Y, rRot2.Z));
			await cmd.StandardInput.FlushAsync();
		}

		public static async Task UpdateDataAsync()
		{
			await UpdateMatrixAsync();
			await UpdatePositionDataAsync();
			await UpdateSensorDataAsync();
			await UpdateDeviceDataAsync();
		}
	}
}