using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoFilials.Scripts
{
    class Nazvanie_Yecheyki
    {
        private string[] Alpabet = new string[26];
        public Nazvanie_Yecheyki()
        {
            Alpabet[0] = "A";
            Alpabet[1] = "B";
            Alpabet[2] = "C";
            Alpabet[3] = "D";
            Alpabet[4] = "E";
            Alpabet[5] = "F";
            Alpabet[6] = "G";
            Alpabet[7] = "H";
            Alpabet[8] = "I";
            Alpabet[9] = "J";
            Alpabet[10] = "K";
            Alpabet[11] = "L";
            Alpabet[12] = "M";
            Alpabet[13] = "N";
            Alpabet[14] = "O";
            Alpabet[15] = "P";
            Alpabet[16] = "Q";
            Alpabet[17] = "R";
            Alpabet[18] = "S";
            Alpabet[19] = "T";
            Alpabet[20] = "U";
            Alpabet[21] = "V";
            Alpabet[22] = "W";
            Alpabet[23] = "X";
            Alpabet[24] = "Y";
            Alpabet[25] = "Z";
        }
        public string Name_yacheiki(int z)
        {
            string a = "";
            while (z > 26)
            {
                if ((z % 26) == 0)
                {

                }
                else
                {
                    a = Alpabet[(z % 26) - 1] + a;
                }
                z = z / 26;
            }
            a = Alpabet[z - 1] + a;

            return a;
        }
    }
}
