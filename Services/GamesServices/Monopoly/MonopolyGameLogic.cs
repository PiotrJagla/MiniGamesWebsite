﻿using Models.MultiplayerConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enums.Monopoly;
using System.Threading.Tasks;
using Models.Monopoly;
using Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Services.GamesServices.Monopoly.Update;
using MySqlX.XDevAPI.Common;
using Services.GamesServices.Monopoly.Board;

namespace Services.GamesServices.Monopoly
{
    public class MonopolyGameLogic : MonopolyService
    {
        private static List<MonopolyPlayer> Players = new List<MonopolyPlayer>();
        private static SpecialIndexes PlayersSpecialIndexes = new SpecialIndexes();

        private static MonopolyBoard BoardService = new MonopolyBoard();

        public void StartGame(List<Player> PlayersInGame)
        {
            if (GameIsAlreadyStarted())
                return;

            for (int i = 0; i < PlayersInGame.Count; i++)
            {
                AddPlayer((PlayerKey)i);
            }
            PlayersSpecialIndexes.WhosTurn = 0;
        }

        private bool GameIsAlreadyStarted()
        {
            return Players.Count != 0;
        }

        private void AddPlayer(PlayerKey playerKey)
        {
            Players.Add(new MonopolyPlayer());
            Players.Last().Key = playerKey;
            Players.Last().OnCellIndex = 0;
            Players.Last().MoneyOwned = Consts.Monopoly.StartMoneyAmount;
        }

        public List<MonopolyCell> GetBoard()
        {
            return BoardService.GetBoard();   
        }

        public MonopolyUpdateMessage GetUpdatedData()
        {
            MonopolyUpdateMessage UpdatedData = new MonopolyUpdateMessage();
            UpdatedData.PlayersData = MakePlayersUpdateData().GetPlayersUpdatedData();
            UpdatedData.CellsOwners = MakeBoardUpdateData().GetCellsOwners();
            UpdatedData.MoneyBond = MakeMoneyBond();
            return UpdatedData;
        }

        private PlayersUpdateData MakePlayersUpdateData()
        {
            PlayersUpdateData PlayersUpdatedData = new PlayersUpdateData();
            PlayersUpdatedData.FormatPlayersUpdateData(Players);
            return PlayersUpdatedData;
        }

        private MonopolyBoardUpdateData MakeBoardUpdateData()
        {
            MonopolyBoardUpdateData BoardUpdatedData = new MonopolyBoardUpdateData();
            BoardUpdatedData.FormatBoardUpdateData(BoardService.GetBoard());
            return BoardUpdatedData;
        }

        private MoneyObligation MakeMoneyBond()
        {
            if (GameIsAlreadyStarted() == false) 
                return new MoneyObligation();

            return CalculateBond();
        }

        private MoneyObligation CalculateBond()
        {
            MoneyObligation result = new MoneyObligation();
            if (BoardService.DoesCellHaveAnotherOwner(Players[PlayersSpecialIndexes.MainPlayer]))
            {
                MonopolyCell CellMainPlayerSteppedOn = BoardService.GetCell(Players[PlayersSpecialIndexes.MainPlayer].OnCellIndex);
                result.PlayerGettingMoney = CellMainPlayerSteppedOn.GetOwner();
                result.PlayerLosingMoney = Players[PlayersSpecialIndexes.MainPlayer].Key;
                result.ObligationAmount = CellMainPlayerSteppedOn.GetCosts().Stay;
            }
            return result;
        }


        public void UpdateData(MonopolyUpdateMessage UpdatedData)
        {
            UpdatePlayersData(UpdatedData.PlayersData);
            UpdateBoardData(UpdatedData.CellsOwners);
            UpdateMoneyObligation(UpdatedData.MoneyBond);
        }

        private void UpdatePlayersData(List<PlayerUpdateData> PlayersUpdatedData)
        {
            for (int i = 0; i < PlayersUpdatedData.Count; i++)
            {
                Players[i].OnCellIndex = PlayersUpdatedData[i].Position;
                Players[i].MoneyOwned = PlayersUpdatedData[i].Money;
            }
        }

        private void UpdateBoardData(List<MonopolyCellOwner> BoardUpdatedData)
        {
            for (int i = 0; i < BoardUpdatedData.Count; i++)
            {
                BoardService.GetBoard()[i].SetOwner(BoardUpdatedData[i].Owner);
            }
        }

        private void UpdateMoneyObligation(MoneyObligation obligation)
        {
            MonopolyPlayer PlayerGettingMoney = Players.FirstOrDefault(p => p.Key == obligation.PlayerGettingMoney);
            MonopolyPlayer PlayerLosingMoney = Players.FirstOrDefault(p => p.Key == obligation.PlayerLosingMoney);
            if (PlayerGettingMoney != null && PlayerLosingMoney != null)
            {
                PlayerGettingMoney.MoneyOwned += obligation.ObligationAmount;
                PlayerLosingMoney.MoneyOwned -= obligation.ObligationAmount;
            }
        }

        public void BuyCellIfPossible()
        {
            int MainPlayerBoardPos = Players[ PlayersSpecialIndexes.MainPlayer ].OnCellIndex;
            int MainPlayerMoney = Players[ PlayersSpecialIndexes.MainPlayer ].MoneyOwned;
            
            if (BoardService.CanAffordBuying(MainPlayerMoney, MainPlayerBoardPos))
            {
                BuyCell(MainPlayerBoardPos);
            }
        }

        private void BuyCell(int MainPlayerBoardPos)
        {
            PlayerKey MainPlayerKey = Players[ PlayersSpecialIndexes.MainPlayer ].Key;
            BoardService.GetCell(MainPlayerBoardPos).SetOwner(MainPlayerKey);
            Players[PlayersSpecialIndexes.MainPlayer].MoneyOwned -= BoardService.GetCell(MainPlayerBoardPos).GetCosts().Buy;
        }

        public MoveResult Move(int amount)
        {
            OnStartCellCrossed(amount);
            MonopolyPlayer MainPlayer = Players[PlayersSpecialIndexes.MainPlayer];
            MainPlayer.OnCellIndex = (MainPlayer.OnCellIndex + amount) % BoardService.GetBoard().Count;
            return BoardService.IsNoOneCell( MainPlayer.OnCellIndex ) ? MoveResult.OnNobodysCell : MoveResult.OnSomeonesCell;
        }

        private void OnStartCellCrossed(int MoveAmount)
        {
            if (DidCrossedStartCell(MoveAmount))
            {
                Players[PlayersSpecialIndexes.MainPlayer].MoneyOwned += Consts.Monopoly.OnStartCrossedMoneyGiven;
            }
        }

        private bool DidCrossedStartCell(int MoveAmount)
        {
            return (Players[PlayersSpecialIndexes.MainPlayer].OnCellIndex + MoveAmount) >= BoardService.GetBoard().Count;
        }

        public bool IsYourTurn()
        {
            return PlayersSpecialIndexes.WhosTurn == PlayersSpecialIndexes.MainPlayer;
        }

        public void NextTurn()
        {
            PlayersSpecialIndexes.WhosTurn = (++PlayersSpecialIndexes.WhosTurn) % Players.Count;
        }

        public void SetMainPlayerIndex(int index)
        {
            if (PlayersSpecialIndexes.MainPlayer == -1)
                PlayersSpecialIndexes.MainPlayer = index;
        }
    }
}
