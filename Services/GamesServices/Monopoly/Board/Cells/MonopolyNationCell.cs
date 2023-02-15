﻿using Enums.Monopoly;
using Microsoft.Extensions.Logging.Abstractions;
using Models;
using Models.Monopoly;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pkcs;
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
    public class MonopolyNationCell : MonopolyCell
    {
        private Nation OfNation { get; set; }

        private CellBuyingBehaviour BuyingBehaviour;
        private MonopolBehaviour monopolBehaviour;

        private Dictionary<string, Costs> BuildingTypeToCostsMap;

        public MonopolyNationCell(Costs costs, Nation nation = Nation.NoNation)
        {
            OfNation = nation;
            BuyingBehaviour = new CellAbleToBuyBehaviour(costs);
            monopolBehaviour = new MonopolNationCellBehaviour();

            BuildingTypeToCostsMap.Add(
                Consts.Monopoly.FieldBuyString, Consts.Monopoly.NationFieldCosts
            );

            BuildingTypeToCostsMap.Add(
                Consts.Monopoly.OneHouseBuyString, Consts.Monopoly.NationOneHouseCosts
            );

            BuildingTypeToCostsMap.Add(
                Consts.Monopoly.TwoHousesBuyString, Consts.Monopoly.NationTwoHousesCosts
            );
        }

        public Nation GetNation()
        {
            return OfNation;
        }
        public string OnDisplay()
        {
            string result = "";
            result += $" Owner: {BuyingBehaviour.GetOwner().ToString()} |";
            result += $" Nation: {OfNation.ToString()} |";
            result += $" Buy For: {BuyingBehaviour.GetCosts().Buy} |";
            result += $" Stay Cost: {BuyingBehaviour.GetCosts().Stay} ";
            if (BuyingBehaviour.IsThereChampionship() == true)
                result += Consts.Monopoly.ChampionshipInfo;
            return result;
        }
        public Beach GetBeachName()
        {
            return Beach.NoBeach;
        }
        public MonopolyModalParameters GetModalParameters(in List<MonopolyCell> Board, MonopolyPlayer MainPlayer)
        {
            if (Board[MainPlayer.OnCellIndex].GetBuyingBehavior().GetOwner() != PlayerKey.NoOne)
                return null;

            StringModalParameters Parameters = new StringModalParameters();

            Parameters.Title = "What Do You wanna build?";
            Parameters.ButtonsContent.Add("Nothing");
            Parameters.ButtonsContent.Add(Consts.Monopoly.FieldBuyString);
            Parameters.ButtonsContent.Add(Consts.Monopoly.OneHouseBuyString);
            Parameters.ButtonsContent.Add(Consts.Monopoly.TwoHousesBuyString);
            return new MonopolyModalParameters(Parameters, ModalShow.AfterMove, ModalResponseIdentifier.Nation);
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
            BuyingBehaviour.SetOwner(MainPlayer.Key);
            BuyingBehaviour.SetBaseCosts(BuildingTypeToCostsMap[WhatIsBought]);
            //monopolBehaviour.UpdateBoardMonopol(CheckMonopol, MainPlayer.OnCellIndex);
        }
    }
}
