using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Models
{
    public class Weapon
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }

        public Character? Character { get; set; }   // 1 - 1 İlişki
        public int CharacterId { get; set; }
    }
}