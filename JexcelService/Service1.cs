using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;

namespace JexcelService
{
    public partial class Service1 : ServiceBase
    {
        // name space(using System.Timers;)  
        Timer timer = new Timer();
        string koneksi = ConfigurationManager.AppSettings["SqlConnection"];
        string BudgetfileDB = ConfigurationManager.AppSettings["BudgetDokumentTable"];
        string BudgetDB = ConfigurationManager.AppSettings["BudgetTable"];
        string BudgetDBTemp = ConfigurationManager.AppSettings["BudgetTableTemp"];

        public class VWBudgeting
        {
            public int Id { get; set; }
            public Guid PengadaanId { get; set; }
            public string Branch { get; set; }
            public string Department { get; set; }
            public string Description { get; set; }
            public string COA { get; set; }
            public string NoCOA { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public Nullable<decimal> BudgetAmount { get; set; }
            public Nullable<decimal> BudgetUsage { get; set; }
            public Nullable<decimal> BudgetLeft { get; set; }
            public Nullable<decimal> BudgetReserved { get; set; }
            public Nullable<decimal> BudgetLeftTotal { get; set; }
            public Nullable<decimal> Inputbudget { get; set; }
            public Nullable<int> Version { get; set; }
            public Nullable<decimal> TotalInput { get; set; }
            public string BudgetType { get; set; }
        }

        //string excelFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\File\\Book1.xls";
        //Initialize Service on Designer
        public Service1()
        {
            InitializeComponent();
        }

        //When Start Services Do
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            WriteToFile("");
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 60000; //number in milisecinds   120000 = 2 minutes || 300000 = 5 minutes || 420000 = 7 minutes
            timer.Enabled = true;
            
            try
            {
                using (var conn = new SqlConnection(koneksi))
                {
                    conn.Open();
                    WriteToFile("Connection established");
                    conn.Close();
                }

            }
            catch (SqlException ex)
            {
                WriteToFile("Error on start " + ex.Message);
                WriteToFile(ex.StackTrace);
                throw ex;
            }
        }

        //When Stop Services Do
        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
            WriteToFile("");
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        //Logic Start From Here 
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);
            
