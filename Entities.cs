using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon_console
{
    public class Entities
    {
        public static readonly Dictionary<char, string> KeyStore = new()
        {
            ['q'] = "input tap 54 1330",
            ['w'] = "input tap 155 1330",
            ['e'] = "input tap 270 1330",
            ['r'] = "input tap 375 1330",
            ['t'] = "input tap 485 1330",
            ['y'] = "input tap 590 1330",
            ['u'] = "input tap 700 1330",
            ['i'] = "input tap 808 1330",
            ['o'] = "input tap 915 1330",
            ['p'] = "input tap 1025 1330",

            ['a'] = "input tap 105 1500",
            ['s'] = "input tap 215 1500",
            ['d'] = "input tap 315 1500",
            ['f'] = "input tap 430 1500",
            ['g'] = "input tap 538 1500",
            ['h'] = "input tap 645 1500",
            ['j'] = "input tap 750 1500",
            ['k'] = "input tap 860 1500",
            ['l'] = "input tap 970 1500",

            ['z'] = "input tap 215 1660",
            ['x'] = "input tap 322 1660",
            ['c'] = "input tap 434 1660",
            ['v'] = "input tap 540 1660",
            ['b'] = "input tap 645 1660",
            ['n'] = "input tap 755 1660",
            ['m'] = "input tap 865 1660",

            [' '] = "input tap 540 1830",
            ['.'] = "input tap 865 1830",

            ['1'] = "input tap 54 1330",
            ['2'] = "input tap 155 1330",
            ['3'] = "input tap 270 1330",
            ['4'] = "input tap 375 1330",
            ['5'] = "input tap 485 1330",
            ['6'] = "input tap 590 1330",
            ['7'] = "input tap 700 1330",
            ['8'] = "input tap 808 1330",
            ['9'] = "input tap 915 1330",
            ['0'] = "input tap 1025 1330",

            ['@'] = "input tap 105 1500",
            ['#'] = "input tap 215 1500",
            ['$'] = "input tap 315 1500",
            ['%'] = "input tap 430 1500",
            ['&'] = "input tap 538 1500",
            ['-'] = "input tap 645 1500",
            ['+'] = "input tap 750 1500",
            ['('] = "input tap 860 1500",
            [')'] = "input tap 970 1500",

            ['*'] = "input tap 215 1660",
            ['"'] = "input tap 322 1660",
            ['\''] = "input tap 434 1660",
            [':'] = "input tap 540 1660",
            [';'] = "input tap 645 1660",
            ['!'] = "input tap 755 1660",
            ['?'] = "input tap 865 1660",
        };
        public static readonly Dictionary<char, string> NumberStore = new()
        {
            ['0'] = "input tap 435 1828",
            ['1'] = "input tap 115 1330",
            ['2'] = "input tap 435 1330",
            ['3'] = "input tap 710 1330",
            ['4'] = "input tap 115 1495",
            ['5'] = "input tap 435 1495",
            ['6'] = "input tap 710 1495",
            ['7'] = "input tap 115 1660",
            ['8'] = "input tap 435 1660",
            ['9'] = "input tap 710 1660",
        };
        public static readonly Dictionary<int, string> Month = new()
        {
            [1] = "input tap 540 204",
            [2] = "input tap 540 375",
            [3] = "input tap 540 546",
            [4] = "input tap 540 717",
            [5] = "input tap 540 888",
            [6] = "input tap 540 1059",
            [7] = "input tap 540 1230",
            [8] = "input tap 540 1401",
            [9] = "input tap 540 1572",
            [10] = "input tap 540 1743",
            [11] = "input tap 540 1851",
            [12] = "input tap 540 1851"   //random ra tháng 12 thfi cho là tháng 11 luôn , tại tahnsg 12 thì phải kéo xuống => tốn thao tác
        };
        public static readonly Dictionary<int, string> Gender = new()
        {
            [0] = "input tap 540 750", //female
            [1] = "input tap 540 910", //male
        };
    }
}
