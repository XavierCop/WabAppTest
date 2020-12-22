using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Text;
using System.Web.Script.Services;
using System.Reflection;
using lambertcs;

namespace WebApplication1
{
    //to Update
    /// <summary>
    /// WebService V1.4.2
    /// last update 08/18/2020
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    [System.ComponentModel.ToolboxItem(false)]
    // Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public void getLastUpdate()
        {
            List<Objet> listDate = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("select FORMAT(MAX([DateUpdate]), 'dd-MM-yyyy') as DateUpdate from dbo.RefObjects;", con);
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        string date = rd["DateUpdate"].ToString();
                        obj.DateUpdate = date != "" ? DateTime.Parse(date) : DateTime.Parse("01/01/0001");
                        obj.DateUpdateString = date != "" ? DateTime.Parse(date).ToString("dd-MM-yyyy") : DateTime.Parse("01/01/0001").ToString("dd-MM-yyyy");
                        listDate.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(listDate));
            }
        }


        [WebMethod]
        public void getValueFilter(String Project_name, String listFilter)
        {
            List<Project> listProject = new List<Project>();

            List<Filter> listofFilter = new List<Filter>();

            List<Error> listerror = new List<Error>();
            int cpt = 0;
            int i = 0;
            int j = 0;

            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connstring))
            {

                try
                {
                    SqlCommand cmd;
                    SqlDataReader rd;
                    var sqlstring = "";
                    if ((Project_name == "") || (Project_name == "All") || (Project_name == "All,"))
                    {

                        cmd = new SqlCommand("select DISTINCT [Name] from dbo.Project order by [Name] ASC;", con);
                        cmd.Connection = con;
                        con.Open();
                        rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {

                            Project_name += "," + rd["Name"].ToString().Replace("]", "").Replace("[", "").Replace("\"", "");

                            Project project = new Project();
                            project.Name = rd["Name"].ToString() != "" ? rd["Name"].ToString() : "";
                            listProject.Add(project);
                        }
                        con.Close();
                        if (Project_name[0].ToString() == ",")
                        {
                            Project_name = Project_name.Substring(1);
                        }
                    }


                    for (i = 0; i < listFilter.Split(',').Length; i++)
                    {

                        cpt = 0;
                        if ((Project_name.Split(',').Length > 0) && (Project_name.Split(',').ToString() != null))
                        {

                            switch (listFilter.Split(',')[i])
                            {
                                case "Status":
                                    sqlstring = "select distinct [" + listFilter.Split(',')[i] + "] from dbo.StatusActifPlanner where [Main_Project] In (";
                                    break;
                                case "Title":
                                    sqlstring = "select distinct [" + listFilter.Split(',')[i] + "] from dbo.StatusActifPlanner where [Main_Project] In (";
                                    break;
                                default:
                                    sqlstring = "select distinct [" + listFilter.Split(',')[i] + "] from dbo.RefObjects inner join [ObjectProject] on RefObjects.Object_type = [ObjectProject].Object_type and RefObjects.Code_Object = [ObjectProject].Code_Object inner join Project on [ObjectProject].[ProjectId] = Project.[Id] where Project.[Name] In (";
                                    break;
                            }

                            for (j = 0; j < Project_name.Split(',').Length; j++)
                            {
                                if (Project_name.Split(',').ToString() != "")
                                {
                                    cpt++;
                                    if (cpt == Project_name.Split(',').Length)
                                    {
                                        sqlstring += "'" + Project_name.Split(',')[j].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                                    }
                                    else
                                    {
                                        sqlstring += "'" + Project_name.Split(',')[j].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                                    }
                                }
                            }

                            sqlstring += ") order by " + listFilter.Split(',')[i] + " ASC;";
                        }
                        else
                        {
                            switch (listFilter.Split(',')[i])
                            {
                                case "Status":
                                    sqlstring = "select distinct [" + listFilter.Split(',')[i] + "] from dbo.StatusActifPlanner order by [" + listFilter.Split(',')[i] + "] ASC;";
                                    break;
                                case "Title":
                                    sqlstring = "select distinct [" + listFilter.Split(',')[i] + "] from dbo.StatusActifPlanner order by [" + listFilter.Split(',')[i] + "] ASC;";
                                    break;
                                default:
                                    sqlstring = "select distinct [" + listFilter.Split(',')[i] + "] from dbo.RefObjects order by [" + listFilter.Split(',')[i] + "] ASC;";
                                    break;
                            }

                        }
                        cmd = new SqlCommand(sqlstring, con);

                        cmd.Connection = con;
                        con.Open();
                        rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            Filter Filter = new Filter();
                            Filter.Key = listFilter.Split(',')[i].ToString();
                            Filter.Value = rd[listFilter.Split(',')[i]].ToString() != "" ? rd[listFilter.Split(',')[i]].ToString() : "";
                            listofFilter.Add(Filter);
                        }
                        con.Close();

                    }


                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                var data = new { listProject = listProject, listofFilter = listofFilter };
                Context.Response.Write(js.Serialize(data));


            }
        }


        [WebMethod]
        public void getGROUP1(String Project_name)
        {
            List<Objet> listGroup1 = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cpt = 0;
                        var sqlstring = "select distinct [Groupment1] from dbo.RefObjects where [Project_name] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cpt++;
                            if (cpt == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Groupment1] ASC;";
                        cmd = new SqlCommand(sqlstring, con);
                    }
                    else
                    {
                        cmd = new SqlCommand("select distinct [Groupment1] from dbo.RefObjects order by [Groupment1] ASC;", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        obj.Group_NRO = rd["Groupment1"].ToString() != "" ? rd["Groupment1"].ToString() : "";
                        listGroup1.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listGroup1));

            }
        }


        [WebMethod]
        public void getGROUP2(String Project_name)
        {
            List<Objet> listGroup2 = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cptgrp2 = 0;
                        var sqlstring = "select distinct [Groupment2] from dbo.RefObjects where [Project_name] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cptgrp2++;
                            if (cptgrp2 == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Groupment2] ASC;";
                        cmd = new SqlCommand(sqlstring, con);
                    }
                    else
                    {
                        cmd = new SqlCommand("select distinct [Groupment2] from dbo.RefObjects order by [Groupment2] ASC;", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        obj.Group_PM = rd["Groupment2"].ToString() != "" ? rd["Groupment2"].ToString() : "";
                        listGroup2.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listGroup2));

            }
        }


        [WebMethod]
        public void getAllCommune(String Project_name)
        {
            List<Commune> listTown = new List<Commune>();
            List<Error> listerror = new List<Error>();
            int cpt = 0;
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cptCommune = 0;
                        var sqlstring = "select distinct [Town] from dbo.RefObjects where [Project_name] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cptCommune++;
                            if (cptCommune == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Town] ASC;";
                        cmd = new SqlCommand(sqlstring, con);
                    }
                    else
                    {
                        cmd = new SqlCommand("select DISTINCT [Town] from dbo.RefObjects order by [Town] ASC;", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        cpt += 1;
                        Commune commune = new Commune();
                        commune.Id = cpt;
                        commune.NameTown = rd["Town"].ToString() != "" ? rd["Town"].ToString() : "";
                        listTown.Add(commune);
                    }
                    con.Close();


                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listTown));
            }
        }


        [WebMethod]
        public void getStatusAuditTab(String Project_name)
        {
            List<Objet> listStatusAudit = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cptStatAudit = 0;
                        var sqlstring = "select distinct [Status_Audit] from dbo.RefObjects where [Project_name] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cptStatAudit++;
                            if (cptStatAudit == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Status_Audit] ASC;";
                        cmd = new SqlCommand(sqlstring, con);
                    }
                    else
                    {
                        cmd = new SqlCommand("select distinct [Status_Audit] from dbo.RefObjects order by [Status_Audit] ASC;", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        obj.Status_Audit = rd["Status_Audit"].ToString() != "" ? rd["Status_Audit"].ToString() : "";
                        listStatusAudit.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listStatusAudit));
            }
        }


        [WebMethod]
        public void getAllEtatReprises(String Project_name)
        {
            List<Objet> listStatusRecovery = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cptEtatRep = 0;
                        var sqlstring = "select distinct [Status_Recovery] from dbo.RefObjects where [Project_name] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cptEtatRep++;
                            if (cptEtatRep == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Status_Recovery] ASC;";
                        cmd = new SqlCommand(sqlstring, con);
                    }
                    else
                    {
                        cmd = new SqlCommand("select distinct [Status_Recovery] from dbo.RefObjects order by [Status_Recovery] ASC;", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        obj.Status_Recovery = rd["Status_Recovery"].ToString() != "" ? rd["Status_Recovery"].ToString() : "";
                        listStatusRecovery.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listStatusRecovery));
            }
        }


        [WebMethod]
        public void getAllStatutDeployment(String Project_name)
        {
            List<Objet> listStatusDeployment = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cptStatdep = 0;
                        var sqlstring = "select distinct [Status] from dbo.StatusActifPlanner where [Main_Project] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cptStatdep++;
                            if (cptStatdep == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Status] ASC;";
                        cmd = new SqlCommand(sqlstring, con);

                    }
                    else
                    {
                        cmd = new SqlCommand("select distinct [Status] from dbo.StatusActifPlanner order by [Status] ASC;", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        obj.Status_Deployment = rd["Status"].ToString() != "" ? rd["Status"].ToString() : "";
                        listStatusDeployment.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listStatusDeployment));
            }
        }


        [WebMethod]
        public void getAllTitleStatut(String Project_name)
        {
            List<Objet> listStatusDeployment = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cpttitle = 0;
                        var sqlstring = "select distinct [Title] from dbo.StatusActifPlanner where [Main_Project] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cpttitle++;
                            if (cpttitle == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Title] ASC;";
                        cmd = new SqlCommand(sqlstring, con);

                    }
                    else
                    {
                        cmd = new SqlCommand("select distinct [Title] from dbo.StatusActifPlanner order by [Title] ASC;", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        obj.Status_Deployment = rd["Title"].ToString() != "" ? rd["Title"].ToString() : "";
                        listStatusDeployment.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listStatusDeployment));
            }
        }


        [WebMethod]
        public void getAllTypes(String Project_name)
        {
            List<Objet> listTypes = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    SqlCommand cmd;
                    if (Project_name.Split(',').Length > 0)
                    {
                        int cptalltype = 0;
                        var sqlstring = "select distinct [Object_type], [Object_Global_Type] from dbo.RefObjects where [Project_name] In (";

                        for (int i = 0; i < Project_name.Split(',').Length; i++)
                        {
                            cptalltype++;
                            if (cptalltype == Project_name.Split(',').Length)
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "'";
                            }
                            else
                            {
                                sqlstring += "'" + Project_name.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "") + "' ,";
                            }
                        }

                        sqlstring += ") order by [Object_type] ASC;";
                        cmd = new SqlCommand(sqlstring, con);

                    }
                    else
                    {
                        cmd = new SqlCommand("select distinct [Object_type], [Object_Global_Type] from dbo.RefObjects order by [Object_type];", con);
                    }

                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Objet obj = new Objet();
                        obj.Object_type = rd["Object_type"].ToString() != "" ? rd["Object_type"].ToString() : "";
                        obj.Object_Global_Type = rd["Object_Global_Type"].ToString() != "" ? rd["Object_Global_Type"].ToString() : "";
                        listTypes.Add(obj);
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listTypes));
            }
        }


        [WebMethod]
        public void getObjectFilter(String Datafilter, String DatafilterEtat, string listproject)
        {

            List<Objet> listNodes = new List<Objet>();
            List<Objet> listCables = new List<Objet>();
            List<Objet> listZones = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    var sqlstring = generatesql(Datafilter, DatafilterEtat, listproject);
                    ReadData(listNodes, listCables, listZones, sqlstring, con);
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                var data = new { listNodes = listNodes, listCables = listCables, listZones = listZones };
                Context.Response.Write(js.Serialize(data));
            }
        }


        [WebMethod]
        public void getObjectByPostalCode(string PostalCode)
        {
            List<Objet> listNodes = new List<Objet>();
            List<Objet> listCables = new List<Objet>();
            List<Objet> listZones = new List<Objet>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    var sqlstring = "select * from dbo.RefObjects where [Postal_Code]='" + PostalCode + "';";
                    ReadData(listNodes, listCables, listZones, sqlstring, con);
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                Context.Response.Write(js.Serialize(listNodes));
                Context.Response.Write(js.Serialize(listCables));
                Context.Response.Write(js.Serialize(listZones));
            }
        }


        [WebMethod]
        public void getObjectDetailsOld(string Code_Object, string Object_type, string Project_Name)
        {
            List<Objet> listObject = new List<Objet>();
            List<StatusActifPlanner> listStatusActifplanner = new List<StatusActifPlanner>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    var sqlstring = "SELECT TOP 1 * " +
                        "FROM RefObjects " +
                        "left JOIN dbo.DataSource_AuditTab_StatusObject ON dbo.RefObjects.Code_Object = dbo.DataSource_AuditTab_StatusObject.Code_Object " +
                            "AND dbo.RefObjects.Object_type = dbo.DataSource_AuditTab_StatusObject.Type_Object " +
                        "left JOIN dbo.Zone ON dbo.RefObjects.Code_Object = dbo.Zone.ZN_Code " +
                            "AND dbo.RefObjects.Object_type = dbo.Zone.ZN_Type " +
                        "left JOIN dbo.Node ON dbo.RefObjects.Code_Object = dbo.Node.ND_Code " +
                            "AND dbo.RefObjects.Object_type = dbo.Node.ND_Type " +
                        "left JOIN dbo.Cable ON dbo.RefObjects.Code_Object = dbo.Cable.CB_Code " +
                            "AND dbo.RefObjects.Object_type = dbo.Cable.CB_Type " +
                        "where dbo.RefObjects.Code_Object='" + Code_Object + "' " +
                            "And dbo.RefObjects.Object_type='" + Object_type + "';";

                    ReadDetailsObject(listObject, sqlstring, con);

                    sqlstring = "SELECT * from StatusActifPlanner " +
                            "where dbo.StatusActifPlanner.Object_Code='" + Code_Object + "' " +
                            "And dbo.StatusActifPlanner.Object_type='" + Object_type + "' ";


                    sqlstring += " order by [Order] asc;";

                    ReadDetailsStatusActifPlanner(listStatusActifplanner, sqlstring, con);
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                var data = new { listObject = listObject, listStatusActifplanner = listStatusActifplanner };
                Context.Response.Write(js.Serialize(data));

            }
        }


        [WebMethod]
        public void getObjectDetails(string Code_Object, string Object_type)
        {

            List<Project> listProject = new List<Project>();
            List<Objet> listObject = new List<Objet>();
            List<StatusActifPlanner> listStatusActifplanner = new List<StatusActifPlanner>();
            List<Error> listerror = new List<Error>();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    string sqlstring = "SELECT Name from Project where Id in(select ProjectId from ObjectProject where dbo.ObjectProject.Code_Object='" + Code_Object + "' " +
                             "And dbo.ObjectProject.Object_type='" + Object_type + "');";

                    ReadProjectName(listProject, sqlstring, con);

                    sqlstring = "SELECT TOP 1 * " +
                         "FROM RefObjects " +
                         "left JOIN dbo.DataSource_AuditTab_StatusObject ON dbo.RefObjects.Code_Object = dbo.DataSource_AuditTab_StatusObject.Code_Object " +
                             "AND dbo.RefObjects.Object_type = dbo.DataSource_AuditTab_StatusObject.Type_Object " +
                         "left JOIN dbo.Zone ON dbo.RefObjects.Code_Object = dbo.Zone.ZN_Code " +
                             "AND dbo.RefObjects.Object_type = dbo.Zone.ZN_Type " +
                         "left JOIN dbo.Node ON dbo.RefObjects.Code_Object = dbo.Node.ND_Code " +
                             "AND dbo.RefObjects.Object_type = dbo.Node.ND_Type " +
                         "left JOIN dbo.Cable ON dbo.RefObjects.Code_Object = dbo.Cable.CB_Code " +
                             "AND dbo.RefObjects.Object_type = dbo.Cable.CB_Type " +
                         "where dbo.RefObjects.Code_Object='" + Code_Object + "' " +
                             "And dbo.RefObjects.Object_type='" + Object_type + "';";

                    ReadDetailsObject(listObject, sqlstring, con);

                    sqlstring = "SELECT * from StatusActifPlanner " +
                            "where dbo.StatusActifPlanner.Object_Code='" + Code_Object + "' " +
                            "And dbo.StatusActifPlanner.Object_type='" + Object_type + "' ";


                    sqlstring += " order by [Order] asc;";

                    ReadDetailsStatusActifPlanner(listStatusActifplanner, sqlstring, con);
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = "Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                var data = new { listProject = listProject, listObject = listObject, listStatusActifplanner = listStatusActifplanner };
                Context.Response.Write(js.Serialize(data));

            }
        }


        public void ReadData(List<Objet> listNodes, List<Objet> listCables, List<Objet> listZones, String sqlstring, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(sqlstring, con);
            cmd.Connection = con;
            con.Open();
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                Objet obj = new Objet();
                if (rd["Object_Global_Type"].ToString().Trim().ToUpper() == "NODE")
                {
                    obj.Code_Object = rd["Code_Object"].ToString() != "" ? rd["Code_Object"].ToString() : "";
                    obj.Object_type = rd["Object_type"].ToString() != "" ? rd["Object_type"].ToString() : "";

                    obj.Object_Global_Type = rd["Object_Global_Type"].ToString() != "" ? rd["Object_Global_Type"].ToString() : "";
                    string txt = ConvertLambertToWKT(rd["Geom_WKT"].ToString(), rd["Object_Global_Type"].ToString());
                    //obj.Geom_WKT = rd["Geom_WKT"].ToString() != "" ? rd["Geom_WKT"].ToString() : "";
                    obj.Geom_WKT = txt;
                    obj.Status_Audit = rd["Status_Audit"].ToString() != "" ? rd["Status_Audit"].ToString() : "";
                    obj.Status_Recovery = rd["Status_Recovery"].ToString() != "" ? rd["Status_Recovery"].ToString() : "";
                    obj.Status_Deployment = rd["Status"].ToString() != "" ? rd["Status"].ToString() : "";
                }
                else
                {
                    obj.Code_Object = rd["Code_Object"].ToString() != "" ? rd["Code_Object"].ToString() : "";
                    obj.Object_type = rd["Object_type"].ToString() != "" ? rd["Object_type"].ToString() : "";

                    obj.Object_Global_Type = rd["Object_Global_Type"].ToString() != "" ? rd["Object_Global_Type"].ToString() : "";
                    string txt = ConvertLambertToWKT(rd["Geom_WKT"].ToString(), rd["Object_Global_Type"].ToString());
                    //obj.Geom_WKT = rd["Geom_WKT"].ToString() != "" ? rd["Geom_WKT"].ToString() : "";
                    obj.Geom_WKT = txt;
                    obj.Status_Audit = rd["Status_Audit"].ToString() != "" ? rd["Status_Audit"].ToString() : "";
                    obj.Status_Recovery = rd["Status_Recovery"].ToString() != "" ? rd["Status_Recovery"].ToString() : "";
                    obj.Status_Deployment = rd["Status"].ToString() != "" ? rd["Status"].ToString() : "";
                }
            if (rd["Object_Global_Type"].ToString().ToUpper() == "NODE")
                {
                    listNodes.Add(obj);
                }
                if (rd["Object_Global_Type"].ToString().ToUpper() == "CABLE")
                {
                    listCables.Add(obj);
                }
                if (rd["Object_Global_Type"].ToString().ToUpper() == "ZONE")
                {
                    listZones.Add(obj);
                }

            }
            con.Close();
            return;
        }

        public void ReadProjectName(List<Project> listProject, String sqlstring, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(sqlstring, con);

            cmd.Connection = con;
            con.Open();
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                Project Prj = new Project();
                Prj.Name = rd["Name"].ToString() != "" ? rd["Name"].ToString() : ""; ;
                listProject.Add(Prj);
            }
            con.Close();
            return;
        }

        //To Update
        public void ReadDetailsObject(List<Objet> listObject, String sqlstring, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(sqlstring, con);
            //SqlCommand cmd = new SqlCommand("select * from dbo.Object;", con);
            cmd.Connection = con;
            con.Open();
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                Objet obj = new Objet();
                obj.Code_Object = rd["Code_Object"].ToString() != "" ? rd["Code_Object"].ToString() : "";
                obj.Object_type = rd["Object_type"].ToString() != "" ? rd["Object_type"].ToString() : "";
                obj.Name_Object = rd["Name_Object"].ToString() != "" ? rd["Name_Object"].ToString() : "";
                obj.Adresse = rd["Adress"].ToString() != "" ? rd["Adress"].ToString() : "";
                obj.Commune = rd["Town"].ToString() != "" ? rd["Town"].ToString() : "";
                obj.CP = rd["Postal_Code"].ToString() != "" ? rd["Postal_Code"].ToString() : "";
                obj.Geom_WKT = rd["Geom_WKT"].ToString();
                obj.Object_Global_Type = rd["Object_Global_Type"].ToString() != "" ? rd["Object_Global_Type"].ToString() : "";
                obj.Status_Audit = rd["Status_Audit"].ToString() != "" ? rd["Status_Audit"].ToString() : "";
                obj.Status_Recovery = rd["Status_Recovery"].ToString() != "" ? rd["Status_Recovery"].ToString() : "";
                obj.Status_Deployment = rd["Status_Deployment"].ToString() != "" ? rd["Status_Deployment"].ToString() : "";
                obj.Nb_Reprise = string.IsNullOrEmpty(rd["Nb de reprises"].ToString()) ? 0 : Convert.ToInt32(rd["Nb de reprises"].ToString());
                obj.Nb_Reprise_NB = string.IsNullOrEmpty(rd["Nb de reprise non bloquante"].ToString()) ? 0 : Convert.ToInt32(rd["Nb de reprise non bloquante"].ToString());
                obj.Nb_Reprise_B = string.IsNullOrEmpty(rd["Nb de reprise bloquante"].ToString()) ? 0 : Convert.ToInt32(rd["Nb de reprise bloquante"].ToString());
                obj.Nb_Reprise_Realise_NB = string.IsNullOrEmpty(rd["Nb de reprise réalisé non bloquante"].ToString()) ? 0 : Convert.ToInt32(rd["Nb de reprise réalisé non bloquante"].ToString());
                obj.Nb_Reprise_Realise_B = string.IsNullOrEmpty(rd["Nb de reprise réalisé bloquante"].ToString()) ? 0 : Convert.ToInt32(rd["Nb de reprise réalisé bloquante"].ToString());
                obj.Nb_Reprise_Valide_NB = string.IsNullOrEmpty(rd["Nb de reprise validé non bloquante"].ToString()) ? 0 : Convert.ToInt32(rd["Nb de reprise validé non bloquante"].ToString());
                obj.Nb_Reprise_Valide_B = string.IsNullOrEmpty(rd["Nb de reprise validé bloquante"].ToString()) ? 0 : Convert.ToInt32(rd["Nb de reprise validé bloquante"].ToString());
                obj.IdFiche = rd["Fiche"].ToString() != "" ? rd["Fiche"].ToString() : "";
                obj.Moe = rd["Société"].ToString() != "" ? rd["Société"].ToString() : "";
                obj.Capacity_max = rd["CB_CapacityMax"].ToString() != "" ? rd["CB_CapacityMax"].ToString() : "";
                //obj.Type_building = rd["TypeBuilding"].ToString() != "" ? rd["TypeBuilding"].ToString() : "";
                obj.End_Date = rd["End_Date"].ToString() != "" ? rd["End_Date"].ToString() : "";
                obj.Audit_Date = rd["Date"].ToString() != "" ? rd["Date"].ToString() : "";
                obj.Nb_Outlet = string.IsNullOrEmpty(rd["ND_Nb_Outlet"].ToString()) ? 0 : Convert.ToInt32(rd["ND_Nb_Outlet"].ToString());
                obj.Progress = rd["Progression"].ToString() != "" ? rd["Progression"].ToString() : "0";
                obj.Group_NRO = rd["Groupment1"].ToString() != "" ? rd["Groupment1"].ToString() : "";
                obj.Group_PM = rd["Groupment2"].ToString() != "" ? rd["Groupment2"].ToString() : "";
                obj.lot = rd["lot"].ToString() != "" ? rd["lot"].ToString() : "";
                obj.Moe = rd["MOE"].ToString() != "" ? rd["MOE"].ToString() : "";
                switch (obj.Object_Global_Type.ToUpper())
                {
                    case "NODE":
                        obj.Type_Support = rd["ND_Type_Support"].ToString() != "" ? rd["ND_Type_Support"].ToString() : "";
                        obj.Property_Support = rd["ND_Property_Support"].ToString() != "" ? rd["ND_Property_Support"].ToString() : "";
                        break;
                    case "CABLE":
                        obj.Type_Support = rd["CB_Type_Support"].ToString() != "" ? rd["CB_Type_Support"].ToString() : "";
                        obj.Property_Support = rd["CB_Property_Support"].ToString() != "" ? rd["CB_Property_Support"].ToString() : "";
                        break;
                    default:
                        obj.Type_Support = "";
                        obj.Property_Support = "";
                        break;
                }

                obj.ND_Code1 = rd["CB_ND_Code1"].ToString() != "" ? rd["CB_ND_Code1"].ToString() : "";
                obj.ND_Code2 = rd["CB_ND_Code2"].ToString() != "" ? rd["CB_ND_Code2"].ToString() : "";
                obj.Type_implantation = rd["CB_Type_implantation"].ToString() != "" ? rd["CB_Type_implantation"].ToString() : "";
                obj.cb_lenght = string.IsNullOrEmpty(rd["CB_Length"].ToString()) ? 0 : Convert.ToInt32(rd["CB_Length"].ToString());
                listObject.Add(obj);

            }
            con.Close();
            return;
        }


        public void ReadDetailsStatusActifPlanner(List<StatusActifPlanner> listStatusActifplanner, String sqlstring, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(sqlstring, con);
            //SqlCommand cmd = new SqlCommand("select * from dbo.Object;", con);
            cmd.Connection = con;
            con.Open();
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                StatusActifPlanner StatusAP = new StatusActifPlanner();
                StatusAP.Title_status = rd["Title"].ToString() != "" ? rd["Title"].ToString() : "";
                StatusAP.Status = rd["Status"].ToString() != "" ? rd["Status"].ToString() : "";
                StatusAP.End_Date = rd["End_date"].ToString() != "" ? rd["End_date"].ToString() : "";
                listStatusActifplanner.Add(StatusAP);
            }
            con.Close();
            return;
        }


        public string generatesql(string Datafilter, string DatafilterEtat, string listproject)
        {
            int cptvalue = 0;
            JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> dictObject = (Dictionary<string, object>)jsSerializer.DeserializeObject(Datafilter);
            Dictionary<string, object> dictEtat = (Dictionary<string, object>)jsSerializer.DeserializeObject(DatafilterEtat);

            string sqlString = "select distinct(dbo.RefObjects.Code_Object), Object_Global_Type, dbo.RefObjects.Object_type, Geom_WKT, Status_Recovery, dbo.StatusActifPlanner.Status, Status_Audit from dbo.RefObjects ";

            sqlString += "inner join ObjectProject on RefObjects.Object_type = ObjectProject.Object_type and RefObjects.Code_Object = ObjectProject.Code_Object inner join Project on Project.[Id] = ObjectProject.[ProjectId] ";

            if (DatafilterEtat != "")
            {
                sqlString += "inner join dbo.StatusActifPlanner " +
                    "on dbo.RefObjects.Code_Object = dbo.StatusActifPlanner.Object_Code " +
                    "and dbo.RefObjects.Object_type = dbo.StatusActifPlanner.Object_type ";

                foreach (KeyValuePair<string, object> item in dictEtat)
                {
                    if (!item.Value.Equals("All"))
                    {
                        sqlString += " and [" + item.Key + "] = '" + item.Value + "' ";
                    }
                }
                sqlString += "and [Object_Global_Type] IN ('Node', 'Cable', 'Zone') ";
            }
            else
            {
                sqlString += "left join dbo.StatusActifPlanner " +
                             "on dbo.RefObjects.Code_Object = dbo.StatusActifPlanner.Object_Code " +
                             "and dbo.RefObjects.Object_type = dbo.StatusActifPlanner.Object_type " +
                              "and dbo.StatusActifPlanner.[Order] = 0 ";

                sqlString += "where [Object_Global_Type] IN ('Node', 'Cable', 'Zone') ";
            }

            foreach (KeyValuePair<string, object> item in dictObject)
            {
                if (!item.Value.Equals("All"))
                {
                    sqlString += " and [" + item.Key + "] = '" + item.Value + "' ";
                }

            }

            if (listproject.Split(',')[0] != "All")
            {
                sqlString += "and Project.[Name] In (";

                for (int i = 0; i < listproject.Split(',').Length; i++)
                {
                    cptvalue++;
                    if (cptvalue == listproject.Split(',').Length)
                    {
                        sqlString += "'" + listproject.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "").Trim() + "'";
                    }
                    else
                    {
                        sqlString += "'" + listproject.Split(',')[i].Replace("]", "").Replace("[", "").Replace("\"", "").Trim() + "' ,";
                    }
                };

                sqlString += ") ";
            }
            sqlString += ";";

            return sqlString;
        }

        //To Add
        [WebMethod]
        public void SearchPosition(string code, string type)
        {
            // Initialisation des variables
            string sql;
            SqlCommand command;
            SqlDataReader dataReader;
            Objet obj = new Objet();
            string output;
            List<Error> listerror = new List<Error>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string connstring = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connstring))
            {
                try
                {
                    // Ecriture de la requête Sql
                    sql = "select * from dbo.RefObjects where code_object = '" + code + "' and object_type = '" + type + "'";
                    // Envoi de la requête Sql
                    command = new SqlCommand(sql, con);

                    con.Open();
                    // Lancement du dataReader
                    dataReader = command.ExecuteReader();

                    // Extraction de la valeur de la position en brut
                    while (dataReader.Read())
                    {
                        obj.Geom_WKT = dataReader["Geom_WKT"].ToString() != "" ? dataReader["Geom_WKT"].ToString() : "";
                    }
                    // Epuration de la position brute
                    string net = obj.Geom_WKT.ToUpper().Replace("MULTI[[", "").Replace("POINT[[", "").Replace("]", "").Replace("LINESTRING[[", "").Replace("[", "").Replace("POLYGONE", "").Replace("POLYGON", "");
                    // Séparation de la latitude et de la longitude
                    string[] netSplit = net.Split(new Char[] { ',' });
                    // Attribution de la latitude et de la longitude à l'objet position
                    obj.Longitude = netSplit[1];
                    obj.Latitude = netSplit[0];
                    // Traduction de l'objet position au format JSON
                    output = js.Serialize(obj);
                    // Fermeture de la connexion
                    con.Close();
                    // Fermeture du dataReader
                    dataReader.Close();
                    // Fermeture de la commande des requêtes Sql
                    command.Dispose();
                    // Ecriture de la position au format JSON
                    Context.Response.Write(output);
                }
                catch (Exception e)
                {
                    Error errorObject = new Error();
                    errorObject.Id = 22;
                    errorObject.Message = (string)"Msg :" + e.Message;
                    listerror.Add(errorObject);
                }
            }

        }


        public String ConvertLambertToWKT(String Geom, String GlobalType)
        {
            Objet obj = new Objet();
            // Initialisation du string retourné 
            string output = "";
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> listeOutput = new List<string>();
            List<string> liste = new List<string>();
            List<Point> listept = new List<Point>();
            List<string> listeType = new List<string>();
            bool multi = false;
            if (Geom != "")
            {              
                if (GlobalType == "NODE")
                {
                    string net = Geom.ToUpper().Replace("POINT", "").Replace("]", "").Replace("[", "");
                    string[] netSplit = net.Split(new Char[] { ',' });
                    int i = 0;
                    while (i < ((netSplit.Length) / 2))
                    {
                        // Conversion du string en double :
                        double longiDoub = Convert.ToDouble(netSplit[(2 * i) + 1].Replace('.', ','));
                        double latiDoub = Convert.ToDouble(netSplit[2 * i].Replace('.', ','));


                        //Conversion WGS84 vers Lambert 93 :
                        Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                        listept.Add(pt);
                        i++;
                        output = "POINT[[" + pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]]";
                    }
                }
                else if (GlobalType == "CABLE")
                {
                    bool multiBool = false;
                    if (Geom.ToUpper().Contains("MULTI"))
                    {
                        multiBool = true;
                    }
                    string baseNet = Geom.ToUpper().Replace("LINESTRING[", "").Replace("MULTI", "");
                    string baseStartNet = baseNet.Replace("[[", "/");
                    string[] baseSplitNet = baseStartNet.Split('/');
                    string linestring = "";
                    if (multiBool) {
                        linestring += "MultiLineString[";
                    }
                    else { 
                    linestring+="LineString[";
                    }
                    if (baseSplitNet.Length > 1) { 
                        int i = 0;
                        while (i < baseSplitNet.Length - 1) {
                            linestring += "[";
                            int a = 0;
                            string startNet = baseSplitNet[i+1].Replace("]", "").Replace("[", "");
                            string[] splitNet = startNet.Split(',');
                            while (a < splitNet.Length / 2)
                            {
                                double longiDoub = Convert.ToDouble(splitNet[(2 * a) + 1].Replace('.', ','));
                                double latiDoub = Convert.ToDouble(splitNet[2 * a].Replace('.', ','));
                                Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                                listept.Add(pt);
                                linestring += "[" + pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]";
                                a ++;
                                if (a < (splitNet.Length / 2))
                                {
                                    linestring += ",";
                                }
                            }
                            linestring += "]";
                            if (i < baseSplitNet.Length-2) {
                                linestring += ",";
                            }
                            i++;
                        }
                        //if (multiBool)
                        //{
                        //    linestring += "]";
                        //}
                    }
                    else
                    {
                        int a = 0;
                        string startNet = baseNet.Replace("]", "").Replace("[", "");
                        string[] splitNet = startNet.Split(',');
                        while (a < splitNet.Length/2)
                        {
                            
                            double longiDoub = Convert.ToDouble(splitNet[(2*a)+1].Replace('.', ','));
                            double latiDoub = Convert.ToDouble(splitNet[2*a].Replace('.', ','));
                            Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                            listept.Add(pt);
                            linestring += "[" + pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]";
                            a ++;
                            if (a < (splitNet.Length/2))
                            {
                                linestring += ",";
                            }
                        }
                    }
                    linestring += "]";
                    //    string net = Geom.ToUpper().Replace("LINESTRING[", "").Replace("],[", "/").Replace("[", "");
                    //    string[] netSplit = net.Split(new Char[] { '/' });                   
                    //    string linestring = "LineString[";
                    //    int a = 0;
                    //    while(a < netSplit.Length)
                    //    {
                    //        if(netSplit.Length > 1) { 
                    //            string[] netSplitReturns = netSplit[a].Split(',');
                    //            int i = 0;
                    //            linestring += "[";
                    //            while (i < (netSplitReturns.Length)/2)
                    //            {
                    //                double longiDoub = Convert.ToDouble(netSplitReturns[(2 * i) + 1].Replace('.', ',').Replace("]", ""));
                    //                double latiDoub = Convert.ToDouble(netSplitReturns[2 * i].Replace('.', ','));
                    //                Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                    //                listept.Add(pt);
                    //                linestring += pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]";
                    //                if (i < ((netSplitReturns.Length) / 2) -1)
                    //                {
                    //                    linestring += ",";
                    //                }
                    //                i++;
                    //            } 
                    //        }
                    //        else
                    //        {
                    //            string[] netSplitReturns = netSplit[a].Split(',');
                    //            int i = 0;
                    //            while (i < (netSplitReturns.Length) / 2)
                    //            {
                    //                double longiDoub = Convert.ToDouble(netSplitReturns[(2 * i) + 1].Replace('.', ',').Replace("]", ""));
                    //                double latiDoub = Convert.ToDouble(netSplitReturns[2 * i].Replace('.', ','));
                    //                Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                    //                listept.Add(pt);
                    //                linestring += pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]";
                    //                if (i < ((netSplitReturns.Length) / 2)-1)
                    //                {
                    //                    linestring += ",";
                    //                }
                    //                i++;
                    //            }
                    //        }
                    //        a++;
                    //    }
                    //    linestring += "]";
                    //string newLinestring = linestring.Replace("],]","]]");
                    output = linestring;
                }                    
                else if (GlobalType == "ZONE")
                {
                    string linestring;
                    bool multiBool = false;
                    if (Geom.ToUpper().Contains("MULTI"))
                    {
                        multiBool = true;
                    }
                    string baseNet = Geom.ToUpper().Replace("POLYGON[", "").Replace("MULTI","");
                    string baseStartNet = baseNet.Replace("[[", "/");
                    string[] baseSplitNet = baseStartNet.Split('/');
                    if (multiBool)
                    {
                        linestring = "MULTIPOLYGON[[";
                    }
                    else
                    {
                        linestring = "POLYGON[";
                    }
                    if (baseSplitNet.Length > 1)
                    {
                        int i = 0;
                        while (i < baseSplitNet.Length - 1)
                        {
                            linestring += "[";
                            int a = 0;
                            string startNet = baseSplitNet[i + 1].Replace("]", "").Replace("[", "");
                            string[] splitNet = startNet.Split(',');
                            
                            while (a < splitNet.Length / 2)
                            {
                                try
                                {
                                    double longiDoub = Convert.ToDouble(splitNet[(2 * a) + 1].Replace('.', ','));
                                    double latiDoub = Convert.ToDouble(splitNet[2 * a].Replace('.', ','));
                                    Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                                    listept.Add(pt);
                                    linestring += "[" + pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]";
                                    a++;
                                    if (a < (splitNet.Length / 2))
                                    {
                                        linestring += ",";
                                    }
                                }
                                catch (Exception e)
                                {
                                    Error errorObject = new Error();
                                    errorObject.Message = (string)"Msg :" + e.Message ;
                                    errorObject.Id = 22;
                                }
                                //double longiDoub = Convert.ToDouble(splitNet[(2 * a) + 1].Replace('.', ','));
                                //double latiDoub = Convert.ToDouble(splitNet[2 * a].Replace('.', ','));
                                //Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                                //listept.Add(pt);
                                //linestring += "[" + pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]";
                                //a ++;
                                //if (a < splitNet.Length - 1)
                                //{
                                //    linestring += ",";
                                //}
                            }
                            linestring += "]";
                            
                            if (i < baseSplitNet.Length - 2)
                            {
                                linestring += ",";
                            }
                            i++;
                        }
                        if (multiBool)
                        {
                            linestring += "]";
                        }
                    }
                    else
                    {
                        linestring += "[";
                        int a = 0;
                        string startNet = baseNet.Replace("]", "").Replace("[", "");
                        string[] splitNet = startNet.Split(',');
                        while (a < splitNet.Length / 2)
                        {

                            double longiDoub = Convert.ToDouble(splitNet[(2 * a) + 1].Replace('.', ','));
                            double latiDoub = Convert.ToDouble(splitNet[2 * a].Replace('.', ','));
                            Point pt = Lambert.convertToWGS84Deg(latiDoub, longiDoub, Zone.Lambert93);
                            listept.Add(pt);
                            linestring += "[" + pt.y.ToString().Replace(',', '.') + "," + pt.x.ToString().Replace(',', '.') + "]";
                            a++;
                            if (a < (splitNet.Length / 2))
                            {
                                linestring += ",";
                            }
                        }
                        linestring += "]";
                    }
                    linestring += "]";
                    output = linestring;
                }
            }
            return output;
        }
    }
}
