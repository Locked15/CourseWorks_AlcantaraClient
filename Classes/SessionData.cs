using System;
using AlcantaraClient.Entities;

namespace AlcantaraClient.Classes
{
    public class SessionData
    {
        public static User CurrentUser { get; set; }

        public static Order CurrentOrder { get; set; }
    }
}
