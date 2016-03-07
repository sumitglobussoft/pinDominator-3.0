using System;
using BaseLib;
using Gtk;

namespace PinDominator
{
	public partial class frmUploadUnFollowList : Gtk.Window
	{
		public frmUploadUnFollowList () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		protected void OnBtnUnFollowlstBrowseUnFollowClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseUnFollowList();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}


		private void BrowseUnFollowList()
		{
			try
			{
				ClGlobul.lstUploadUnFollowList.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtUnFollowList_UnFollow.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstUploadUnFollowList = GlobusFileHelper.ReadFile(txtUnFollowList_UnFollow.Text.Trim());
			        GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Total UnFollow List Uploaded :" + ClGlobul.lstUploadUnFollowList.Count + " ]");	

				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnSaveUnFollowListClicked (object sender, EventArgs e)
		{
			try
			{
				SaveUnFollowList();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void SaveUnFollowList()
		{
			try
			{
				if (string.IsNullOrEmpty(txtUnFollowList_UnFollow.Text)) 
				{					
					MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "Please Upload UnFollow List ");
					ResponseType response = (ResponseType)md.Run ();
					md.Destroy ();
					return;
				}
				try
				{
					this.Destroy();

				}
				catch{
				};
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
	}
}

