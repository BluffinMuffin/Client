﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluffinMuffin.Protocol.DataTypes;
using BluffinMuffin.Protocol.DataTypes.Enums;
using BluffinMuffin.Protocol.DataTypes.Options;

namespace BluffinMuffin.Client.Extensions
{
    public static class LobbyOptionExtensions
    {

        public static int MinimumBuyInAmount(this LobbyOptions lo, int gameSize)
        {
            switch (lo.MinimumBuyInParameter)
            {
                case BuyInParameterEnum.FixedAmount:
                    return lo.MinimumBuyInValue;
                case BuyInParameterEnum.Multiplicator:
                    return lo.MinimumBuyInValue * gameSize;
            }
            return 0;
        }

        public static int MaximumBuyInAmount(this LobbyOptions lo, int gameSize)
        {
            switch (lo.MaximumBuyInParameter)
            {
                case BuyInParameterEnum.FixedAmount:
                    return lo.MaximumBuyInValue;
                case BuyInParameterEnum.Multiplicator:
                    return lo.MaximumBuyInValue * gameSize;
            }
            return int.MaxValue;
        }
    }
}