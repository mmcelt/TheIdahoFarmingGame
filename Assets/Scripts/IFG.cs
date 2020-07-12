using UnityEngine;

public class IFG
{
	//Custom Property Keys
	public const string Networth_Game = "nwg";						//int
	public const string Timed_Game = "tg";								//float
	public const string Number_Of_Players = "nop";					//int
	public const string Player_Ready = "playerReady";				//bool
	public const string Selected_Farmer = "selectedFarmer";		//string
	public const string Selected_Farmer_Index = "farmerIndex";  //int
	public const string Player_Cash = "cash";							//int
	public const string Player_Notes = "notes";						//int
	public const string Player_Networth = "networth";				//int
	public const string Player_Hay = "hay";							//int
	public const string Player_Fruit = "fruit";						//int
	public const string Player_Grain = "grain";						//int
	public const string Player_FCows = "fCows";						//int
	public const string Player_RCows = "rCows";						//int
	public const string Player_Spuds = "spuds";                 //int
	public const string Player_Tractor = "tractor";             //bool
	public const string Player_Harvester = "harvester";         //bool
	public const string Player_Otb_Count = "otbs";					//int
	public const string Oxford_Range_Owned = "oxford";          //bool
	public const string Targhee_Range_Owned = "targhee";        //bool
	public const string LostRiver_Range_Owned = "lostRiver";    //bool
	public const string Lemhi_Range_Owned = "lemhi";            //bool
	public const string Wages_Garnished = "garnished";				//bool

	//Players
	public const string Becky = "Blackfoot Becky";
	public const string Jerry = "Jerome Jerry";
	public const string Kay = "Kimberly Kay";
	public const string Mike = "Menan Mike";
	public const string Ric = "Ririe Ric";
	public const string Ron = "Rigby Ron";

	//Winner List
	public const string Save_Separator = "#SAVE-VALUE#";

	//Widely used Strings
	public const string TetonDamMessageText = "HIDEOUS DISASTER! THE TETON DAM HAS BURST. ROLL THE DIE TO SEE YOUR FATE.\nEVEN: YOUR HIT\nODD: YOU ESCAPED";
	public const string TetonDamHeaderText = "TETON DAM DISASTER";
	public const string HarvestBaseMessage = "Please roll the die to get your gross Harvest Check...";
	//Special Colors
	public static Color Purple = new Color(0.6039f, 0.1607f, 0.7411f);
	public static Color Brown = new Color(0.6627f,0.4117f,0f);
	public static Color SpudBlue = new Color(0f, 0.6392157f, 0.8392158f);
	public static Color GreyedOut = new Color(0.5943396f, 0.5943396f, 0.5943396f);
	public static Color Orange = new Color(1, 0.647f, 0);

	public static bool SpudBonusGiven;
	public static bool CompleteFarmerBonusGiven;
}

public enum RaiseEventCodes
{
	Farmer_Selected_Event_Code = 0,
	Draw_Otb_Event_Code = 1,
	Receive_Otb_Event_code = 2,
	Draw_Ff_Event_Code = 3,
	Receive_Ff_Event_Code = 4,
	Draw_Oe_Event_Code = 5,
	Receive_Oe_Event_Code = 6,
	Replace_Otb_Event_Code = 7,
	Replace_Ff_Event_Code = 8,
	Replace_Oe_Event_Code = 9,
	Custom_Hire_Harvester_Code = 10,
	Get_Initial_Otb_Event_Code = 11,
	Change_Active_Player_Event_Code = 12,
	Place_Initial_Stickers_Event_Code = 13,
	Spud_Bonus_Given_Event_Code = 14,
	Spud_Message_Event_Code = 15,
	Shuffle_Deck_Event_Code = 16,
	Update_Deck_Data_Event_Code = 17,
	Complete_Farmer_Bonus_Given_Event_Code = 18,
	Complete_Farmer_Message_Event_Code = 19,
	Harvest_Roll_Message_Event_Code = 20,
	Shuffle_Message_Event_Code = 21,
	End_Networth_Game_Event_Code = 22,
	Teton_Dam_Event_Code = 23,
	Out_Of_Otbs_Event_Code = 24,
	Sell_Otb_To_Player_Event_Code = 25,
	Update_WinnersList_Event_Code = 26,
	Client_End_Of_Networth_Game_Message_Event_Code = 27,
	End_Timed_Game_Event_Code = 28,
	Teton_Dam_Hit_Event_Code = 29
};

