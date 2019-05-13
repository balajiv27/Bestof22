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
    public class ExcelData
    {
        private OleDbConnection OpenConnection(string path)
        {
            OleDbConnection oledbConn = null;
            try
            {
                if (Path.GetExtension(path) == ".xls")
                    oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + path +

"; Extended Properties= \"Excel 8.0;HDR=Yes;IMEX=2\"");
                else if (Path.GetExtension(path) == ".xlsx")
                    oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" +

path + "; Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");

                oledbConn.Open();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                //Error
            }
            return oledbConn;
        }

        private List<Boxes> ExtractEmployeeExcel(OleDbConnection oledbConn,string sheetName)
        {
            try
            {
                OleDbCommand cmd = new OleDbCommand(); ;
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet dsEmployeeInfo = new DataSet();

                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format("SELECT * FROM [Sheet4$] WHERE Playing = '{0}'", "y");// @"SELECT * FROM [Sheet3$] where Playing=""TRUE"""; //Excel Sheet Name ( Employee )
                oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(dsEmployeeInfo, "Boxes");

                var dsEmployeeInfoList = dsEmployeeInfo.Tables[0].AsEnumerable().Select(s => new Boxes
                {
                    name = Convert.ToString(s["Player"] != DBNull.Value ? s["Player"] : ""),
                    credit = Convert.ToDouble(s["Credits"] != DBNull.Value ? s["Credits"] : 0.0),
                    type = Convert.ToString(s["Type"] != DBNull.Value ? s["Type"] : ""),
                    point = Convert.ToDouble(s["Points"] != DBNull.Value ? s["Points"] : 0.0),
                    team = Convert.ToString(s["Team"] != DBNull.Value ? s["Team"] : ""),
                    isPlaying = Convert.ToBoolean((s["Playing"] != DBNull.Value ? s["Playing"] : "").ToString() == "y" ? true : false),
                    priority=Convert.ToInt16((s["Priority"]!=DBNull.Value?s["Priority"]:0))
                }).ToList();

                return dsEmployeeInfoList;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw ex;
            }
            
        }
        public List<Boxes> ReadExcel(string path,string sheetName)
        {
            List<Boxes> objEmployeeInfo = new List<Boxes>();
            try
            {
                OleDbConnection oledbConn = OpenConnection(path);
                if (oledbConn.State == ConnectionState.Open)
                {
                    objEmployeeInfo = ExtractEmployeeExcel(oledbConn,sheetName);
                    oledbConn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                // Error
            }
            return objEmployeeInfo;
        }
    }
}
