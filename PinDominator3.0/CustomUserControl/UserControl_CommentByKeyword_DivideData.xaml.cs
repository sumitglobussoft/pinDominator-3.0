using BaseLib;
using CommentManager;
using FirstFloor.ModernUI.Windows.Controls;
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

namespace PinDominator.CustomUserControl
{
    /// <summary>
    /// Interaction logic for UserControl_CommentByKeyword_DivideData.xaml
    /// </summary>
    public partial class UserControl_CommentByKeyword_DivideData : UserControl
    {
        public UserControl_CommentByKeyword_DivideData()
        {
            InitializeComponent();
        }

        CommentByKeywordManager objComment = new CommentByKeywordManager();

        private void rdbDivideEqually_CommentByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CommentByKeywordManager.rdbDivideEquallyCommentByKeyword = true;
                rdbDivideByUser_CommentByKeyword.IsEnabled = false;
                txtCountGivenByUser_CommentByKeyword.Visibility = Visibility.Hidden;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbDivideByUser_CommentByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CommentByKeywordManager.rdbDivideGivenByUserCommentByKeyword = true;
                rdbDivideEqually_CommentByKeyword.IsEnabled = false;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnSave_CommentByKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CommentByKeywordManager.rdbDivideGivenByUserCommentByKeyword == true)
                {
                    if (!string.IsNullOrEmpty(txtCountGivenByUser_CommentByKeyword.Text))
                    {
                        CommentByKeywordManager.CountGivenByUserCommentByKeyword = Convert.ToInt32(txtCountGivenByUser_CommentByKeyword.Text);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Please Give Count Given By User");
                        ModernDialog.ShowMessage("Please Give Count Given By User", "Count Given By User", MessageBoxButton.OK);
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }






    }
}
