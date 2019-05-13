using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbabilityCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            int teamstoGenerate = 3;
            string[] teamNames;
            double mustPickCredits, TotalCreditLimit=100;
            int wkLimit = 1, alLimitMin = 1, alLimitMax = 3, clLimitMin = 3, clLimitMax = 5, btLimitMin = 3, btLimitMax=5, totalNos=11,team1MP=0,team2MP=0;
            string team1 = default(string);
            string team2 = default(string);
            string sheetName = "CSKRR";
            Console.WriteLine("Starttime : " + DateTime.Now);
            string path = @"D:\Balaji\Personal\probability.xlsx";
            ExcelData getData = new ExcelData();
            List<Boxes> lstBoxes = getData.ReadExcel(path,sheetName);
            List<Boxes> mustPicks = new List<Boxes>();

            teamNames = lstBoxes.Select(x => x.team).Distinct().ToArray();
            team1 = teamNames[0];
            team2 = teamNames[1];
            mustPicks = lstBoxes.Where(x => x.priority == 3).ToList();
            mustPickCredits = mustPicks.Sum(x => x.credit);
            if(mustPickCredits!=0)
            { TotalCreditLimit = TotalCreditLimit - mustPickCredits;

            };
            foreach (var item in mustPicks)
            {
                totalNos = totalNos - 1;
                switch (item.type)
                {
                    case "wk":
                        wkLimit = wkLimit - 1;
                        break;
                    case "bt":
                        btLimitMax = btLimitMax - 1;
                        btLimitMin = btLimitMin - 1;
                        break;
                    case "al":
                        alLimitMax = alLimitMax - 1;
                        alLimitMin = alLimitMin-1;
                        break;
                    case "cl":
                        clLimitMax = clLimitMax - 1;
                        clLimitMin =clLimitMin- 1;
                        break;
                }
                if(item.team==team1)
                {
                    team1MP += 1;
                }
                else if(item.team==team2)
                {
                    team2MP+=1;
                }
            }
          
            var lstTotalTeams= GetCombination(lstBoxes);
            var lstDerived = lstTotalTeams.Where(x => x.lstBoxes.Count == totalNos);

            List<Teams> lstFinal = new List<Teams>();

            lstFinal=lstDerived.ToList();
            Console.WriteLine("Total items :" + lstFinal.Count());
            int teamcounter=0;
            foreach (var item in lstDerived.ToList())
            {
                Console.Write("\r{0}%   ", teamcounter++);
                string[] st = item.lstBoxes.Select(x => x.name).ToArray();
                var TtCR = item.lstBoxes.Sum(x => x.credit);
                var typeAl=item.lstBoxes.Count(x => x.type == "al");
                var typeBl = item.lstBoxes.Count(x => x.type == "cl");
               
                var typeBT = item.lstBoxes.Count(x => x.type == "bt");
                var typeWK = item.lstBoxes.Count(x => x.type == "wk");
                if (typeAl == 1 && typeBl<5 && typeBT>2) 
                {
                    string a = "d";
                }
                var team1Count = item.lstBoxes.Count(x => x.team == team1);
                var team2Count = item.lstBoxes.Count(x => x.team == team2);
                team1Count += team1MP;
                team2Count += team2MP;
                if(TtCR>TotalCreditLimit)
                {
                    lstFinal.Remove(item);
                }
                else if(typeAl>alLimitMax || typeAl<alLimitMin)
                {
                    lstFinal.Remove(item);
                }
                else if (typeBl >clLimitMax  || typeBl < clLimitMin)
                {
                    lstFinal.Remove(item);
                }
                else if (typeBT > btLimitMax || typeBT < btLimitMin)
                {
                    lstFinal.Remove(item);
                }
                else if (typeWK != wkLimit)
                {
                    lstFinal.Remove(item);
                }
                else if(team1Count<4 || team1Count >7)
                {
                    lstFinal.Remove(item);
                }
                else if(team2Count<4 || team2Count>7)
                {
                    lstFinal.Remove(item);
                }
               
            }
          

            Console.WriteLine("EndTime" + DateTime.Now);
            Console.WriteLine("After filter :" + lstFinal.Count);
           // Console.WriteLine(lstDerived.Count());
            Console.WriteLine("EndTime"+DateTime.Now);
            var maxcr = lstFinal.Select(x => x.lstBoxes.Max(y => y.point));
            var lstTopTeam = lstFinal.OrderByDescending(x => x.lstBoxes.Sum(y=>y.point));
          
            var getFirst5 = lstTopTeam.Take(teamstoGenerate);
            foreach (var item in getFirst5.ToList())
            {
                Console.WriteLine("\n Team:");
                foreach (var it in item.lstBoxes.ToList())
                {
                    Console.WriteLine("\t" + it.name);
                }
            }
            
