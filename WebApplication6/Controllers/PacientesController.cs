using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace WebApplication6.Controllers
{
    public class PacientesController : Controller
    {
        // GET: Pacientes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult create(EPaciente pac)
        {

            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_Mantenimiento_paciente", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Historia", 0);
                    cmd.Parameters.AddWithValue("@ApePat", pac.ApePat);
                    cmd.Parameters.AddWithValue("@ApeMat", pac.ApeMat);
                    cmd.Parameters.AddWithValue("@NomPac", pac.NomPac);
                    cmd.Parameters.AddWithValue("@FecNac", pac.FecNac);
                    cmd.Parameters.AddWithValue("@NumDoc", pac.NumDoc);
                    cmd.Parameters.AddWithValue("@estado", 1);
                    cmd.Parameters.AddWithValue("@evento", 1);
                    try
                    {
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        return RedirectToAction("ListaPacientes");
                    }
                    catch (Exception ex)
                    {
                        return View(pac);
                    }
                    finally { cnn.Close(); }
                }
            }

        }


        public List<EPaciente> ListaPaciente()
        {
            List<EPaciente> lista = new List<EPaciente>(); 
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ListaPacientes", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                   
                    while (dr.Read()) {
                        EPaciente pac = new EPaciente();
                        pac.Historia = Convert.ToInt32( dr["Historia"]);
                        pac.ApePat = dr["ApePat"].ToString();
                        pac.ApeMat = dr["ApeMat"].ToString();
                        pac.NomPac = dr["NomPac"].ToString(); 
                        pac.FecNac =Convert.ToDateTime(dr["FecNac"]);
                        pac.estado = bool.Parse(dr["EstPac"].ToString());
                        lista.Add(pac); 
                    } 
                }
            }
            return lista; 
        }

        public ActionResult ListaPacientes()
        {
           ViewBag.paciente  = ListaPaciente();
            return View();

        }
    }
}