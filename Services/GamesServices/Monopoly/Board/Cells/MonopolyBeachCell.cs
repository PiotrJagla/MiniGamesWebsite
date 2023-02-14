﻿using Enums.Monopoly;
using Models;
using Models.Monopoly;
using Services.GamesServices.Monopoly.Board.Behaviours;
using Services.GamesServices.Monopoly.Board.Behaviours.Buying;
using Services.GamesServices.Monopoly.Board.Behaviours.Monopol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.GamesServices.Monopoly.Board.Cells
{
    public class MonopolyBeachCell : MonopolyCell
    {
        private Beach BeachName { get; set; }

        private CellBuyingBehaviour BuyingBehaviour;

        private MonopolBehaviour monopolBehaviour;

        public MonopolyBeachCell(Costs costs, Beach WhatBeach = Beach.NoBeach)
        {
            BeachName = WhatBeach;

            BuyingBehaviour = new CellAbleToBuyBehaviour(costs);
            monopolBehaviour = new MonopolBeachCellBehaviour();
        }
        public Nation GetNation()
        {
            return Nation.NoNation;
        }
        
        public string OnDisplay()
        {
            string result = "";
            result += $" Owner: {BuyingBehaviour.GetOwner().ToString()} |";
            result += $" Beach: {BeachName.ToString()} |";
            result += $" Buy For: {BuyingBehaviour.GetCosts().Buy} |";
            result += $" Stay Cost: {BuyingBehaviour.GetCosts().Stay} ";
            return result;
        }
        public Beach GetBeachName()
        {
            return BeachName;
        }

        public MonopolyModalParameters GetModalParameters(in List<MonopolyCell> Board, MonopolyPlayer MainPlayer)
        {
            if (Board[MainPlayer.OnCellIndex].GetBuyingBehavior().GetOwner() != PlayerKey.NoOne)
                return null;

            StringModalParameters Parameters = new StringModalParameters();

            Parameters.Title = "Do you wany to buy this cell";
            Parameters.ButtonsContent.Add("Yes");
            Parameters.ButtonsContent.Add("No");
            return new MonopolyModalParameters(Parameters, ModalShow.AfterMove, ModalResponseIdentifier.Beach);
        }

        public CellBuyingBehaviour GetBuyingBehavior()
        {
            return BuyingBehaviour;
        }


        public MonopolBehaviour MonopolChanges()
        {
            return monopolBehaviour;
        }

        public void CellBought(MonopolyPlayer MainPlayer, string WhatIsBought)
        {
            
        }
    }
}