#region commented 
            
            
            //List<Boxes> teams = new List<Boxes>();
            //var hold = lstBoxes.OrderByDescending(x=>x.priority).ThenByDescending(x => x.point)
            //     .ThenBy(x => x.credit)
            //     .ToList();
            //var totalCredits = lstBoxes.Sum(x => x.credit);

            //foreach (var item in hold)
            //{
            //    if (curCredit + item.credit <= crLimit)
            //    {
            //        Boxes team = new Boxes();
            //        team.name = item.name;
            //        team.credit = item.credit;
            //        curCredit += item.credit;
            //        if(item.type=="wk" && wkCount==0)
            //        {
            //            wkCredit+= item.credit;
            //            teams.Add(team);
            //        }
            //        else if(item.type=="al" && alCount<3 && alCount<30)
            //        {
            //            alCredit += item.credit;
            //            teams.Add(team);
            //        }
            //        else if (item.type == "bt" && btCount < 5 && btCount <45)
            //        {
            //            btCredit += item.credit;
            //            teams.Add(team);
            //        }
            //        else if (item.type == "cl" && blCount < 3 && blCount <45)
            //        {
            //            blCredit += item.credit;
            //            teams.Add(team);
            //        }
            //        switch (item.type)
            //        {
            //            case "wk":
            //                wkCount++;
            //                break;
            //            case "al":
            //                alCount++;
            //                break;
            //            case "bt":
            //                btCount++;
            //                break;
            //            case "cl":
            //                blCount++;
            //                break;
            //        }
            //    }
            //}
            ////string test;

            //foreach (var item in teams)
            //{
            //    Console.WriteLine("\n\tName:" + item.name + "\t Credit:" + item.credit);
            //}
#endregion
           // Console.WriteLine("\n\tTotal credit : " + curCredit);
            Console.WriteLine("\nEnd time"+DateTime.Now);
            Console.Read();
        }



        static List<Teams> GetCombination(List<Boxes> list)
        {
            List<Teams> lstResults = new List<Teams>();
            Teams tm = new Teams();
            List<Boxes> lstPicked = new List<Boxes>();
            lstPicked = list.Where(x => x.priority != 3).ToList();
            double count = Math.Pow(2, lstPicked.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                Teams newTeam = new Teams();
                List<Boxes> lstOut = new List<Boxes>();
               
                string st = default(string);
                string str = Convert.ToString(i, 2).PadLeft(lstPicked.Count, '0');
                //Console.WriteLine("\t" + str);
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                   //     Console.Write(list[j].name);
                        lstOut.Add(lstPicked[j]);
                    }
                }
                if(lstOut.Count!=0)
                {
                    newTeam.lstBoxes.AddRange(lstOut);
                }
                lstResults.Add(newTeam);
              //  lstResults.Add(st);
               


            }

            List<string> lsRes = new List<string>();
          //  lsRes = lstResults;
            //foreach (var item in lstResults)
            //{
            //    if (item.Length == 3)
            //    {
            //        Console.WriteLine("\n" + item);
            //        //lsRes.Remove(item);
            //    }
            //}
            return lstResults;
            Console.WriteLine();
        }
             
    }

    public class Boxes
    {
        public string name;
        public double credit;
        public double point;
        public string type;
        public int priority;
        public bool isPlaying;
        public string team;
    }

    public class Teams
    {
        public List<Boxes> lstBoxes {get;set;}
        double creditTotal;
        public Teams()
        {
            lstBoxes = new List<Boxes>();
            creditTotal = lstBoxes.Sum(s => s.credit);
        }
    }

    public class Team
    {
        public Boxes boxes;
        double creditTotal;
    }
}
