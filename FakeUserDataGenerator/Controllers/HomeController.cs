using Dataloader;
using FakeUserDataGenerator.Data;
using FakeUserDataGenerator.Models;
using FakeUserDataGenerator.Models.UsersData;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace FakeUserDataGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserContext _userContext;
        private static List<int> columns = new List<int>();
        public HomeController(ILogger<HomeController> logger, UserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public JsonResult GetData(string region, string seed, float numErrors, int page = 1)
        {
            DataHttpClient users = new DataHttpClient();
            var res = users.GenerateListOfPersons(page, seed, region).Result;
            var Jdata = (JArray)res["results"];

            List<UsersViewModel> data = new List<UsersViewModel>();
            int indexForFirstPage = 1;
            int indexForNotFirstPage = page * 10 + indexForFirstPage;

            foreach (var user in Jdata)
            {
                var m = new UsersViewModel()
                {
                    index = page == 1 ? indexForFirstPage++ : indexForNotFirstPage++,
                    userID = (string)user["login"]["uuid"],
                    name = (string)user["name"]["first"] + " " + (string)user["name"]["last"],
                    address = region.ToUpper() + " " + (string)user["location"]["postcode"] + " " + user["location"]["city"] + ", " + user["location"]["street"]["name"]
                        + ", " + user["location"]["street"]["number"],
                    phone = (string)user["cell"],
                };
                data.Add(m);
            }
            var rowsWithErrors = GenerateRowsWithErrors(data, seed, numErrors, page);
            var usersWithErrors = GenerateErrors(data, rowsWithErrors);
            var dj = columns.GroupBy(g => g);
            var cjc = dj.Select(s => new { k = s.Key, c = s.Count() });
            return Json(usersWithErrors);

        }
        private Dictionary<int, int> GenerateRowsWithErrors(List<UsersViewModel> users, string seed, float error, int page)
        {
            decimal fraction = (decimal)error;
            int iPart = (int)fraction;
            decimal dPart = fraction % 1.0m;

            int numberOfRecords = 20;
            if (page > 1) numberOfRecords = 10;
            int seedInitial = GetSeedFromString(seed);
            var seedRND = new Random(seedInitial);
            var seedFinal = seedRND.Next();

            Dictionary<int, int> rowsToApplyErrors = new Dictionary<int, int>();

            if (error == 0) return rowsToApplyErrors;

            if (dPart <= 1)
            {
                while (rowsToApplyErrors.Count < (numberOfRecords * dPart))
                {
                    if (page == 1)
                    {
                        var addIndex = (seedFinal / 10) % 10 + 10;

                        if (!rowsToApplyErrors.ContainsKey(addIndex))
                        {
                            rowsToApplyErrors[addIndex] = 1;
                        }
                    }
                    var index = seedFinal % 10;

                    if (!rowsToApplyErrors.ContainsKey(index))
                    {
                        rowsToApplyErrors[index] = 1;
                    }
                    seedFinal = new Random(seedFinal - page).Next();
                }
            }

            if (iPart > 0)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (!rowsToApplyErrors.ContainsKey(i))
                    {
                        rowsToApplyErrors[i] = iPart;
                    }
                    else
                    {
                        rowsToApplyErrors[i] = rowsToApplyErrors[i] + iPart;
                    }
                }
            }
            return rowsToApplyErrors;
        }

        private List<UsersViewModel> GenerateErrors(List<UsersViewModel> users, Dictionary<int, int> collectionOfRowsWithErrors)
        {
            foreach (var item in collectionOfRowsWithErrors)
            {
                ApplyErrors(users[item.Key], item.Value, item.Key + item.Value);
            }
            return users;
        }

        private static void ApplyErrors(UsersViewModel user, int numOfErrors, int seed)
        {
            string culture = user.address[0..3];
            Dictionary<int, int> ErrorColumnPairs = new Dictionary<int, int>()
            {
                {1, 13},
                {2, 14},
                {3, 15},
                {4, 23},
                {5, 24},
                {6, 25},
                {7, 33},
                {8, 34},
                {9, 35}
            };
            for (int i = 0; i < numOfErrors; i++)
            {
                var stringToApplyErrors = "";

                string stringToApplyRandom = GetHashedString(user);

                int keyRandom = new Random(GetSeedFromString(stringToApplyRandom)).Next(1, 10);

                int columnId = ErrorColumnPairs[keyRandom] % 10;
                int errorId = ErrorColumnPairs[keyRandom] / 10;


                if (columnId == 3) stringToApplyErrors = user.name;
                if (columnId == 4) stringToApplyErrors = user.address;
                if (columnId == 5) stringToApplyErrors = user.phone;

                //choosing position in string to apply errors
                int position = new Random(seed + i).Next(0, stringToApplyErrors.Length);

                var strWithErrors = ApplyErrorToString(stringToApplyErrors, position, errorId, culture);
                if (columnId == 3) user.name = strWithErrors;
                if (columnId == 4) user.address = strWithErrors;
                if (columnId == 5) user.phone = strWithErrors;
            }
        }

        private static string GetHashedString (UsersViewModel user_)
        {
        byte[] byteArray = Encoding.ASCII.GetBytes(user_.phone + user_.name + user_.address);
        SHA256 mySHA256 = SHA256.Create();
        var hashStuff = mySHA256.ComputeHash(byteArray);
        return ToHex(hashStuff, false);
        }
        private static string ApplyErrorToString(string stringToApplyError, int position, int typeOfError, string culture)
        {
            char RNDcharForSwapping = GetRandomChar(culture, position);

            if (typeOfError == 2 && stringToApplyError.Length > 2)                 
            {
                return stringToApplyError.Remove(position, 1);
            }

            if (typeOfError == 1 && stringToApplyError.Length < 40)
            {
                return stringToApplyError.Insert(position, RNDcharForSwapping.ToString());
            }
            if (typeOfError == 3)
            {
                return SwapCharacters(stringToApplyError, position);
            }
            return stringToApplyError;                                      
        }

       private static int GetSeedFromString(string value)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(value);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToInt32(hashBytes, 0);
            }
        }
       private static char GetRandomChar(string region, int seed)
       {    
            string chars = "";
            switch (region)
            {    
                case "ES":
                    chars = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZabcdefghijklmnñopqrstuvwxyz";
                    break; 
                case "DE": 
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöüß";
                    break;
                default:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                    break;
            }
            //choosing random alphabet char
            int postion = new Random(seed+chars.Length).Next(0, chars.Length);
            return chars[postion];
        }

        private static string SwapCharacters(string stringInit, int position)
        {
            StringBuilder sb = new StringBuilder(stringInit);
            var temp = ' ';

            if (position == 0)
            {
                temp = stringInit[position + 1];
                sb[position+1] = stringInit[position];
                sb[position] = temp;
            }
            else
            {
                temp = stringInit[position - 1];
                sb[position - 1] = sb[position];
                sb[position] = temp;

            }
            return sb.ToString();
        }
        private static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }

    }      

}