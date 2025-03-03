using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finns_i_Monogame;

namespace Fan_igen
{
    public class Spelare
    {
        private List<Kortvisuel> Hand = new List<Kortvisuel>();
        private int VemÄrDu;
        private int Poäng = 0;

        private int VilkenDuFrågar;
        private int PositionAvDetKort;
        public Spelare(int a){
            VemÄrDu = a;
        }

        public int spelare{
            get{return VilkenDuFrågar;}
            set{VilkenDuFrågar = value;}
        }
        public int PosiKort{
            get{return PositionAvDetKort;}
            set{PositionAvDetKort=value;}
        } 
        public int poäng{
            set{Poäng+=value;}
            get{return Poäng;}
        }
        public int vemärdu{
            get{return VemÄrDu;}
        }
        public string text{
            get{return "Spelare "+(VemÄrDu+1);}
        }
        public List<Kortvisuel> hand{
            set{Hand = value;}
            get{return Hand;}
        }
    }
}