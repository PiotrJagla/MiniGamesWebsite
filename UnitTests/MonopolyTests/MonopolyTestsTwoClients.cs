﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.GamesServices.Monopoly;
using Services.GamesServices.Monopoly.Board;
using Services.GamesServices.Monopoly.Update;
using Models.Monopoly;
using Models.MultiplayerConnection;
using Enums.Monopoly;

namespace UnitTests.MonopolyTests
{
    [TestClass]
    public class MonopolyTestsTwoClients
    {
        private PlayerKey[] BuyingOrder = new PlayerKey[]
        {
            PlayerKey.First, PlayerKey.Secound, PlayerKey.Secound, PlayerKey.First, PlayerKey.First,PlayerKey.First,PlayerKey.First,
            PlayerKey.First,PlayerKey.First,PlayerKey.First,PlayerKey.NoOne, PlayerKey.NoOne, PlayerKey.NoOne, PlayerKey.NoOne,
            PlayerKey.NoOne,
        };

        private PlayerKey[] BuyingOrderInLosingMonopolCheck = new PlayerKey[]
        {
            PlayerKey.Secound, PlayerKey.Secound, PlayerKey.First, PlayerKey.First, PlayerKey.First,PlayerKey.First,PlayerKey.First,
            PlayerKey.First,PlayerKey.First,PlayerKey.First,PlayerKey.NoOne, PlayerKey.NoOne, PlayerKey.NoOne, PlayerKey.NoOne,
            PlayerKey.NoOne,
        };

        private List<MonopolyService> Clients;

        [TestInitialize]
        public void TestsSetup()
        {
            Clients = ResetClients();
        }

        private List<MonopolyService> ResetClients()
        {
            List<MonopolyService> Result = new List<MonopolyService>();
            int ClientsNumber = 2;
            for (int i = 0; i < ClientsNumber; i++)
            {
                Result.Add(new MonopolyGameLogic());
            }
            return Result;
        }

        [TestMethod]
        public void PlayersMoneyOwnedAfter1To5TurnsTest()
        {
            int TurnsToTest = 5;
            for (int turn = 1; turn <= TurnsToTest; turn++)
            {
                Clients = ResetClients();
                List<MoneyFlow> ClientsMoneyFlow = MonopolyDataPrepare.ExecuteTurnsNumber(TurnsToTest, ref Clients, BuyingOrder);

                List<PlayerUpdateData> ActualMoneyFirst = Clients[0].GetUpdatedData().PlayersData;
                List<int> ExpectedMoney = MonopolyDataPrepare.GetExpectedMoney(ClientsMoneyFlow);
                Assert.IsTrue(MonopolyDataPrepare.CompareMoneyAmount(ExpectedMoney, ActualMoneyFirst));
            }
        }

        [TestMethod]
        public void BankruptTest()
        {
            
            List<MoneyFlow> PlayersMoneyFlow = null;

            for (int i = 1; ; i++)
            {
                Clients = ResetClients();
                PlayersMoneyFlow = MonopolyDataPrepare.ExecuteTurnsNumber(i, ref Clients,BuyingOrder);

                if (PlayersMoneyFlow[1].Income + Consts.Monopoly.StartMoneyAmount < PlayersMoneyFlow[1].Loss)
                {
                    break;
                }
            }

            MonopolyUpdateMessage CheckBankrupcy = Clients[1].GetUpdatedData();

            Assert.IsTrue(CheckBankrupcy.BankruptPlayer == PlayerKey.Secound);
        }

       [TestMethod]
        public void WinnerTest()
        {
            List<MoneyFlow> PlayersMoneyFlow = null;

            for (int i = 1; ; i++)
            {
                Clients = ResetClients();
                PlayersMoneyFlow = MonopolyDataPrepare.ExecuteTurnsNumber(i, ref Clients, BuyingOrder);

                if (PlayersMoneyFlow[1].Income + Consts.Monopoly.StartMoneyAmount < PlayersMoneyFlow[1].Loss)
                {
                    break;
                }
            }

            Clients[1].UpdateData(Clients[1].GetUpdatedData());

            Assert.IsTrue(Clients[0].WhoWon() == PlayerKey.First);
            Assert.IsTrue(Clients[1].WhoWon() == PlayerKey.First);
        }

        [TestMethod]
        public void LosingNationMonopolAfterSellingCellTest()
        {
            List<MoneyFlow> PlayersMoneyFlow = null;
            int StayCostWithoutMonopolExpected = Clients[0].GetBoard()[1].GetBuyingBehavior().GetCosts().Stay;

            for (int i = 3; ; i++)
            {
                Clients = ResetClients();
                PlayersMoneyFlow = MonopolyDataPrepare.ExecuteTurnsNumber(i, ref Clients, BuyingOrderInLosingMonopolCheck);

                if (PlayersMoneyFlow[1].Income + Consts.Monopoly.StartMoneyAmount < PlayersMoneyFlow[1].Loss)
                {
                    break;
                }
            }
            int StayCistWithoutMonopolActual = Clients[0].GetBoard()[1].GetBuyingBehavior().GetCosts().Stay;
            Assert.IsTrue(StayCistWithoutMonopolActual == StayCostWithoutMonopolExpected);
        }
    }
}
