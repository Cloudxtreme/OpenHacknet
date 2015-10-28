﻿// Decompiled with JetBrains decompiler
// Type: Hacknet.MissionSerializer
// Assembly: Hacknet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 48C62A5D-184B-4610-A7EA-84B38D090891
// Assembly location: C:\Program Files (x86)\Steam\SteamApps\common\Hacknet\Hacknet.exe

using System;
using System.Text;

namespace Hacknet
{
    public static class MissionSerializer
    {
        public const string MISSION_FILE_DELIMITER = "  #%#\n";

        public static string generateMissionFile(object mission_obj, int contractRegistryNumber = 0,
            string GroupName = "CSEC")
        {
            var activeMission = (ActiveMission) mission_obj;
            return
                string.Concat(
                    string.Concat(
                        GroupName + (object) " Contract #" + contractRegistryNumber +
                        "\n--------------------------------------------  #%#\n" + "Code = " +
                        encodeString(activeMission.reloadGoalsSourceFile) + "\n  #%#\n" + "Client = " +
                        activeMission.client + "  #%#\n" + "Target = " + activeMission.target + "  #%#\n",
                        (object) "RequiredRank = ", (object) activeMission.requiredRank, (object) "  #%#\n"),
                    "Difficulty = ", activeMission.difficulty, "\n  #%#\n") + "Title = " + activeMission.postingTitle +
                "  #%#\n" + "Posting = " + activeMission.postingBody + "\n  #%#\n" + "E_TargetTrack = " +
                encodeString(activeMission.genTarget) + "  #%#\n" + "E_TargetTaskData = " +
                encodeString(activeMission.genFile) + "  #%#\n" + "E_TargetTaskTrack = " +
                encodeString(activeMission.genPath) + "  #%#\n" + "E2_TargetTaskTrack_1 = " +
                encodeString(activeMission.genTargetName) + "  #%#\n" + "E3_TargetTaskTrack_2 = " +
                encodeString(activeMission.genOther) + "  #%#\n" + "E3_TargetTaskTrack_3 = " +
                (activeMission.wasAutoGenerated ? "gen" : "cmd") + "  #%#\n";
        }

        public static object restoreMissionFromFile(string data, out int contractRegistryNumber)
        {
            var separator = new string[1]
            {
                "  #%#\n"
            };
            var strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string str1;
            var str2 = str1 = "unknown";
            var str3 = str1;
            var str4 = str1;
            var str5 = str1;
            var str6 = str1;
            var str7 = str1;
            var str8 = str1;
            var filename = str1;
            var flag = false;
            int num1;
            var num2 = num1 = 0;
            for (var index = 0; index < strArray.Length; ++index)
            {
                if (!strArray[index].StartsWith("//"))
                {
                    var line = strArray[index];
                    if (line.StartsWith("Code"))
                        filename = decodeString(getDataFromConfigLine(line, "= "));
                    else if (line.StartsWith("Client"))
                        str8 = getDataFromConfigLine(line, "= ");
                    else if (line.StartsWith("Target"))
                        str7 = getDataFromConfigLine(line, "= ");
                    else if (line.StartsWith("E_TargetTrack"))
                        str6 = decodeString(getDataFromConfigLine(line, "= "));
                    else if (line.StartsWith("E_TargetTaskData"))
                        str5 = decodeString(getDataFromConfigLine(line, "= "));
                    else if (line.StartsWith("E_TargetTaskTrack"))
                        str4 = decodeString(getDataFromConfigLine(line, "= "));
                    else if (line.StartsWith("E2_TargetTaskTrack_1"))
                        str3 = decodeString(getDataFromConfigLine(line, "= "));
                    else if (line.StartsWith("E3_TargetTaskTrack_2"))
                        str2 = decodeString(getDataFromConfigLine(line, "= "));
                    else if (line.StartsWith("E3_TargetTaskTrack_3"))
                        flag = getDataFromConfigLine(line, "= ") == "gen";
                    else if (line.StartsWith("Rank"))
                    {
                        try
                        {
                            num2 = Convert.ToInt32(getDataFromConfigLine(line, "= "));
                        }
                        catch (FormatException ex)
                        {
                            contractRegistryNumber = 0;
                        }
                        catch (OverflowException ex)
                        {
                            contractRegistryNumber = 0;
                        }
                    }
                    else if (line.StartsWith("Difficulty"))
                    {
                        try
                        {
                            num1 = Convert.ToInt32(getDataFromConfigLine(line, "= "));
                        }
                        catch (FormatException ex)
                        {
                            contractRegistryNumber = 0;
                        }
                        catch (OverflowException ex)
                        {
                            contractRegistryNumber = 0;
                        }
                    }
                }
            }
            MissionGenerationParser.Client = str8;
            MissionGenerationParser.Comp = str6;
            MissionGenerationParser.File = str5;
            MissionGenerationParser.Path = str4;
            var activeMission = (ActiveMission) ComputerLoader.readMission(filename);
            activeMission.genFile = str5;
            activeMission.genPath = str4;
            activeMission.genTarget = str6;
            activeMission.genTargetName = str3;
            activeMission.genOther = str2;
            activeMission.target = str7;
            activeMission.client = str8;
            activeMission.requiredRank = num2;
            activeMission.difficulty = num1;
            activeMission.wasAutoGenerated = flag;
            contractRegistryNumber = 1;
            return activeMission;
        }

        private static string encodeString(string s)
        {
            var str = "";
            if (s != null)
            {
                for (var index = 0; index < s.Length; ++index)
                    str = str + (int) s[index] + " ";
            }
            return str.Trim();
        }

        private static string decodeString(string s)
        {
            var separator = new char[1]
            {
                ' '
            };
            var strArray = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var stringBuilder = new StringBuilder();
            for (var index = 0; index < strArray.Length; ++index)
                stringBuilder.Append(Convert.ToChar(Convert.ToInt32(strArray[index])));
            return stringBuilder.ToString();
        }

        private static string getDataFromConfigLine(string line, string sentinel = "= ")
        {
            return line.Substring(line.IndexOf(sentinel) + 2);
        }
    }
}