using CarBookingBE.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace CarBookingBE.Utils.HandleToken
{
    public class TokenBlacklist
    {
        public static List<TokenBlacklistDTO> _blacklistedTokens = new List<TokenBlacklistDTO>();
        public static string blackListPath = HttpContext.Current.Server.MapPath($"~/Utils/HandleToken/TokenBlacklist.txt");
        public TokenBlacklist ()
        {
            _blacklistedTokens.Clear();
            try
            {
                if(!File.Exists(blackListPath))
                {
                    File.Create(blackListPath);
                }
                using (StreamReader reader = new StreamReader(blackListPath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        string[] dataArray = line.Split(',');
                        string id = dataArray[0];
                        string token = dataArray[1];
                        _blacklistedTokens.Add(new TokenBlacklistDTO { UserId = id, JwtToken = token });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }
        }
        public static void WriteToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(blackListPath))
                {
                    foreach (TokenBlacklistDTO e in _blacklistedTokens)
                    {
                        writer.WriteLine(e);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error writing file: " + ex.Message);
            }
        }
        public static void BlacklistToken(string uid, string token)
        {
            _blacklistedTokens.Add(new TokenBlacklistDTO { UserId = uid, JwtToken = token });
            WriteToFile();
        }
        public static bool IsTokenBlacklisted(string token)
        {
            lock (_blacklistedTokens)
            {
                foreach (var blackToken in _blacklistedTokens)
                {
                    if(blackToken.JwtToken.Equals(token))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}