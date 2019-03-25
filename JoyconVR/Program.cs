using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Joycon4CS;

namespace JoyconVR
{
	internal class Program
	{
		private static JoyconManager _joyconManager;

		private static readonly CancellationTokenSource CancelSource = new CancellationTokenSource();

		private static readonly CancellationToken Cancel = CancelSource.Token;

		private static Vector3 _lPos = new Vector3(1, -1, 0);
		private static Vector3 _rPos = new Vector3(-1, -1, 0);
		private static Vector3 _lAcc = Vector3.Zero;
		private static Vector3 _rAcc = Vector3.Zero;
		private static Vector3 _lRot = Vector3.Zero;
		private static Vector3 _rRot = Vector3.Zero;

		private static Process _cmdPro;

		private static ProcessStartInfo _startInfo;


		public static void Main()
		{
#pragma warning disable 4014
			MainAsync();
#pragma warning restore 4014
		}

		private static async Task MainAsync()
		{
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

			_startInfo = new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = false,
				RedirectStandardError = false,
				FileName = "CMD.exe"
			};

			_cmdPro = new Process {StartInfo = _startInfo};
			_cmdPro.Start();
			Console.SetOut(_cmdPro.StandardInput);

			_joyconManager = new JoyconManager();


			Console.WriteLine("left.bat");
			Console.WriteLine("right.bat");

			await ScanAsync();
			await LoopAsync(TimeSpan.MinValue, Cancel);
		}


		private static async Task LoopAsync(TimeSpan interval, CancellationToken cancellationToken)
		{
			while (true)
			{
				await UpdateAsync();
				await Task.Delay(interval, cancellationToken);
			}

			// ReSharper disable once FunctionNeverReturns
		}

		private static async Task UpdateAsync()
		{
			_joyconManager.Update();

			await UpdateInfoAsync();
			await UpdatePositionDataAsync();
			await UpdateDeviceDataAsync();
		}


		private static async Task UpdatePositionDataAsync()
		{
			_lPos = _lPos + Rotate3D(_lAcc, _lRot);
			_rPos = _rPos + Rotate3D(_rAcc, _rRot);
			await EmptyAsync();
		}

		private static Vector3 Rotate3D(Vector3 point, Vector3 rotation)
		{
			var temp = new Math3D.Math3D.Vector3D(point.X, point.Y, point.Z);
			Math3D.Math3D.RotateX(temp, (float) rotation.X);
			Math3D.Math3D.RotateY(temp, (float) rotation.Y);
			Math3D.Math3D.RotateZ(temp, (float) rotation.Z);
			return new Vector3(temp.x, temp.y, temp.z);
		}


		private static async Task UpdateDeviceDataAsync()
		{
			Console.WriteLine("client_commandline.exe setdeviceposition 0 " + VectorToString(_lPos));
			Console.WriteLine("client_commandline.exe setdeviceposition 1 " + VectorToString(_rPos));
			Console.WriteLine("client_commandline.exe setdevicerotation 0 " + VectorToString(_lRot));
			Console.WriteLine("client_commandline.exe setdevicerotation 1 " + VectorToString(_rRot));
			await EmptyAsync();
		}


		private static string VectorToString(Vector3 v)
		{
			return v.X + " " + v.Y + " " + v.Z;
		}

		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			CancelSource.Cancel();
			_joyconManager.OnApplicationQuit();
		}

		private static async Task ScanAsync()
		{
			_joyconManager.Scan();

			UpdateDebugType();
			await UpdateInfoAsync();
		}

		private static void UpdateDebugType()
		{
			foreach (var j in _joyconManager.j)
				j.debug_type = Joycon.DebugType.NONE;
		}


		private static async Task UpdateInfoAsync()
		{
			if (_joyconManager.j.Count > 0)
			{
				var j = _joyconManager.j[0];

				_lRot.X = (float) (j.GetVector().eulerAngles.Y * 180.0f / Math.PI);
				_lRot.Y = (float) (j.GetVector().eulerAngles.Z * 180.0f / Math.PI);
				_lRot.Z = (float) (j.GetVector().eulerAngles.X * 180.0f / Math.PI);

				_lAcc.X = (float) j.GetAccel().X;
				_lAcc.Y = (float) j.GetAccel().Y;
				_lAcc.Z = (float) j.GetAccel().Z;
			}

			if (_joyconManager.j.Count > 1)
			{
				var j = _joyconManager.j[1];

				_rRot.X = (float) (j.GetVector().eulerAngles.Y * 180.0f / Math.PI);
				_rRot.Y = (float) (j.GetVector().eulerAngles.Z * 180.0f / Math.PI);
				_rRot.Z = (float) (j.GetVector().eulerAngles.X * 180.0f / Math.PI);

				_rAcc.X = (float) j.GetAccel().X;
				_rAcc.Y = (float) j.GetAccel().Y;
				_rAcc.Z = (float) j.GetAccel().Z;
			}

			await EmptyAsync();
		}

#pragma warning disable 1998
		private static async Task EmptyAsync()
		{
		}
#pragma warning restore 1998
	}
}