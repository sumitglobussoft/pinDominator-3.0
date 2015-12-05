using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseLib
{
    public class ClGlobul
    {

        //Pin Module...
        public static List<Pins> lstListOfPins = new List<Pins>();
        public static List<string> lstListOfNewUsers = new List<string>();
        public static List<Pins> lst_AddnewPinWithNewBoard = new List<Pins>();
        public static List<string> CommentNicheMessageList = new List<string>();
        public static List<string> lstRepinUrl = new List<string>();
        public static List<string> addNewPinWithBoard = new List<string>();
        public static List<string> RepinMessagesList = new List<string>();

        //Scraper Module....
        public static List<string> lstTotalUserScraped = new List<string>();
        public static List<string> GetPinList = new List<string>();

        //Board Module...
        public static List<string> lstBoardNames = new List<string>();
        public static List<string> lstListOfBoardNames = new List<string>();
        public static List<string> lstBoardNameswithUserNames = new List<string>();
        public static List<string> lstBoardRepinMessage = new List<string>();

        //Like Module...
        public static List<string> lstAddToBoardUserNames = new List<string>();
        public static List<string> lstLikePinUrls = new List<string>();

        //Invite Module...
        public static List<string> lstEmailInvites = new List<string>();

        //Follow Module..
        public static List<string> lstFollowUsername = new List<string>();
        public static List<string> ListOfFollowUsersFollowers = new List<string>();
        public static Queue<string> FollowUsersFollowerQueue = new Queue<string>();
        public static List<string> lstkeyword = new List<string>();

        //Comment Module....
        public static List<string> CommentMessagesList = new List<string>();
        public static List<string> lstMessageKeyword = new List<string>();
     
        //UbFollow Module...
        public static List<string> lstFollowing_UnFollow = new List<string>();

        public static List<string> lstPins = new List<string>();

        public static List<string> lstBoardUrls = new List<string>();
        public static List<string> lstBoardId = new List<string>();
        public static List<string> BoardNames = new List<string>();

    }

    public struct Pins
    {
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Board { get; set; }
        public string Email { get; set; }
    }




}
