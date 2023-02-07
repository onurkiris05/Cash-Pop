using System;
using System.Globalization;
using System.Linq;
using ElephantSDK;
using UnityEngine;

namespace _Main.Scripts.Utilities
{
    public static class MoneyRollRemoteValues
    {
        public static float BaseLenght = 5;
        public static float CollectionSpeed = 0.25f;
        
        public static float BasePricePerBanknote = 1f;
        public static float AdditionalMultiplierPerLevel = 0.2f;

        private static int[] _gateUpgradeLevelAmounts = new []{1};

        public static int GetUpgradeLevel(Gate gate) =>
            _gateUpgradeLevelAmounts[gate.Level - 1 <= _gateUpgradeLevelAmounts.Length -1 ? gate.Level - 1 : _gateUpgradeLevelAmounts.Length -1]; 

        public static void LoadValues()
        {
            var remoteConfigs = RemoteConfig.GetInstance();
            BaseLenght = remoteConfigs.GetFloat("base_length", 5);
            CollectionSpeed = remoteConfigs.GetFloat("collection_speed", 0.25f);
            BasePricePerBanknote = remoteConfigs.GetFloat("money_base_value", 1f);
            AdditionalMultiplierPerLevel = remoteConfigs.GetFloat("money_increase_amount", 0.2f);
            _gateUpgradeLevelAmounts = IdleStringPricesToIntPrices(remoteConfigs.Get("gate_upgrade_level_amounts", "1"));
        }

        #region HelperMethods

        private static int[] IdleStringPricesToIntPrices(string priceStr)
        {
            var prices = priceStr.Split(',');

            int[] priceInt = new int[prices.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                if (int.TryParse(prices[i], out priceInt[i]))
                {
                }
                else
                {
                    return null;
                }
            }

            return priceInt;
        }

        public static ulong[] IdleStringPricesToUlongPrices(string priceStr)
        {
            var prices = priceStr.Split(',');

            ulong[] priceInt = new ulong[prices.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                if (ulong.TryParse(prices[i], out priceInt[i]))
                {
                }
                else
                {
                    return null;
                }
            }

            return priceInt;
        }

        public static float[] IdleStringPricesToFloatPrices(string priceStr)
        {
            var prices = priceStr.Split(',');

            float[] priceInt = new float[prices.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                if (float.TryParse(prices[i], NumberStyles.Float, CultureInfo.InvariantCulture, out priceInt[i]))
                {
                }
                else
                {
                    return null;
                }
            }

            return priceInt;
        }

        private static string[] IdleStringPricesToStringPrices(string priceStr)
        {
            var prices = priceStr.Split(',');
            return prices;
        }

        private static bool[] ConvertIntToBoolArray(int[] intArray)
        {
            var boolArray = new bool[intArray.Sum()];
            var index = 0;
            for (var i = 0; i < intArray.Length; i++)
            {
                var intValue = intArray[i];
                for (int j = 0; j < intValue - 1; j++)
                {
                    boolArray[index] = false;
                    index++;
                }

                boolArray[index] = true;
                index++;
            }

            return boolArray;
        }

        #endregion
    }
}