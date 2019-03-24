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

namespace JoyconVR
{
	internal class Program
	{
		public static List<NintendoController> Nintdev;


		public static NintendoController rightCon;

		public static NintendoController leftCon;


		public static Vector3 LPos = new Vector3(1,-1,0);
		public static Vector3 RPos = new Vector3(-1,-1,0);
		public static Vector3 LAcc = Vector3.Zero;
		public static Vector3 RAcc = Vector3.Zero;
		public static Matrix4x4 LRot;
		public static Matrix4x4 RRot;
		public static Vector3 LRot2 = Vector3.Zero;
		public static Vector3 RRot2 = Vector3.Zero;

		public static Process CmdPro;

		public static ProcessStartInfo startInfo;

		public static void Main(string[] args)
		{
			Process CmdPro;
			ProcessStartInfo startInfo;
			startInfo = new ProcessStartInfo();
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardInput = true;
			startInfo.RedirectStandardOutput = false;
			startInfo.RedirectStandardError = false;
			startInfo.FileName = "CMD.exe";

			CmdPro = new Process();
			CmdPro.StartInfo = startInfo;
			CmdPro.Start();
			Console.SetOut(CmdPro.StandardInput);
			
			Nintdev = NintendoController.Discover();
			foreach (var controller in Nintdev)
			{
				if (controller.GetController().GetJoystick().GetType() == typeof(LeftJoycon))
					leftCon = controller;
				if (controller.GetController().GetJoystick().GetType() == typeof(RightJoycon))
					rightCon = controller;

			}
			Console.WriteLine("left.bat");
			Console.WriteLine("right.bat");

			leftCon.GetIMU().
			Loop();
		}
		public static void Loop()
		{
			while (true)
			{
				UpdateData();
				
			}
		}

		public static void UpdateSensorData()
		{
			LAcc = new Vector3(
				leftCon.GetIMU().GetData().xAcc, 
				leftCon.GetIMU().GetData().yAcc,
				leftCon.GetIMU().GetData().zAcc
			);
			RAcc = new Vector3(
				rightCon.GetIMU().GetData().xAcc, 
				rightCon.GetIMU().GetData().yAcc,
				rightCon.GetIMU().GetData().zAcc
			);
			LRot2 = new Vector3(
				leftCon.GetIMU().GetData().xGyro, 
				leftCon.GetIMU().GetData().yGyro, 
				leftCon.GetIMU().GetData().zGyro
			);
			LRot2 = new Vector3(
				rightCon.GetIMU().GetData().xGyro, 
				rightCon.GetIMU().GetData().yGyro, 
				rightCon.GetIMU().GetData().zGyro
			);
		}

		public static void UpdatePositionData()
		{
			LPos = Vector3.Add(LPos, Vector3.Transform(Vector3.Multiply(LAcc,1f), LRot));
			RPos = Vector3.Add(RPos, Vector3.Transform(Vector3.Multiply(RAcc,1f), RRot));
		}

		public static void  UpdateMatrix()
		{
			LRot = Matrix4x4.CreateFromYawPitchRoll(
				(float) new Angle(Convert.ToDouble(LRot2.Y)).Radians,
				(float) new Angle(Convert.ToDouble(LRot2.X)).Radians,
				(float) new Angle(Convert.ToDouble(LRot2.Z)).Radians
			);
			RRot = Matrix4x4.CreateFromYawPitchRoll(
				(float) new Angle(Convert.ToDouble(RRot2.Y)).Radians,
				(float) new Angle(Convert.ToDouble(RRot2.X)).Radians,
				(float) new Angle(Convert.ToDouble(RRot2.Z)).Radians
			);
		}

		public static void UpdateDeviceData()
		{
			
			Console.WriteLine("client_commandline.exe setdeviceposition 0 " + VectorToString(LPos));
			Console.WriteLine("client_commandline.exe setdeviceposition 1 " + VectorToString(RPos));
			Console.WriteLine("client_commandline.exe setdevicerotation 0 " + VectorToString(LRot2));
			Console.WriteLine("client_commandline.exe setdevicerotation 1 " +  VectorToString(RRot2));
		}

		public static void UpdateData()
		{
			//UpdateMatrix();
			//UpdatePositionData();
			leftCon.Poll();
			rightCon.Poll();
			UpdateSensorData();
			UpdateDeviceData();
		}

		public static string VectorToString(Vector3 v)
		{
			return v.X + " " + v.Y + " " + v.Z;
		}
	}
}