using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Models
{
    public class Level
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public int TileMapWidth { get; set; }
        public int TileMapHeight { get; set; }
        public int TileBaseSize { get; set; }
        public float TileMapScale { get; set; }
        public int StartMoney { get; set; }

        public List<TextEvent> TextEvents { get; set; } = new List<TextEvent>();

        // Default constructor is required for XML serialization
        public Level() { }
    }
}