            using (SqlConnection cnn = new SqlConnection(koneksi))
            {
                try
                {
                    cnn.Open();
                    SqlCommand command;
                    SqlDataReader dataReader;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string sql = "select * from " + BudgetfileDB + " where [Status]=1";
                    //WriteToFile("Service get sql string " + sql);
                    command = new SqlCommand(sql, cnn);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        string Id;
                        //Guid ProcessId;
                        string FilePath;
                        int Status;
                        int Version;
                        string Year;
                        string Jenis;
                        Id = dataReader[11].ToString();
                        Year = dataReader[14].ToString();
                        Jenis = dataReader[15].ToString();
                        //Year = Convert.ToInt32(dataReader[14]);
                        //Jenis = Convert.ToInt32(dataReader[15]);
                        //WriteToFile("Service get Id " + Id);
                        Version = Convert.ToInt32(dataReader[13]);
                        //WriteToFile("Service get Version " + Version);
                        string konStringExcel;
                        //WriteToFile("Service is read result id " + Id);
                        FilePath = dataReader[1].ToString();
                        //WriteToFile("Service is read result path " + FilePath);
                        Status = Convert.ToInt32(dataReader[12]);
                        //WriteToFile("Service is read result status " + Status);

                        if (dataReader[13] == DBNull.Value)
                        {
                            Version = '1';
                            //WriteToFile("Service read result empty Version and now " + Version);
                        }
                        else
                        {
                            WriteToFile("Service read result exist Version is " + Version);
                            //For add version
                            //Version = Version + Convert.ToInt32(1);
                            //WriteToFile("Service is read result exist Version and now2 " + Version);
                        }

                        if (!File.Exists(FilePath))
                        {
                            WriteToFile("Service check there are no file in the " + FilePath);
                            //WriteToFile("");
                        }
                        else
                        {
                            WriteToFile("Service start logic ");
                            string extension = Path.GetExtension(FilePath);
                            switch (extension)
                            {

                                case ".xls": //Excel 97-03
                                    konStringExcel = @"provider=microsoft.jet.oledb.4.0;data source='" + FilePath + "';extended properties=" + "\"excel 8.0;hdr=yes;\"";
                                    WriteToFile("Service is read extension .xls");
                                    ImportDataFromExcel(konStringExcel, Id, FilePath, Status, Version, Year, Jenis);
                                    break;
                                case ".xlsx": //Excel 07 or higher
                                              //konStringExcel = @"provider=Microsoft.ACE.OLEDB.12.0;data source=" + FilePath + ";extended properties=" + "\"excel 12.0;hdr=yes;\"";
                                              //konStringExcel = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + FilePath + "';Extended Properties=\"Excel 12.0;HDR=YES;\"";
                                    konStringExcel = "Provider=Microsoft.ACE.OLEDB.14.0;Data Source='" + FilePath + "';Extended Properties=\"Excel 14.0 Xml;HDR=YES;\"";
                                    WriteToFile("Service is read extension .xlsx");
                                    ImportDataFromExcel(konStringExcel, Id, FilePath, Status, Version, Year, Jenis);
                                    break;
                            }
                        }
                        if (cnn != null) cnn.Close();
                    }
                }
                catch (SqlException ex)
                {
                    WriteToFile("Service fail to initiate " + ex.Message);
                    WriteToFile(ex.StackTrace);
                }
            }

            WriteToFile("Service check and there is no update");
            WriteToFile("");

        }

        //Logic for excel to DB
        public void ImportDataFromExcel(string konStringExcel, string Id, string FilePath, int Status, int Version, string Year, string Jenis)
        {
            try
            {
                WriteToFile("Service start to run function " + DateTime.Now + " And Version now is" + Version);

                using (SqlConnection cnnProcessed = new SqlConnection(koneksi))
                {
                    try
                    {
                        cnnProcessed.Open();
                        SqlCommand commandProcessed;
                        SqlDataAdapter adapterProcessed = new SqlDataAdapter();
                        string sqlProcessed = "UPDATE " + BudgetfileDB + " SET Status = '2' where [ProcessId]='" + Id + "'";
                        commandProcessed = new SqlCommand(sqlProcessed, cnnProcessed);
                        adapterProcessed.InsertCommand = new SqlCommand(sqlProcessed, cnnProcessed);
                        adapterProcessed.InsertCommand.ExecuteNonQuery();
                        commandProcessed.Dispose();
                        cnnProcessed.Close();
                        WriteToFile("Service start to run function and status for this ProcessId file [ProcessId]=" + Id + " is 2 (Processed) " + DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("Service fail to update status Dokumen Budget to 2 " + ex.Message);
                        WriteToFile(ex.StackTrace);
                    }
                }

                //int OldVersion = 0;
                //SqlConnection cnnOldVersion = new SqlConnection(koneksi);
                //cnnOldVersion.Open();
                //SqlCommand commandOldVersion;
                //SqlDataReader dataReaderOldVersion;
                //SqlDataAdapter adapterOldVersion = new SqlDataAdapter();
                //string sqlOldVersion = "select TOP(1) * from " + BudgetDB + " where [Year]='" + Year + "' and [Jenis]='" + Jenis + "' order by Version";
                //WriteToFile("Service Get Last Version " + sqlOldVersion);
                //commandOldVersion = new SqlCommand(sqlOldVersion, cnnOldVersion);
                //dataReaderOldVersion = commandOldVersion.ExecuteReader();
                //while (dataReaderOldVersion.Read())
                //{
                //    OldVersion = Convert.ToInt32(dataReaderOldVersion[11]);
                //}
                //WriteToFile("Service is Read Last Version " + OldVersion);
                //cnnOldVersion.Close();

                string conString = string.Format(konStringExcel, FilePath);
                //WriteToFile("Service get constring " + conString);
                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    try
                    {
                        excel_con.Open();
                        WriteToFile("Service make connection " + excel_con);
                        string sheet1 = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();
                        DataTable dtExcelData = new DataTable();

                        dtExcelData.Columns.AddRange(new DataColumn[12] {

                         new DataColumn("No", typeof(int)),
                         new DataColumn("Branch", typeof(string)),
                         new DataColumn("Departement", typeof(string)),
                         new DataColumn("Description", typeof(string)),
                         new DataColumn("COA Eproc", typeof(string)),
                         new DataColumn("Month", typeof(string)),
                         new DataColumn("Budget Amount", typeof(decimal)),
                         new DataColumn("Budget Usage", typeof(decimal)),
                         new DataColumn("Budget Left", typeof(decimal)),
                         new DataColumn("Year", typeof(string)),
                         new DataColumn("Version", typeof(int)),
                         new DataColumn("Jenis", typeof(string))
                        });

                        using (OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * FROM [" + sheet1 + "]", excel_con))
                        {
                            oda.Fill(dtExcelData);
                            for (int i = dtExcelData.Rows.Count - 1; i >= 0; i--)
                            {
                                dtExcelData.Rows[i]["Year"] = Year;
                                dtExcelData.Rows[i]["Version"] = Version;
                                dtExcelData.Rows[i]["Jenis"] = Jenis;
                            }
                        }
                        int fail = 0;
                        for (int i = dtExcelData.Rows.Count - 1; i >= 0; i--)
                        {
                            if (dtExcelData.Rows[i]["Branch"] == DBNull.Value || dtExcelData.Rows[i]["Departement"] == DBNull.Value || dtExcelData.Rows[i]["Description"] == DBNull.Value || dtExcelData.Rows[i]["COA Eproc"] == DBNull.Value || dtExcelData.Rows[i]["Month"] == DBNull.Value || dtExcelData.Rows[i]["Budget Amount"] == DBNull.Value || dtExcelData.Rows[i]["Budget Usage"] == DBNull.Value || dtExcelData.Rows[i]["Budget Left"] == DBNull.Value)
                            //if (dtExcelData.Rows[i]["Branch"] == DBNull.Value || dtExcelData.Rows[i]["Department"] == DBNull.Value || dtExcelData.Rows[i]["Description"] == DBNull.Value || dtExcelData.Rows[i]["COA Eproc"] == DBNull.Value || dtExcelData.Rows[i]["Month"] == DBNull.Value || dtExcelData.Rows[i]["Budget Amount"] == DBNull.Value)
                            {
                                dtExcelData.Rows[i].Delete();
                                fail = fail + Convert.ToInt32(1);
                            }
                        }
                        int allrows = dtExcelData.Rows.Count;
                        WriteToFile("Service count rows " + allrows);
                        WriteToFile("Service count is not null rows " + (dtExcelData.Rows.Count - fail));
                        WriteToFile("Service count is null rows " + fail);
                        
                        //int count = 0;
                        //SqlConnection cnnCheck = new SqlConnection(koneksi);
                        //cnnCheck.Open();
                        //SqlCommand commandCheck;
                        //SqlDataReader dataReadercheck;
                        //SqlDataAdapter adaptercheck = new SqlDataAdapter();
                        //string sqlcheck = "select * from " + BudgetDB + " where [Version]='"+ Version + "'";
                        //WriteToFile("Service get sql string to check" + sqlcheck);
                        //commandCheck = new SqlCommand(sqlcheck, cnnCheck);
                        //dataReadercheck = commandCheck.ExecuteReader();
                        //if (dataReadercheck.HasRows)
                        //{
                        //    while (dataReadercheck.Read())
                        //    {
                        //        count++; // <------------- here
                        //    }
                        //}
                        //WriteToFile("Service check imported data is " + count);
                        //cnnCheck.Close();
                        //if ((allrows-fail) != count)
                        //{
                        //    WriteToFile("Service check that the data not all imported ");
                        //}

                        using (SqlConnection con = new SqlConnection(koneksi))
                        {
                            con.Open();
                            using (SqlTransaction trx = con.BeginTransaction())
                            {
                                try
                                {
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, trx))
                                    {
                                        sqlBulkCopy.DestinationTableName = BudgetDBTemp;
                                        sqlBulkCopy.ColumnMappings.Add("Branch", "Branch");
                                        sqlBulkCopy.ColumnMappings.Add("Departement", "Department");
                                        sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                                        sqlBulkCopy.ColumnMappings.Add("COA Eproc", "COA");
                                        sqlBulkCopy.ColumnMappings.Add("Month", "Month");
                                        sqlBulkCopy.ColumnMappings.Add("Budget Amount", "BudgetAmount");
                                        sqlBulkCopy.ColumnMappings.Add("Budget Usage", "BudgetUsage");
                                        sqlBulkCopy.ColumnMappings.Add("Budget Left", "BudgetLeft");
                                        sqlBulkCopy.ColumnMappings.Add("Year", "Year");
                                        sqlBulkCopy.ColumnMappings.Add("Version", "Version");
                                        sqlBulkCopy.ColumnMappings.Add("Jenis", "Jenis");
                                        sqlBulkCopy.WriteToServer(dtExcelData);
                                        trx.Commit();       
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteToFile("Service running import from excel and failed " + ex.Message);
                                    WriteToFile(ex.StackTrace);
                                    trx.Rollback();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("Service running excel reader and failed " + ex.Message);
                        WriteToFile(ex.StackTrace);
                    }
                }

                using (SqlConnection cnnSuccess = new SqlConnection(koneksi))
                {
                    try
                    {
                        cnnSuccess.Open();
                        SqlCommand commandSuccess;
                        SqlDataAdapter adapterSuccess = new SqlDataAdapter();
                        string sqlSuccess = "UPDATE " + BudgetfileDB + " SET Status = '4' where [ProcessId]='" + Id + "'";
                        commandSuccess = new SqlCommand(sqlSuccess, cnnSuccess);
                        adapterSuccess.InsertCommand = new SqlCommand(sqlSuccess, cnnSuccess);
                        adapterSuccess.InsertCommand.ExecuteNonQuery();
                        commandSuccess.Dispose();
                        cnnSuccess.Close();
                        WriteToFile("Service success to run function and status for this ProcessId file [ProcessId]=" + Id + " is 4 (Success) " + DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("Service fail to update status Dokumen Budget to 4 " + ex.Message);
                        WriteToFile(ex.StackTrace);
                    }
                }

                //To delete all data of the old version and only left the new version of data on database
                //int OldVersion = Version - Convert.ToInt32(1);
                //using (SqlConnection cnnDeleteExist = new SqlConnection(koneksi))
                //{
                //    try
                //    {
                //        cnnDeleteExist.Open();
                //        SqlCommand commandDeleteExist;
                //        SqlDataAdapter adapterDeleteExist = new SqlDataAdapter();
                //        string sqlDeleteExist = "DELETE FROM " + BudgetDB + " where [Year]='" + Year + "' and [Jenis]='" + Jenis + "' and [Version]='" + OldVersion + "' ";
                //        //WriteToFile("Service get string " + sqlDeleteExist);
                //        commandDeleteExist = new SqlCommand(sqlDeleteExist, cnnDeleteExist);
                //        adapterDeleteExist.InsertCommand = new SqlCommand(sqlDeleteExist, cnnDeleteExist);
                //        adapterDeleteExist.InsertCommand.ExecuteNonQuery();
                //        commandDeleteExist.Dispose();
                //        cnnDeleteExist.Close();
                //        WriteToFile("Service Deleted Data Exist on [Year] " + Year + " and [Jenis] " + Jenis + " at " + DateTime.Now);
                //    }
                //    catch (Exception ex)
                //    {
                //        WriteToFile("Service fail to delete data on version "+ OldVersion + " , Type "+ Jenis + " , Budget Year "+Year+" " + ex.Message);
                //        WriteToFile(ex.StackTrace);
                //    }
                //}
                using (SqlConnection cnMigrate = new SqlConnection(koneksi))
                {
                    try
                    {
                        SqlCommand cmdMigrate = new SqlCommand();
                        List<VWBudgeting> data = new List<VWBudgeting>();
                        cmdMigrate.Connection = cnMigrate;
                        cmdMigrate.Connection.Open();
                        cmdMigrate.CommandText = "SELECT * FROM " + BudgetDBTemp + " WHERE Version = '" + Version + "'";
                        SqlDataReader dr = cmdMigrate.ExecuteReader();
                        while (dr.Read())
                        {
                            var details = new VWBudgeting();
                            details.Id = Convert.ToInt32(dr.GetValue(0));
                            details.Branch = dr.GetValue(1).ToString();
                            details.Department = dr.GetValue(2).ToString();
                            details.Description = dr.GetValue(3).ToString();
                            details.COA = dr.GetValue(4).ToString();
                            details.Month = dr.GetValue(5).ToString();
                            details.Year = dr.GetValue(9).ToString();
                            details.BudgetAmount = (Convert.IsDBNull(dr["BudgetAmount"])) ? 0 : dr.GetDecimal(dr.GetOrdinal("BudgetAmount"));
                            details.BudgetUsage = (Convert.IsDBNull(dr["BudgetUsage"])) ? 0 : dr.GetDecimal(dr.GetOrdinal("BudgetUsage"));
                            details.BudgetLeft = (Convert.IsDBNull(dr["BudgetLeft"])) ? 0 : dr.GetDecimal(dr.GetOrdinal("BudgetLeft"));
                            details.BudgetReserved = (Convert.IsDBNull(dr["BudgetReserved"])) ? 0 : dr.GetDecimal(dr.GetOrdinal("BudgetReserved"));
                            details.Version = Convert.ToInt32(dr.GetValue(11));
                            details.BudgetType = dr.GetValue(12).ToString();
                            data.Add(details);
                        }
                        foreach (VWBudgeting i in data)
                        {
                            using (SqlConnection cnLook = new SqlConnection(koneksi))
                            {
                                SqlCommand cmdLook = new SqlCommand();
                                cmdLook.Connection = cnLook;
                                cmdLook.Connection.Open();
                                cmdLook.CommandText = "SELECT * FROM " + BudgetDB + " WHERE Branch = '" + i.Branch + "' and Department = '" + i.Department + "' and COA = '" + i.COA + "' and Year = '" + i.Year + "' and Month = '" + i.Month + "' and Jenis = '" + i.BudgetType + "' ";
                                SqlDataReader Reader = cmdLook.ExecuteReader();
                                if (Reader.Read())
                                {
                                    int IdBudgetDB = Convert.ToInt32(Reader[0]);
                                    decimal BudgetAmountNew = Reader.GetDecimal(dr.GetOrdinal("BudgetAmount"));
                                    using (SqlConnection cnnEdit = new SqlConnection(koneksi))
                                    {
                                        try
                                        {
                                            cnnEdit.Open();
                                            SqlCommand commandEdit;
                                            SqlDataAdapter adapterEdit = new SqlDataAdapter();
                                            string sqlEdit = "UPDATE " + BudgetDB + " SET BudgetAmount = '" + BudgetAmountNew + "' WHERE Id = '" + IdBudgetDB + "'";
                                            commandEdit = new SqlCommand(sqlEdit, cnnEdit);
                                            adapterEdit.InsertCommand = new SqlCommand(sqlEdit, cnnEdit);
                                            adapterEdit.InsertCommand.ExecuteNonQuery();
                                            commandEdit.Dispose();
                                            cnnEdit.Close();
                                            WriteToFile("Service success Edit BudgetData");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteToFile("Service fail to Edit BudgetData " + ex.Message);
                                            WriteToFile(ex.StackTrace);
                                        }
                                    }
                                }
                                else
                                {
                                    using (SqlConnection cnnAdd = new SqlConnection(koneksi))
                                    {
                                        try
                                        {
                                            cnnAdd.Open();
                                            SqlCommand commandEdit;
                                            SqlDataAdapter adapterAdd = new SqlDataAdapter();
                                            string sqlAdd = "INSERT INTO " + BudgetDB + "" +
                                                     "( [Branch] ,[Department] ,[Description] ,[COA] ,[Month] ,[BudgetAmount] ,[BudgetUsage] ,[BudgetLeft] ,[Year] ,[BudgetReserved] ,[Jenis]) VALUES( " +
                                                     "'" + i.Branch + "'," +
                                                     "'" + i.Department + "'," +
                                                     "'" + i.Description + "'," +
                                                     "'" + i.COA + "'," +
                                                     "'" + i.Month + "'," +
                                                     "'" + i.BudgetAmount + "'," +
                                                     "'" + i.BudgetUsage + "'," +
                                                     "'" + i.BudgetLeft + "'," +
                                                     "'" + i.Year + "'," +
                                                     "'" + i.BudgetReserved + "'," +
                                                     "'" + i.BudgetType + "')";
                                            commandEdit = new SqlCommand(sqlAdd, cnnAdd);
                                            adapterAdd.InsertCommand = new SqlCommand(sqlAdd, cnnAdd);
                                            adapterAdd.InsertCommand.ExecuteNonQuery();
                                            commandEdit.Dispose();
                                            cnnAdd.Close();
                                            WriteToFile("Service success Add BudgetData");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteToFile("Service fail to Add BudgetData " + ex.Message);
                                            WriteToFile(ex.StackTrace);
                                        }
                                    }
                                }
                            }
                        }
                        cmdMigrate.Connection.Close();
                        WriteToFile("Service Success to migrate data");
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("Service fail to migrate data " + ex.Message);
                        WriteToFile(ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToFile("Service failed to run " + ex.StackTrace);
                WriteToFile(ex.StackTrace);
                using (SqlConnection cnnFailed = new SqlConnection(koneksi))
                {
                    try
                    {
                        cnnFailed.Open();
                        SqlCommand command;
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        string sqlFailed = "UPDATE " + BudgetfileDB + " SET Status = '3' where [ProcessId]='" + Id + "'";
                        command = new SqlCommand(sqlFailed, cnnFailed);
                        adapter.InsertCommand = new SqlCommand(sqlFailed, cnnFailed);
                        adapter.InsertCommand.ExecuteNonQuery();
                        command.Dispose();
                        cnnFailed.Close();

                        WriteToFile("Service failed to run function and status for this ProcessId file [ProcessId]=" + Id + " is 3 (Failed) " + DateTime.Now);
                        WriteToFile("");
                    }
                    catch (Exception e)
                    {
                        WriteToFile("Service fail to update status Dokumen Budget to 3 " + e.Message);
                        WriteToFile(e.StackTrace);
                    }
                }
            }
        }
    }
}

