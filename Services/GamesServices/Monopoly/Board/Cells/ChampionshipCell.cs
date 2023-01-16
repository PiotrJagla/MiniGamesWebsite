﻿using Enums.Monopoly;
using Models;
using Models.Monopoly;
using Services.GamesServices.Monopoly.Board.Behaviours;
using Services.GamesServices.Monopoly.Board.BuyingBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.GamesServices.Monopoly.Board.Cells
{
    public class ChampionshipCell : MonopolyCell
    {
        private CellBuyingBehaviour BuyingBehaviour;
        private MonopolBehaviour monopolBehaviour;

        public ChampionshipCell()
        {
            BuyingBehaviour = new CellNotAbleToBuyBehaviour();
            monopolBehaviour = new NoMonopolBehaviour();
        }

        public Beach GetBeachName()
        {
            return Beach.NoBeach;
        }

        public CellBuyingBehaviour GetBuyingBehavior()
        {
            return BuyingBehaviour;
        }

        public MonopolyModalParameters GetModalParameters(in List<MonopolyCell> Board, PlayerKey MainPlayerKey)
        {
            StringModalParameters Parameters = new StringModalParameters();

            foreach (var cell in Board)
            {
                if(IsCellAddedToModal(cell,MainPlayerKey))
                    Parameters.ButtonsContent.Add(cell.OnDisplay());
            }

            Parameters.Title = "Choose Cell To Set World Championship";
            return new MonopolyModalParameters(Parameters, ModalShow.AfterMove, ModalResponseIdentifier.Championship);
        }

        private bool IsCellAddedToModal(MonopolyCell cell, PlayerKey MainPlayerKey)
        {
            return cell is MonopolyNationCell && cell.GetBuyingBehavior().GetOwner() == MainPlayerKey;
        }

        public Nation GetNation()
        {
            return Nation.NoNation;
        }

        public MonopolBehaviour MonopolChanges()
        {
            return monopolBehaviour;
        }

        public string OnDisplay()
        {
            return "World Championship";
        }
    }
}
