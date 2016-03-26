using BaseLib;
using FollowManagers;
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
    /// Interaction logic for UserControl_FollowByKeyword.xaml
    /// </summary>
    public partial class UserControl_FollowByKeyword : UserControl
    {
        public UserControl_FollowByKeyword()
        {
            InitializeComponent();
            bindModuleonCombobox();
        }

        FollowByKeywordManager objFollowByKeywordManager = new FollowByKeywordManager();

        public void bindModuleonCombobox()
        {
            try
            {
                List<CheckBox> lstofModule = new List<CheckBox>();
                lstofModule.Add(new CheckBox() { Content = "Board" });
                lstofModule.Add(new CheckBox() { Content = "Pin" });
                lstofModule.Add(new CheckBox() { Content = "Like" });
                lstofModule.Add(new CheckBox() { Content = "Follower" });
                lstofModule.Add(new CheckBox() { Content = "Following" });

                //cmbBox_Follow_FolloWByKeyword_CertainFollowerSetting.Dispatcher.Invoke(new Action(delegate
                //{

                //    cmbBox_Follow_FolloWByKeyword_CertainFollowerSetting.ItemsSource = lstofModule;
                //}));
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==> " + ex.Message);
            }
        }
        private void cmbBox_Follow_FolloWByKeyword_CertainFollowerSetting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void cmbBox_Follow_FolloWByKeyword_CertainFollowerSetting_DropDownOpened(object sender, EventArgs e)
        {
            
        }

        private void btnSave_FollowByKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Commented Code
                //foreach (CheckBox item in cmbBox_Follow_FolloWByKeyword_CertainFollowerSetting.Items)
                //{
                //    try
                //    {                       
                //        if(item.IsChecked==true)
                //        {
                //            string selectedItem = item.Content.ToString();
                //            ClGlobul.lstCmboBox_FollowByKeyword.Add(selectedItem);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        GlobusLogHelper.log.Error("Error ==>" + ex.Message);
                //    }                   
                //}
                //GlobusLogHelper.log.Info("Total Selected Item Is : " + ClGlobul.lstCmboBox_FollowByKeyword.Count);

                //objFollowByKeywordManager.MinValue = Convert.ToInt32(txtMinValue_FollowByKeyword.Text);
                //objFollowByKeywordManager.MinValue = Convert.ToInt32(txtMaxValue_FollowByKeyword.Text);
                #endregion

                //objFollowByKeywordManager.countBoard = Convert.ToInt32(txtCount_Board.Text);
                //objFollowByKeywordManager.countPin = Convert.ToInt32(txtCount_Pin.Text);
                //objFollowByKeywordManager.countLike = Convert.ToInt32(txtCount_Like.Text);
                //objFollowByKeywordManager.countFollower = Convert.ToInt32(txtCount_Follower.Text);
                //objFollowByKeywordManager.countFollowing = Convert.ToInt32(txtCount_Following.Text);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkBoard_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkBoard.IsChecked == true)
                {
                    objFollowByKeywordManager.chkBoard = true;
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkBoard_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                objFollowByKeywordManager.chkBoard = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkPin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkPin.IsChecked == true)
                {
                    objFollowByKeywordManager.chkPin = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkPin_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                objFollowByKeywordManager.chkPin = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkLike_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkLike.IsChecked == true)
                {
                    objFollowByKeywordManager.chkLike = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkLike_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                objFollowByKeywordManager.chkLike = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkFollower_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkFollower.IsChecked == true)
                {
                    objFollowByKeywordManager.chkFollower = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkFollower_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                objFollowByKeywordManager.chkFollower = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkFollowing_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkFollowing.IsChecked == true)
                {
                    objFollowByKeywordManager.chkFollowing = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void chkFollowing_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                objFollowByKeywordManager.chkFollowing = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error ==>" + ex.Message);
            }
        }

        private void btnFollowByKeyword_Close_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
              
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error ==>" + ex.Message);
            }
        }



    }
}
