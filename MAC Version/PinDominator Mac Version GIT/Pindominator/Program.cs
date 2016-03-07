using System;
using Gtk;
using PinDominator;

namespace PinDominator
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			frmLicensing win = new frmLicensing ();
			//MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
