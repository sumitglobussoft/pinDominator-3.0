
// This file has been generated by the GUI designer. Do not modify.
namespace PinDominator
{
	public partial class frmAddBoard
	{
		private global::Gtk.Fixed fixed1;
		
		private global::Gtk.ScrolledWindow AddBoardLogger;
		
		private global::Gtk.TextView textview1;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Entry txtBoardName;
		
		private global::Gtk.Button btnBrowse;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.CheckButton chkSingleUser_AddBoard;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Entry txtMinDelay_AddBoard;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Entry entry2;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Entry entry3;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget PinDominator.frmAddBoard
			this.Name = "PinDominator.frmAddBoard";
			this.Title = global::Mono.Unix.Catalog.GetString ("frmAddBoard");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.Resizable = false;
			// Container child PinDominator.frmAddBoard.Gtk.Container+ContainerChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.AddBoardLogger = new global::Gtk.ScrolledWindow ();
			this.AddBoardLogger.WidthRequest = 900;
			this.AddBoardLogger.HeightRequest = 140;
			this.AddBoardLogger.Name = "AddBoardLogger";
			this.AddBoardLogger.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child AddBoardLogger.Gtk.Container+ContainerChild
			this.textview1 = new global::Gtk.TextView ();
			this.textview1.CanFocus = true;
			this.textview1.Name = "textview1";
			this.AddBoardLogger.Add (this.textview1);
			this.fixed1.Add (this.AddBoardLogger);
			global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.AddBoardLogger]));
			w2.Y = 416;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("               User Type : ");
			this.fixed1.Add (this.label1);
			global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.label1]));
			w3.X = 240;
			w3.Y = 50;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString (" Enter BoardName : ");
			this.fixed1.Add (this.label2);
			global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.label2]));
			w4.X = 240;
			w4.Y = 100;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.txtBoardName = new global::Gtk.Entry ();
			this.txtBoardName.WidthRequest = 200;
			this.txtBoardName.CanFocus = true;
			this.txtBoardName.Name = "txtBoardName";
			this.txtBoardName.IsEditable = true;
			this.txtBoardName.InvisibleChar = '●';
			this.fixed1.Add (this.txtBoardName);
			global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.txtBoardName]));
			w5.X = 380;
			w5.Y = 95;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.btnBrowse = new global::Gtk.Button ();
			this.btnBrowse.WidthRequest = 100;
			this.btnBrowse.HeightRequest = 26;
			this.btnBrowse.CanFocus = true;
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.UseUnderline = true;
			this.btnBrowse.Label = global::Mono.Unix.Catalog.GetString ("  Browse  ");
			this.fixed1.Add (this.btnBrowse);
			global::Gtk.Fixed.FixedChild w6 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.btnBrowse]));
			w6.X = 600;
			w6.Y = 95;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("eg: Niche:Board Name 1 , Board Name 2");
			this.fixed1.Add (this.label3);
			global::Gtk.Fixed.FixedChild w7 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.label3]));
			w7.X = 380;
			w7.Y = 126;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.chkSingleUser_AddBoard = new global::Gtk.CheckButton ();
			this.chkSingleUser_AddBoard.CanFocus = true;
			this.chkSingleUser_AddBoard.Name = "chkSingleUser_AddBoard";
			this.chkSingleUser_AddBoard.Label = global::Mono.Unix.Catalog.GetString ("For Single User ");
			this.chkSingleUser_AddBoard.DrawIndicator = true;
			this.chkSingleUser_AddBoard.UseUnderline = true;
			this.fixed1.Add (this.chkSingleUser_AddBoard);
			global::Gtk.Fixed.FixedChild w8 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.chkSingleUser_AddBoard]));
			w8.X = 380;
			w8.Y = 50;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("                      Delay : ");
			this.fixed1.Add (this.label4);
			global::Gtk.Fixed.FixedChild w9 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.label4]));
			w9.X = 240;
			w9.Y = 170;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.txtMinDelay_AddBoard = new global::Gtk.Entry ();
			this.txtMinDelay_AddBoard.WidthRequest = 60;
			this.txtMinDelay_AddBoard.CanFocus = true;
			this.txtMinDelay_AddBoard.Name = "txtMinDelay_AddBoard";
			this.txtMinDelay_AddBoard.Text = global::Mono.Unix.Catalog.GetString ("30");
			this.txtMinDelay_AddBoard.IsEditable = true;
			this.txtMinDelay_AddBoard.InvisibleChar = '●';
			this.fixed1.Add (this.txtMinDelay_AddBoard);
			global::Gtk.Fixed.FixedChild w10 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.txtMinDelay_AddBoard]));
			w10.X = 380;
			w10.Y = 165;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString (" To ");
			this.fixed1.Add (this.label5);
			global::Gtk.Fixed.FixedChild w11 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.label5]));
			w11.X = 557;
			w11.Y = 167;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.entry2 = new global::Gtk.Entry ();
			this.entry2.CanFocus = true;
			this.entry2.Name = "entry2";
			this.entry2.IsEditable = true;
			this.entry2.InvisibleChar = '●';
			this.fixed1.Add (this.entry2);
			global::Gtk.Fixed.FixedChild w12 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.entry2]));
			w12.X = 630;
			w12.Y = 155;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("label3");
			this.fixed1.Add (this.label6);
			global::Gtk.Fixed.FixedChild w13 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.label6]));
			w13.X = 263;
			w13.Y = 226;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.entry3 = new global::Gtk.Entry ();
			this.entry3.CanFocus = true;
			this.entry3.Name = "entry3";
			this.entry3.IsEditable = true;
			this.entry3.InvisibleChar = '●';
			this.fixed1.Add (this.entry3);
			global::Gtk.Fixed.FixedChild w14 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.entry3]));
			w14.X = 355;
			w14.Y = 218;
			this.Add (this.fixed1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 901;
			this.DefaultHeight = 556;
			this.Show ();
		}
	}
}
