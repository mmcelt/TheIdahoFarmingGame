using UnityEngine;

public class IFG
{
	//Custom Properties
	public const string Networth_Game = "nwg";
	public const string Timed_Game = "tg";
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
	public const string Lemhi_Range_Owned = "lemhi";				//bool

	//Players
	public const string Becky = "Blackfoot Becky";
	public const string Jerry = "Jerome Jerry";
	public const string Kay = "Kimberly Kay";
	public const string Mike = "Menan Mike";
	public const string Ric = "Ririe Ric";
	public const string Ron = "Rigby Ron";
	//Widely used Strings
	public const string TetonDamMessageText = "HIDEOUS DISASTER! THE TETON DAM HAS BURST. ROLL THE DIE TO SEE YOUR FATE.\nODD: YOUR HIT\nEVEN: YOU ESCAPED";
	public const string TetonDamHeaderText = "TETON DAM DISASTER";
	public const string HarvestBaseMessage = "Please roll the die to get your gross Harvest Check...";
	//Special Colors
	public static Color Purple = new Color(0.6039f, 0.1607f, 0.7411f);
	public static Color Brown = new Color(0.6627f,0.4117f,0f);
	public static Color SpudBlue = new Color(0f, 0.6392157f, 0.8392158f);
	public static Color GreyedOut = new Color(0.5943396f, 0.5943396f, 0.5943396f);
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
	Place_Initial_Stickers_Event_Code = 13
};

