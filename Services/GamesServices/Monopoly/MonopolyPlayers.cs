﻿using Enums.Monopoly;
using Models;
using Models.Monopoly;
using Models.MultiplayerConnection;
using Services.GamesServices.Monopoly.Board.Cells;
using Services.GamesServices.Monopoly.Update;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Services.GamesServices.Monopoly
{
    public class MonopolyPlayers
    {
        private List<MonopolyPlayer> Players;
        private SpecialIndexes PlayersSpecialIndexes;
        private int NumberOfDubletsInARow;
        private bool IsDubletRolled;
        

        public MonopolyPlayers()
        {
            IsDubletRolled = false;
            NumberOfDubletsInARow = 1;
            Players = new List<MonopolyPlayer>();
            PlayersSpecialIndexes = new SpecialIndexes();
        }

        public void InitPlayers(List<Player> PlayersInGame)
        {
            for (int i = 0; i < PlayersInGame.Count; i++)
            {
                AddPlayer((PlayerKey)i);
            }
            PlayersSpecialIndexes.WhosTurn = 0;
        }

        public void SetMainPlayerIndex(int index)
        {
            if (PlayersSpecialIndexes.MainPlayer == -1)
                PlayersSpecialIndexes.MainPlayer = index;
        }

        private void AddPlayer(PlayerKey key)
        {
            Players.Add(new MonopolyPlayer());
            Players.Last().Key = key;
            Players.Last().OnCellIndex = 0;
            Players.Last().MoneyOwned = Consts.Monopoly.StartMoneyAmount;
            
        }

        public MonopolyPlayersUpdateData MakePlayersUpdateData()
        {
            MonopolyPlayersUpdateData PlayersUpdatedData = UpdateDataFactory.CreatePlayersUpdateData();
            PlayersUpdatedData.FormatPlayersUpdateData(Players);
            return PlayersUpdatedData;
        }

        public MonopolyPlayer GetMainPlayer()
        {
            try
            {
                MonopolyPlayer MainPlayerCopy = new MonopolyPlayer();
                MainPlayerCopy = Players[PlayersSpecialIndexes.MainPlayer];
                return MainPlayerCopy;
            }
            catch
            {
                return null;
            }
        }

        public void UpdateMainPlayer(MonopolyPlayer UpdatedPlayer)
        {
            Players[PlayersSpecialIndexes.MainPlayer] = UpdatedPlayer;
        }

        public bool DidGameStart()
        {
            return Players.Count != 0;
        }

        public PlayerKey CheckForBankruptPlayer(ref MonopolyUpdateMessage UpdateData)
        {
            try
            {
                if (GetMainPlayer().MoneyOwned < 0)
                    return GetMainPlayer().Key;
            }
            catch
            {
                return PlayerKey.NoOne;
            }

            //There is copy of MoneyObligation Because lambda doesnt accept references
            MoneyObligation BondCopy = new MoneyObligation();
            BondCopy.PlayerLosingMoney = UpdateData.MoneyBond.PlayerLosingMoney;
            int PlayerObligatedToPayMoneyOwned = GetPlayerObligatedToPayMoneyOwned(BondCopy);

            int ObligationAmount = UpdateData.MoneyBond.ObligationAmount;

            ChangeMoneyBondIfBankrupt(ref UpdateData, PlayerObligatedToPayMoneyOwned);
            return GetBankruptPlayer(UpdateData, PlayerObligatedToPayMoneyOwned, ObligationAmount);
        }

        private int GetPlayerObligatedToPayMoneyOwned(MoneyObligation BondCopy)
        {
            int PlayerObligatedToPayMoneyOwned = 0;
            MonopolyPlayer PlayerObligatedToPay = Players.FirstOrDefault(p => p != null && (p.Key == BondCopy.PlayerLosingMoney));
            if (PlayerObligatedToPay != null)
            {
                PlayerObligatedToPayMoneyOwned = PlayerObligatedToPay.MoneyOwned;
            }

            return PlayerObligatedToPayMoneyOwned;
        }

        private void ChangeMoneyBondIfBankrupt(ref MonopolyUpdateMessage UpdateData, int PlayerObligatedToPayMoneyOwned)
        {
            if (PlayerObligatedToPayMoneyOwned < UpdateData.MoneyBond.ObligationAmount)
            {
                UpdateData.MoneyBond.ObligationAmount = PlayerObligatedToPayMoneyOwned;
            }
        }
        private PlayerKey GetBankruptPlayer(MonopolyUpdateMessage UpdateData, int PlayerObligatedToPayMoneyOwned, int ObligationAmount)
        {
            if (PlayerObligatedToPayMoneyOwned < ObligationAmount)
            {
                return UpdateData.MoneyBond.PlayerLosingMoney;
            }

            return PlayerKey.NoOne;
        }

        public void UpdateData(MonopolyUpdateMessage UpdatedData)
        {
            IsDubletRolled = UpdatedData.DidDubletAppear;
            UpdatePlayersData(UpdatedData.PlayersData);
            UpdateMoneyObligation(UpdatedData.MoneyBond);
            UpdateBankruptPlayer(UpdatedData.BankruptPlayer);
        }

        private void UpdatePlayersData(List<PlayerUpdateData> PlayersUpdatedData)
        {            
            for (int i = 0; i < PlayersUpdatedData.Count; i++)
            {
                if (Players[PlayersUpdatedData[i].PlayerIndex] != null)
                {
                    Players[PlayersUpdatedData[i].PlayerIndex].OnCellIndex = PlayersUpdatedData[i].Position;
                    Players[PlayersUpdatedData[i].PlayerIndex].MoneyOwned = PlayersUpdatedData[i].Money;
                }
            }
        }

        public void UpdateMoneyObligation(MoneyObligation obligation)
        {
            MonopolyPlayer PlayerGettingMoney = Players.FirstOrDefault(p => p != null && (p.Key == obligation.PlayerGettingMoney));
            MonopolyPlayer PlayerLosingMoney = Players.FirstOrDefault(p => p != null && (p.Key == obligation.PlayerLosingMoney));
            if (PlayerGettingMoney != null && PlayerLosingMoney != null)
            {
                PlayerGettingMoney.MoneyOwned += obligation.ObligationAmount;
                PlayerLosingMoney.MoneyOwned -= obligation.ObligationAmount;
            }
        }

        public void UpdateBankruptPlayer(PlayerKey BankruptPlayerKey)
        {
            CheckIfMainPlayerWentBankrupt(BankruptPlayerKey);

            MonopolyPlayer BankruptPlayer = Players.FirstOrDefault(p => p != null && (p.Key == BankruptPlayerKey));
            int BankruptPlayerIndex = Players.IndexOf(BankruptPlayer);
            if (BankruptPlayerIndex != -1)
                Players[BankruptPlayerIndex] = null;
        }

        private void CheckIfMainPlayerWentBankrupt(PlayerKey BankruptPlayer)
        {
            if (PlayersSpecialIndexes.MainPlayer == -1) return;

            if (BankruptPlayer == Players[PlayersSpecialIndexes.MainPlayer].Key)
                PlayersSpecialIndexes.MainPlayer = -1;
        }        

        public void CheckForDublet(int MoveAmount, int BoardSize)
        {
            if(MoveAmount == Consts.Monopoly.Dublet)
            {
                DubletThrown(BoardSize);
            }
            else
            {                
                NumberOfDubletsInARow = 1;
                IsDubletRolled = false;
            }
        }

        private void DubletThrown(int BoardSize)
        {
            if (NumberOfDubletsInARow < 3)
            {
                NumberOfDubletsInARow++;
                IsDubletRolled = true;
            }
            else if (NumberOfDubletsInARow == 3)
            {
                ThirdDublet(BoardSize);
            }
        }

        private void ThirdDublet(int BoardSize)
        {
            IsDubletRolled = false;

            NumberOfDubletsInARow = 1;

            Players[PlayersSpecialIndexes.WhosTurn].OnCellIndex -= Consts.Monopoly.Dublet;
            CheckStartCellCrossed(BoardSize);
        }

        private void CheckStartCellCrossed(int BoardSize)
        {
            if (Players[PlayersSpecialIndexes.WhosTurn].OnCellIndex < 0)
            {
                ChargeMainPlayer(Consts.Monopoly.OnStartCrossedMoneyGiven);
                Players[PlayersSpecialIndexes.WhosTurn].OnCellIndex += BoardSize;
            }
        }

        public bool IsThisThirdDublet(int MoveAmount)
        {
            return MoveAmount == Consts.Monopoly.Dublet && (NumberOfDubletsInARow) == 3; 
        }

        public bool IsDubletRolledBySomeone()
        { 
            return IsDubletRolled;
        }

        public void NextTurn()
        {
            if (IsDubletRolled == false)
            {
                PlayersSpecialIndexes.WhosTurn = (++PlayersSpecialIndexes.WhosTurn) % Players.Count;

                while (Players[PlayersSpecialIndexes.WhosTurn] == null)
                    PlayersSpecialIndexes.WhosTurn = (++PlayersSpecialIndexes.WhosTurn) % Players.Count;

            }

        }

        public void ChargeMainPlayer(int ChargeAmount)
        {
            Players[PlayersSpecialIndexes.MainPlayer].MoneyOwned -= ChargeAmount;
        }

        public void GiveMainPlayerMoney(int MoneyToGive)
        {
            Players[PlayersSpecialIndexes.MainPlayer].MoneyOwned += MoneyToGive;
        }

        public bool IsMainPlayerTurn()
        {
            return PlayersSpecialIndexes.WhosTurn == PlayersSpecialIndexes.MainPlayer;
        }

        public PlayerKey WhoWon()
        {
            if (Players.FindAll(p => p != null).Count == 1)
                return Players.FirstOrDefault(p => p != null).Key;

            return PlayerKey.NoOne;
        }

    }
}
