using FirstFloor.ModernUI.Windows;
//using FaceDominator3._0.Pages.Account;
//using FaceDominator3._0.Pages.Friends;
//using FaceDominator3._0.Pages.FanPage;
//using FaceDominator3._0.Pages.Wall;
//using FaceDominator3._0.Pages.Events;
//using FaceDominator3._0.Pages.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinDominator3.Pages.PageAccount;
using PinDominator3.Pages.PageBlog;
using PinDominator3.Pages.Pageboard;
using PinDominator3.Pages.PageComment;
using PinDominator3.Pages.PageFollow;
using PinDominator3.Pages.PageInvite;
using PinDominator3.Pages.PageLike;
using PinDominator3.Pages.PageMessage;
using PinDominator3.Pages.PagePin;
using PinDominator3.Pages.PageProxy;
using PinDominator3.Pages.PageScraper;
using PinDominator3.Pages.setting;



namespace PinDominator3
{
    /// <summary>
    /// Loads lorem ipsum content regardless the given uri.
    /// </summary>
    public class PageLoader : DefaultContentLoader
    {
        /// <summary>
        /// Loads the content from specified uri.
        /// </summary>
        /// <param name="uri">The content uri</param>
        /// <returns>The loaded content.</returns>
        protected override object LoadContent(Uri uri)
        {
            // return a new LoremIpsum user control instance no matter the uri

            //Accounts Module 
            if (uri.ToString() == "/Upload Accounts")
            {
                //return new SolveSecurity();

                return new UploadAccount();
            }
            else if (uri.ToString() == "/Account Creator")
            {
                 return new AccountCreator();
            }

            //Event Module

            else if (uri.ToString() == "/Account Profile")
            {
                return new AccountProfile();
            }
            else if (uri.ToString() == "/Manage Accounts")
            {
                return new ManageAccount();
            }

            //Pages Module

            else if (uri.ToString() == "/Account Checker")
            {
                  return new AcountChecker();
            }
            else if (uri.ToString() == "/Repin")
            {
                 return new RePin();
            }
            else if (uri.ToString() == "/Add New Pin")
            {
                return new AddNewPin();  
            }
            else if (uri.ToString() == "/Add Pin with new Board")
            {
                return new AddPinWithnewBoard();  
            }
            else if (uri.ToString() == "/Edit Pin Discription")
            {
                 return new EditPinDiscription();
            }
            else if (uri.ToString() == "/Repin By Keyword")
            {
                return new RepinByKeyword();
            }

            else if (uri.ToString() == "/Pin Scraper")
            {
                  return new PinScraper();
            }
            else if (uri.ToString() == "/Comment")
            {
                return new Comment();
            }
            else if (uri.ToString() == "/Comment By Keyword")
            {
                return new CommentByKeyword();
            }

            else if (uri.ToString() == "/Like")
            {
                return new Like();
            }

            else if (uri.ToString() == "/Like By Keyword")
            {
                return new LikeByKeyword();
            }

            //Group Module 

            else if (uri.ToString() == "/Follow By Username")
            {
                return new FollowByUsername();
            }
            else if (uri.ToString() == "/Follow By Keyword")
            {
                return new FollowByKeyword();
            }
            else if (uri.ToString() == "/Unfollow")
            {
                return new UnFollow();
            }
            else if (uri.ToString() == "/Message Reply")
            {
                return new Message();
            }
            else if (uri.ToString() == "/Board")
            {
                return new Boards();
            }
            else if (uri.ToString() == "/Add Users To Board") 
            {
                return new AddUsersToBoard();
            }

            else if (uri.ToString() == "/Add Board Name") 
            {
                return new AddBoardName();
            }

            else if (uri.ToString() == "/Blog")
            {
                return new Blog();
            }
            else if (uri.ToString() == "/Invite")
            {
                return new Invite();
            }

            //Wall Module 

            else if (uri.ToString() == "/User Scraper")
            {
                return new Scraper();
            }
            else if (uri.ToString() == "/Proxy Checker")
            {
                return new Proxy();
            }
            else if (uri.ToString() == "/Help")
            {
                return  new Help();
            }

            //Campaign Module

            else if (uri.ToString() == "/Settings/Appearance")
            {
                // return new CampaignProcess();
            }          
            return "";
           
        }
    }
}
