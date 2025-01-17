﻿using Models.Monopoly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public static class Consts
    {
        public static string MySQLConnectionString => "server=localhost;port=3306;database=bazaDanych;user=root;password=1234";
        public static string ServerURL => "https://localhost:7268";

        public static class Message
        {
            public static string ServerDown = "Server is down";
        }

        public static class HubUrl
        {
            public static string Monopoly => "/monopolyhub";
            public static string Battleship => "/battleshiphub";
            public static string DemoButtons => "/multihub";
        }
        public static class Battleship
        {
            public static Point2D BoardSize = new Point2D(10, 10);
        }
        public static class BlackJack
        {
            public static int MaxPoints = 31;
            public static int MinPoints = 27;
        }
        public static class TicTacToe
        {
            public static Point2D BoardSize = new Point2D(3, 3);
            public static char Player = 'X';
            public static char Enemy = 'O';
            public static char Empty = ' ';
            public static char Tie = '#';
        }
        public static class Monopoly
        {
            public static readonly int IslandEscapeCost = 30;
            public static readonly int StartMoneyAmount = 400;
            public static readonly int OnStartCrossedMoneyGiven = 50;
            public static readonly int TaxAmount = 60;
            public static readonly float MonopolMultiplayer = 2.0f;
            public static readonly float ChampionshipMultiplayer = 2.0f;
            public static readonly float CellRepurchaseMultiplayer = 2.0f;
                          
            public static readonly string IslandDiaplsy = "Desert Island";
            public static readonly string ChampionshipInfo = "|World Championship|";
                          
            public static readonly float[] BeachesOwnedMultiplier = new float[] { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f };
                          
            public static readonly string ThrowDiceIslandButtonContent = "Throw Dice(Excape if 1 is Rolled)";
            public static readonly string PayToEscapeIsland = $"Pay {Consts.Monopoly.IslandEscapeCost} To Leave";
                          
            public static readonly string NoBuildingBought = "Nothing";
            public static readonly string Field = "Field";
            public static readonly string OneHouse = "1 House";
            public static readonly string TwoHouses = "2 Houses";
            public static readonly string ThreeHouses = "3 Houses";
            public static readonly string Hotel = "Hotel";
            public static readonly List<string> PossibleBuildings = new List<string> { Field, OneHouse, TwoHouses, ThreeHouses, Hotel };
                          
            public static readonly string BeachBuyAccepted = "Yes";
            public static readonly string BeachBuyDeclined = "No";
            public static readonly Costs BeachCellCosts = new Costs(100, 30);

            public static readonly string PayTaxRolled = "Pay Tax";
            public static readonly string NewChampionshipModalPrefix = "Championship On:";
            public static readonly string SellCellPrefix = "Sell this:";

            public static readonly int Dublet = 6;

        }
    }
}
