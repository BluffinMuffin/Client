using BluffinMuffin.Client.DataTypes;
using BluffinMuffin.Protocol.DataTypes;
using Com.Ericmas001.Games;

namespace BluffinMuffin.Client.Protocol
{
    class DummyTable : TableInfo
    {

        /// <summary>
        /// Sets the cards on the table
        /// </summary>
        public void SetCards(params GameCard[] cards)
        {
            Cards = cards;
        }

        public void ClearSeat(int noSeat)
        {
            Seats[noSeat].Player = null;
        }

        public void SetSeat(SeatInfo seat)
        {
            m_Seats[seat.NoSeat] = seat;
        }
    }
}
