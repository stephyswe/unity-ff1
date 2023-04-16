using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace Tests.InputControl {

	public abstract class Mouse {
		
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern bool SetCursorPos(int X, int Y);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern void mouse_event(MouseEventFlags dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
		
		// make SetCursorPos public
		public static void Point(int X, int Y)
		{
			SetCursorPos(X, Y);
		}
		
		public static void LeftMouseClick(int xpos, int ypos)
		{
			SetCursorPos(xpos, ypos);
			mouse_event(MouseEventFlags.Leftdown, xpos, ypos, 0, 0);
			mouse_event(MouseEventFlags.Leftup, xpos, ypos, 0, 0);
		}
		
		static void ClickObject(string objName) {
			GameObject obj = GameObject.Find(objName) ?? throw new Exception($"Cannot find object with name {objName}");
			(int xValue, int yValue) = GetBothPos(obj);
			LeftMouseClick(xValue, yValue);
		}

		[Flags]
		enum MouseEventFlags : uint
		{
			Leftdown = 0x00000002,
			Leftup = 0x00000004,
			Middledown = 0x00000020,
			Middleup = 0x00000040,
			Move = 0x00000001,
			Absolute = 0x00008000,
			Rightdown = 0x00000008,
			Rightup = 0x00000010
		}

		// ReSharper disable once UnusedMethodReturnValue.Global
		public static IEnumerable<WaitForSeconds> MultipleClicks(IEnumerable<int[]> positions)
		{
			Point[] points = positions.Select(pos => new Point(pos[0], pos[1])).ToArray();
			foreach (Point point in points)
			{
				LeftMouseClick(point.X, point.Y);
				yield return new WaitForSeconds(1);
			}
		}

		static (int x, int y) GetBothPos(GameObject obj) {
			Vector3 position = obj.transform.localPosition;
			int x = GetXPosOnScreen(position.x, -1293, 985);
			int y = GetXPosOnScreen(position.y, -730, 420);
			return (x, y);
		}
		static int GetXPosOnScreen(float posValue, int oGt, int oGn) {
			const int diff = 47;
			float diff1 = Math.Abs(oGt - posValue);
			double diff2 = diff1 * diff;
			double diff3 = diff2 + oGn;

			int x = (int)diff3;
			return x;
		}
	}
}
