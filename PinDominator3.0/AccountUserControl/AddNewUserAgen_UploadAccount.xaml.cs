using BaseLib;
using BasePD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PinDominator.AccountUserControl
{
    /// <summary>
    /// Interaction logic for AddNewUserAgen_UploadAccount.xaml
    /// </summary>
    public partial class AddNewUserAgen_UploadAccount : UserControl
    {
        public AddNewUserAgen_UploadAccount()
        {
            InitializeComponent();
            BindAccount();
        }
        private void BindAccount()
        {
            try
            {
                cmbSelectAccount.Items.Clear();
                if (PDGlobals.listAccounts.Count > 0)
                {
                    foreach (var item in PDGlobals.listAccounts)
                    {
                        cmbSelectAccount.Items.Add(item.Split(':')[0]);
                    }

                }
                else
                {
                    //
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
    }
}
