using Com.StellmanGreene.CSVReader;
using LukeSkywalker.IPNetwork;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace SRChecker
{
    public partial class CheckerForm : Form
    {

        private DataTable _result = null;
        private List<string> _localIPList = new List<string>();

        public CheckerForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                btnChecker.Enabled = false;
                cmbAction.Enabled = false;
                btnClean.Enabled = false;
                pnlSRFileImport.Enabled = false;

                this.UseWaitCursor = true;

                txtResult.Text = "";
                btnExport.Visible = false;

                if (cmbAction.SelectedIndex == -1)
                {
                    throw new Exception("Please Select Action");
                }
                else if (cmbAction.SelectedItem.ToString() == "SRFileImport")
                {
                    if (txtSRID.Text == string.Empty)
                        throw new Exception("'SR ID' is Required.");

                    if (txtSROwner.Text == string.Empty)
                        throw new Exception("'SR Owner' is Required.");
                }

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtInputFilePath.Text = openFileDialog.FileName;

                    //DataTable table = CSVReader.ReadCSVFile(openFileDialog.FileName, true);
                    DataTable table = GetDataTableFromExcel(openFileDialog.FileName, true);

                    if (table.Rows.Count > 1)
                    {
                        txtResult.Text = txtInputFilePath.Text + " is loaded." + CSVReader.NEWLINE;

                        dtGridView.DataSource = table;
                        dtGridView.Visible = true;

                        Application.DoEvents();

                        checkGivenList(table);
                    }
                    else
                    {
                        throw new Exception("File Has No Data");
                    }
                }
                else
                {
                    throw new Exception("Please Select File");
                }
            }
            catch (Exception exc)
            {
                txtResult.Text = "Error : " + exc.Message;
                txtResult.BackColor = Color.PaleVioletRed;
            }
            finally
            {
                btnClean.Enabled = true;
                this.UseWaitCursor = false;
                prgBar.Value = prgBar.Maximum;
            }
        }

        public DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();

                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    int i = 0;
                    foreach (var cell in wsRow)
                    {
                        i++;
                        if (i > tbl.Columns.Count)
                            break;
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                DataColumn newDataColumn = new DataColumn("SheetName", typeof(System.String));
                newDataColumn.DefaultValue = ws.Name;
                tbl.Columns.Add(newDataColumn);

                return tbl;
            }
        }

        private void checkGivenList(DataTable table)
        {
            int totalCount = 0, failCount = 0;
            decimal processed = 0;

            try
            {
                table.Columns.Add("Result", Type.GetType("System.String"));
                table.Columns.Add("HasError", Type.GetType("System.String"));

                switch (cmbAction.SelectedItem.ToString())
                {
                    case "SubnetCalculator":

                        if (!table.Columns.Contains("SUBNET"))
                            throw new Exception("'SUBNET' column has not found.");

                        if (!table.Columns.Contains("IP"))
                            throw new Exception("'IP' column has not found.");

                        DataTable dtIpRanges = new DataTable("dtIpRanges");
                        DataColumn newDataColumn = null;

                        newDataColumn = new DataColumn("SITE", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("VLAN", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("EXPLANATION", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("NETWORK", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("SUBNET", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("CIDR", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("IP", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("FileName", typeof(System.String));
                        newDataColumn.DefaultValue = openFileDialog.SafeFileName;
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("SheetName", typeof(System.String));
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("CreateUserIP", typeof(System.String));
                        newDataColumn.DefaultValue = this._localIPList.Count > 0 ? this._localIPList[0] : DBNull.Value.ToString();
                        dtIpRanges.Columns.Add(newDataColumn);

                        newDataColumn = new DataColumn("CreateUserTime", typeof(System.DateTime));
                        newDataColumn.DefaultValue = DateTime.Now;
                        dtIpRanges.Columns.Add(newDataColumn);

                        foreach (DataRow dr in table.Rows)
                        {
                            IPNetwork ipNetwork = null;
                            IPNetwork.TryParse(Convert.ToString(dr["IP"]), Convert.ToString(dr["SUBNET"]), out ipNetwork);

                            if (ipNetwork == null)
                                continue;

                            IPRanges ipRanges = new IPRanges(ipNetwork.ToString());

                            foreach (IPAddress ip in ipRanges.GetAllIP())
                            {
                                DataRow drRow2Export = dtIpRanges.NewRow();

                                drRow2Export["SITE"] = dr["SITE"];
                                drRow2Export["VLAN"] = dr["VLAN"];
                                drRow2Export["EXPLANATION"] = dr["EXPLANATION"];
                                drRow2Export["NETWORK"] = dr["IP"];
                                drRow2Export["SUBNET"] = dr["SUBNET"];
                                drRow2Export["CIDR"] = Convert.ToString(ipNetwork.Cidr);
                                drRow2Export["IP"] = Convert.ToString(ip);
                                drRow2Export["SheetName"] = dr["SheetName"];

                                dtIpRanges.Rows.Add(drRow2Export);
                            }
                        }
                        try
                        {
                            SqlConnection IpConnection = new SqlConnection("Persist Security Info=False;Initial Catalog=VodafoneTesting;Application Name=SRChecker;Data Source=92.45.86.60,3495;;MultipleActiveResultSets=True;User=ebysa;Password=Eby342516");
                            IpConnection.Open();

                            if (1 == 0)// First Time Use Only
                            {
                                using (SqlCommand command = new SqlCommand("Truncate Table MercurySubnetIPList", IpConnection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(IpConnection))
                            {
                                bulkCopy.DestinationTableName =
                                    "dbo.MercurySubnetIPList";
                                SqlBulkCopyColumnMapping mapID = null;

                                mapID = new SqlBulkCopyColumnMapping("SITE", "SITE");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("VLAN", "VLAN");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("EXPLANATION", "EXPLANATION");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("NETWORK", "NETWORK");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("SUBNET", "SUBNET");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("IP", "IP");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("CIDR", "CIDR");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("FileName", "FileName");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("SheetName", "SheetName");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("CreateUserIP", "CreateUserIP");
                                bulkCopy.ColumnMappings.Add(mapID);

                                mapID = new SqlBulkCopyColumnMapping("CreateUserTime", "CreateUserTime");
                                bulkCopy.ColumnMappings.Add(mapID);

                                bulkCopy.WriteToServer(dtIpRanges);
                            }
                        }
                        catch (Exception exc)
                        {
                            //throw new Exception("DB Write Error: " + exc.Message);
                        }

                        txtResult.Text = dtIpRanges.Rows.Count.ToString() + " Rows Added To dbo.MercurySubnetIPList Table";
                        txtResult.BackColor = Color.PaleGreen;

                        dtGridView.DataSource = dtIpRanges;
                        this._result = dtIpRanges;

                        btnExport.Visible = true;
                        dtGridView.Visible = true;

                        break;
                    case "SRFileImport":

                        totalCount = 0;
                        failCount = 0;

                        DataColumn newColumn = null;

                        newColumn = new DataColumn("FileName", typeof(System.String));
                        newColumn.DefaultValue = openFileDialog.SafeFileName;
                        table.Columns.Add(newColumn);

                        newColumn = new DataColumn("SRID", typeof(System.String));
                        newColumn.DefaultValue = txtSRID.Text;
                        table.Columns.Add(newColumn);

                        newColumn = new DataColumn("SROwner", typeof(System.String));
                        newColumn.DefaultValue = txtSROwner.Text;
                        table.Columns.Add(newColumn);

                        if (txtSRID.Text == string.Empty)
                            throw new Exception("'SR ID' is Required.");

                        if (txtSROwner.Text == string.Empty)
                            throw new Exception("'SR Owner' is Required.");

                        if (!table.Columns.Contains("Target_System_IP"))
                            throw new Exception("'Target_System_IP' column has not found.");

                        if (!table.Columns.Contains("Source_System_Ip"))
                            throw new Exception("'Source_System_Ip' column has not found.");

                        if (!table.Columns.Contains("Source_System_Name"))
                            throw new Exception("'Source_System_Name' column has not found.");

                        if (!table.Columns.Contains("Source_Customer_Info"))
                            throw new Exception("'Source_Customer_Info' column has not found.");

                        if (!table.Columns.Contains("Target_System_Name"))
                            throw new Exception("'Target_System_Name' column has not found.");

                        if (!table.Columns.Contains("Target_Customer_Info"))
                            throw new Exception("'Target_Customer_Info' column has not found.");

                        if (!table.Columns.Contains("TCP_Port"))
                            throw new Exception("'TCP_Port' column has not found.");

                        if (!table.Columns.Contains("UDP_Port"))
                            throw new Exception("'UDP_Port' column has not found.");

                        SqlConnection connection = new SqlConnection("Persist Security Info=False;Initial Catalog=VodafoneTesting;Application Name=SRChecker;Data Source=92.45.86.60,3495;;MultipleActiveResultSets=True;User=ebysa;Password=Eby342516");
                        connection.Open();

                        using (SqlCommand command = new SqlCommand("DELETE FROM MercuryFWSRDetails WHERE SR_ID = '" + txtSRID.Text + "'", connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName =
                                "dbo.MercuryFWSRDetails";
                            SqlBulkCopyColumnMapping mapID = null;

                            mapID = new SqlBulkCopyColumnMapping("Target_System_IP", "Target_System_IP");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("Source_System_Ip", "Source_System_Ip");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("Source_System_Name", "Source_System_Name");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("Source_Customer_Info", "Source_Customer_Info");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("Target_Customer_Info", "Target_Customer_Info");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("Target_System_Name", "Target_System_Name");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("TCP_Port", "TCP_Port");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("UDP_Port", "UDP_Port");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("SRID", "SR_ID");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("FileName", "FileName");
                            bulkCopy.ColumnMappings.Add(mapID);

                            mapID = new SqlBulkCopyColumnMapping("SROwner", "RequestOwner");
                            bulkCopy.ColumnMappings.Add(mapID);

                            bulkCopy.WriteToServer(table);
                        }

                        txtResult.Text = table.Rows.Count.ToString() + " Rows Added To MercuryFWSRDetails Table";
                        txtResult.BackColor = Color.PaleGreen;

                        dtGridView.DataSource = table;
                        this._result = table;

                        btnExport.Visible = false;
                        dtGridView.Visible = true;

                        connection.Close();

                        break;
                    case "Tracert":
                        {
                            table.Columns.Add("Source_System_Ip_Trace", Type.GetType("System.String"));
                            table.Columns.Add("Target_System_IP_Trace", Type.GetType("System.String"));

                            totalCount = 0;
                            failCount = 0;
                            if (!table.Columns.Contains("Target_System_IP"))
                                throw new Exception("'Target_System_IP' column has not found.");

                            if (!table.Columns.Contains("Source_System_Ip"))
                                throw new Exception("'Source_System_Ip' column has not found.");

                            processed = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                string targetIP = string.Empty, sourceIP = string.Empty;
                                try
                                {

                                    processed++;
                                    prgBar.Value = Convert.ToInt32(((processed / Convert.ToDecimal(table.Rows.Count)) * 100));
                                    Application.DoEvents();

                                    targetIP = Convert.ToString(dr["Target_System_IP"]).Trim();

                                    string[] targetIPList = null;

                                    if (targetIP.Contains("\n"))
                                        targetIPList = targetIP.Split('\n');

                                    if (targetIP.Contains(","))
                                        targetIPList = targetIP.Split(',');

                                    if (targetIPList == null)
                                    {
                                        targetIPList = new string[1];
                                        targetIPList[0] = targetIP;
                                    }

                                    List<string> finalTargetIPList = new List<string>();

                                    foreach (string item in targetIPList)
                                    {
                                        if (item.Contains("-"))
                                        {
                                            string[] itemIPRange = item.Split('.');

                                            string[] itemRangeList = itemIPRange[3].Split('-');
                                            int rangeStart = Convert.ToInt32(itemRangeList[0]);
                                            int rangeEnd = Convert.ToInt32(itemRangeList[1]);

                                            for (int i = rangeStart; i <= rangeEnd; i++)
                                            {
                                                finalTargetIPList.Add(itemIPRange[0] + "." + itemIPRange[1] + "." + itemIPRange[2] + "." + i.ToString());
                                            }

                                        }
                                        else
                                            finalTargetIPList.Add(item);
                                    }
                                    foreach (string targetIPItem in finalTargetIPList)
                                    {
                                        string targetIPToTest = targetIPItem;
                                        if (targetIPToTest.Substring(targetIPToTest.Length - 2, 2) == ".0")
                                            targetIPToTest = targetIPToTest.Substring(0, targetIPToTest.Length - 2) + ".1";
                                        //TODO:Subnet Calculation. For Now Just .1 IP

                                        totalCount++;

                                        foreach (var entry in Tracert(targetIPToTest, 30, 5000))
                                        {
                                            dr["Target_System_IP_Trace"] += CSVReader.NEWLINE + Convert.ToString(entry);

                                            switch (entry.ReplyStatus)
                                            {
                                                case IPStatus.TimedOut:
                                                case IPStatus.TtlExpired:
                                                case IPStatus.TtlReassemblyTimeExceeded:
                                                case IPStatus.TimeExceeded:
                                                    continue;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    sourceIP = Convert.ToString(dr["Source_System_IP"]).Trim();

                                    string[] sourceIPList = null;

                                    if (sourceIP.Contains("\n"))
                                        sourceIPList = sourceIP.Split('\n');

                                    if (sourceIP.Contains(","))
                                        sourceIPList = sourceIP.Split(',');

                                    if (sourceIPList == null)
                                    {
                                        sourceIPList = new string[1];
                                        sourceIPList[0] = sourceIP;
                                    }

                                    List<string> finalSourceIPList = new List<string>();

                                    foreach (string item in sourceIPList)
                                    {
                                        if (item.Contains("-"))
                                        {
                                            string[] itemIPRange = item.Split('.');

                                            string[] itemRangeList = itemIPRange[3].Split('-');
                                            int rangeStart = Convert.ToInt32(itemRangeList[0]);
                                            int rangeEnd = Convert.ToInt32(itemRangeList[1]);

                                            for (int i = rangeStart; i <= rangeEnd; i++)
                                            {
                                                finalSourceIPList.Add(itemIPRange[0] + "." + itemIPRange[1] + "." + itemIPRange[2] + "." + i.ToString());
                                            }

                                        }
                                        else
                                            finalSourceIPList.Add(item);
                                    }
                                    foreach (string sourceIPItem in finalSourceIPList)
                                    {
                                        string sourceIPToTest = sourceIPItem;
                                        if (sourceIPToTest.Substring(sourceIPToTest.Length - 2, 2) == ".0")
                                            sourceIPToTest = sourceIPToTest.Substring(0, sourceIPToTest.Length - 2) + ".1";
                                        //TODO:Subnet Calculation. For Now Just .1 IP

                                        totalCount++;

                                        foreach (var entry in Tracert(sourceIPToTest, 30, 5000))
                                        {
                                            dr["Source_System_IP_Trace"] += CSVReader.NEWLINE + Convert.ToString(entry);

                                            switch (entry.ReplyStatus)
                                            {
                                                case IPStatus.TimedOut:
                                                case IPStatus.TtlExpired:
                                                case IPStatus.TtlReassemblyTimeExceeded:
                                                case IPStatus.TimeExceeded:
                                                    continue;
                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    dr["Result"] = "Trace OK";
                                    dr["HasError"] = "false";
                                }
                                catch (SocketException sExc)
                                {
                                    failCount++;
                                    dr["Result"] += CSVReader.NEWLINE + " Trace Error: Target IP = " + targetIP + ", Source IP = " + sourceIP;
                                    dr["HasError"] = "true";
                                }
                                catch (Exception gExc)
                                {
                                    dr["Result"] += CSVReader.NEWLINE + " Trace Error: Target IP = " + targetIP + ", Source IP = " + sourceIP;
                                    dr["HasError"] = "true";
                                }
                            }

                            txtResult.Text = "Check Completed." + CSVReader.NEWLINE + "Total Valid Target IP : " + totalCount.ToString() + CSVReader.NEWLINE + "Fail Count : " + failCount.ToString() + CSVReader.NEWLINE + "Success Count : " + Convert.ToString(totalCount - failCount) + CSVReader.NEWLINE;
                            if (failCount > 0)
                                txtResult.BackColor = Color.PaleVioletRed;
                            else
                                txtResult.BackColor = Color.PaleGreen;

                            dtGridView.DataSource = table;
                            this._result = table;

                            btnExport.Visible = true;
                            dtGridView.Visible = true;

                            break;
                        }
                    case "Firewall":
                        {
                            totalCount = 0;
                            failCount = 0;
                            if (!table.Columns.Contains("Target_System_IP"))
                                throw new Exception("'Target_System_IP' column has not found.");

                           

                            if (!table.Columns.Contains("TCP_Port"))
                                throw new Exception("'TCP_Port' column has not found.");

                            if (!table.Columns.Contains("Source_System_Ip"))
                                throw new Exception("'Source_System_Ip' column has not found.");

                            processed = 0;
                            foreach (DataRow dr in table.Rows)
                            {
                                processed++;
                                prgBar.Value = Convert.ToInt32(((processed / Convert.ToDecimal(table.Rows.Count)) * 100));
                                Application.DoEvents();

                                if (Convert.ToString(dr["Source_System_Ip"]) != "")
                                {
                                    if (!_localIPList.Contains(Convert.ToString(dr["Source_System_Ip"]).Trim()))
                                    {
                                        txtResult.Text += "Warning: " + Convert.ToString(dr["Source_System_Ip"]) + " Source Ip Is Not In Local IP List." + CSVReader.NEWLINE;
                                        dr["Result"] = "Source Ip Is Not In Local IP List";
                                        dr["HasError"] = "true";

                                        continue;
                                    }
                                }

                                if (Convert.ToString(dr["Target_System_IP"]) == "" || Convert.ToString(dr["TCP_Port"]) == "")
                                {
                                    dr["Result"] = "Invalid Target IP or Port";
                                    dr["HasError"] = "true";
                                }
                                else
                                {
                                    try
                                    {
                                        int test = Convert.ToInt32(Convert.ToString(dr["TCP_Port"]).Replace(",", "").Replace("-", ""));
                                    }
                                    catch
                                    {
                                        throw new Exception("'TCP_Port' must be Integer");
                                    }

                                    List<int> portList = new List<int>();

                                    string[] portRange = Convert.ToString(dr["TCP_Port"]).Split('-');
                                    if (portRange.Length > 1)
                                    {
                                        int start = Convert.ToInt32(portRange[0]);
                                        int end = Convert.ToInt32(portRange[1]);

                                        if (start <= end)
                                        {
                                            while (start <= end)
                                            {
                                                portList.Add(start);
                                                start++;
                                            }
                                        }
                                        else
                                        {
                                            portList.Add(start);
                                            portList.Add(end);
                                        }
                                    }
                                    else
                                    {
                                        string[] portsWithComma = Convert.ToString(dr["TCP_Port"]).Split(',');
                                        if (portsWithComma.Length > 1)
                                        {
                                            foreach (string item in portsWithComma)
                                            {
                                                portList.Add(Convert.ToInt32(item));
                                            }
                                        }
                                        else
                                        {
                                            portList.Add(Convert.ToInt32(dr["TCP_Port"]));
                                        }
                                    }

                                    foreach (int port in portList)
                                    {
                                        totalCount++;

                                        try
                                        {
                                            string targetIP = Convert.ToString(dr["Target_System_IP"]).Trim();
                                            if (targetIP.Substring(targetIP.Length - 2, 2) == ".0")
                                                targetIP = targetIP.Substring(0, targetIP.Length - 2) + ".1";
                                            //TODO:Subnet Calculation. For Now Just .1 IP
                                            TcpClient client = new TcpClient(targetIP, port);
                                            dr["Result"] += CSVReader.NEWLINE + " Success on Port: " + port.ToString();
                                            dr["HasError"] = "false";
                                        }
                                        catch (SocketException sExc)
                                        {
                                            failCount++;
                                            dr["Result"] += CSVReader.NEWLINE + " Error on Port:" + port.ToString();// +" - " + sExc.Message;
                                            dr["HasError"] = "true";
                                        }
                                        catch (Exception gExc)
                                        {
                                            dr["Result"] += CSVReader.NEWLINE + " Error on Port:" + port.ToString();// + " - " +gExc.Message;
                                            dr["HasError"] = "true";
                                        }
                                    }
                                }
                            }

                            txtResult.Text = "Check Completed." + CSVReader.NEWLINE + "Total Valid Target IP/Port : " + totalCount.ToString() + CSVReader.NEWLINE + "Fail Count : " + failCount.ToString() + CSVReader.NEWLINE + "Success Count : " + Convert.ToString(totalCount - failCount) + CSVReader.NEWLINE;
                            if (failCount > 0)
                                txtResult.BackColor = Color.PaleVioletRed;
                            else
                                txtResult.BackColor = Color.PaleGreen;

                            dtGridView.DataSource = table;
                            this._result = table;

                            btnExport.Visible = true;
                            dtGridView.Visible = true;
                        }
                        break;
                    case "DNS":
                        //txtResult.Text = "DNS Not Implemented Yet";

                        if (!table.Columns.Contains("Host"))
                            throw new Exception("'Host' column has not found.");

                        totalCount = 0;
                        failCount = 0;
                        processed = 0;

                        table.Columns.Add("IP", Type.GetType("System.String"));
                        table.Columns.Add("HostName", Type.GetType("System.String"));

                        foreach (DataRow dr in table.Rows)
                        {
                            processed++;
                            prgBar.Value = Convert.ToInt32(((processed / Convert.ToDecimal(table.Rows.Count)) * 100));
                            Application.DoEvents();

                            totalCount++;

                            try
                            {
                                IPHostEntry ipE = Dns.GetHostByName(Convert.ToString(dr["Host"]).Trim());
                                dr["HostName"] = ipE.HostName;

                                //bool pingResult = PingHost(ipE.HostName);

                                IPAddress[] IpA = ipE.AddressList;
                                for (int i = 0; i < IpA.Length; i++)
                                {
                                    dr["IP"] += IpA[i].ToString() + " ";
                                }
                                dr["HasError"] = "false";
                                dr["Result"] = "Ping Result:" + PingHost(ipE.HostName).ToString();
                            }
                            catch (Exception exc)
                            {
                                dr["HostName"] = "No Such Host";
                                dr["HasError"] = "true";
                                dr["Result"] = "Error";
                                failCount++;
                            }
                        }

                        txtResult.Text = "Check Completed." + CSVReader.NEWLINE + "Total Valid Host " + totalCount.ToString() + CSVReader.NEWLINE + "Fail Count : " + failCount.ToString() + CSVReader.NEWLINE + "Success Count : " + Convert.ToString(totalCount - failCount) + CSVReader.NEWLINE;
                        if (failCount > 0)
                            txtResult.BackColor = Color.PaleVioletRed;
                        else
                            txtResult.BackColor = Color.PaleGreen;
                        btnExport.Visible = true;
                        dtGridView.Visible = true;

                        break;
                    default:
                        txtResult.Text = "Invalid Action";
                        break;
                }
            }
            catch (Exception exc)
            {
                txtResult.Text = "Error: " + exc.Message;
                txtResult.BackColor = Color.PaleVioletRed;
            }
        }

        public IPStatus PingHost(string nameOrAddress)
        {
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                return reply.Status;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return IPStatus.Unknown;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //TODO
            //cmbAction.Items.Remove("DNS");
            //cmbAction.SelectedItem = "Firewall";

            //openFileDialog.Filter = "Csv Files|*.csv;*.txt;";

            openFileDialog.Filter = "Excel |*.xlsx";

            string sHostName = Dns.GetHostName();
            IPHostEntry ipE = Dns.GetHostByName(sHostName);
            IPAddress[] IpA = ipE.AddressList;
            txtIPList.Text = ipE.HostName + CSVReader.NEWLINE;
            for (int i = 0; i < IpA.Length; i++)
            {
                _localIPList.Add(IpA[i].ToString());
                txtIPList.Text += IpA[i].ToString() + CSVReader.NEWLINE;
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            txtInputFilePath.Text = "";
            txtResult.Text = "";
            cmbAction.ResetText();
            cmbAction.SelectedIndex = -1;
            btnChecker.Text = "Check Results";
            pnlSRFileImport.Visible = false;
            dtGridView.Visible = false;
            btnExport.Visible = false;
            txtResult.BackColor = DefaultBackColor;
            prgBar.Value = 0;
            btnChecker.Enabled = true;
            cmbAction.Enabled = true;
            txtSROwner.Text = "";
            txtSRID.Text = "";
            pnlSRFileImport.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var dia = new System.Windows.Forms.SaveFileDialog();
            dia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dia.Filter = "Excel Worksheets (*.xlsx)|*.xlsx|xls file (*.xls)|*.xls|All files (*.*)|*.*";
            if (dia.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                var excel = new OfficeOpenXml.ExcelPackage();
                var ws = excel.Workbook.Worksheets.Add("worksheet-name");
                ws.Cells["A1"].LoadFromDataTable(this._result, true, OfficeOpenXml.Table.TableStyles.Light1);
                ws.Cells[ws.Dimension.Address.ToString()].AutoFitColumns();

                using (var file = File.Create(dia.FileName))
                    excel.SaveAs(file);
            }

        }

        /// <summary>
        /// Traces the route which data have to travel through in order to reach an IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address of the destination.</param>
        /// <param name="maxHops">Max hops to be returned.</param>
        public IEnumerable<TracertEntry> Tracert(string ipAddress, int maxHops, int timeout)
        {
            IPAddress address;

            // Ensure that the argument address is valid.
            if (!IPAddress.TryParse(ipAddress, out address))
                throw new ArgumentException(string.Format("{0} is not a valid IP address.", ipAddress));

            // Max hops should be at least one or else there won't be any data to return.
            if (maxHops < 1)
                throw new ArgumentException("Max hops can't be lower than 1.");

            // Ensure that the timeout is not set to 0 or a negative number.
            if (timeout < 1)
                throw new ArgumentException("Timeout value must be higher than 0.");


            Ping ping = new Ping();
            PingOptions pingOptions = new PingOptions(1, true);
            Stopwatch pingReplyTime = new Stopwatch();
            PingReply reply;

            do
            {
                pingReplyTime.Start();
                reply = ping.Send(address, timeout, new byte[] { 0 }, pingOptions);
                pingReplyTime.Stop();

                string hostname = string.Empty;
                if (reply.Address != null)
                {
                    try
                    {
                        hostname = Dns.GetHostByAddress(reply.Address).HostName;    // Retrieve the hostname for the replied address.
                    }
                    catch (SocketException) { /* No host available for that address. */ }
                }

                // Return out TracertEntry object with all the information about the hop.
                yield return new TracertEntry()
                {
                    HopID = pingOptions.Ttl,
                    Address = reply.Address == null ? "N/A" : reply.Address.ToString(),
                    Hostname = hostname,
                    ReplyTime = pingReplyTime.ElapsedMilliseconds,
                    ReplyStatus = reply.Status
                };

                pingOptions.Ttl++;
                pingReplyTime.Reset();
            }
            while (reply.Status != IPStatus.Success && pingOptions.Ttl <= maxHops);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAction.SelectedItem != null && cmbAction.SelectedItem.ToString() == "SRFileImport")
            {
                btnChecker.Text = "Import";
                pnlSRFileImport.Visible = true;
            }
            else
            {
                btnChecker.Text = "Check Results";
                pnlSRFileImport.Visible = false;
            }
        }
    }
}
