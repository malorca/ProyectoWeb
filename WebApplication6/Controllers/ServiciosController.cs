using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace WebApplication6.Controllers
{
    public class ServiciosController : Controller
    {
        // GET: Servicios
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaServicios()
        {

            return View(getListaServicios());
        }
        public List<EServicios> getListaServicios()
        {

            List<EServicios> lista = new List<EServicios>();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_Lista_Servicios_Prueba", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        EServicios serv = new EServicios();
                        serv.codServicio = dr["CodServ"].ToString();
                        serv.nomServicio = dr["NomServ"].ToString();
                        serv.CodEspec = dr["CodEspec"].ToString();
                        serv.CodEmp = dr["CodEmp"].ToString();
                        serv.codSed = dr["CodSede"].ToString();
                        serv.Estado = bool.Parse(dr["EstServ"].ToString());
                        
                        lista.Add(serv);
                    }

                }
            }
            return lista;
        }
        public ActionResult create()
        {
            ViewBag.ListaEspecialidad = new SelectList(getListaEspecialidades(), "CodEspec", "NomEspec");
            ViewBag.ListaEmpresa = new SelectList(getListaEmpresa(), "CodEmp", "RazonEmp"); 
            ViewBag.ListaSede = new SelectList(getListaSede(), "codSed", "nomSede");
            ViewBag.ListaTipoTarifa = new SelectList(getListaTipoTarifa(), "CodTipTar", "DescTipTar");
            return View();
        }

        [HttpPost]
        public ActionResult create(EServicios serv, string evento, string borrar = null)
        {
            string codser = "";

            ViewBag.ListaEspecialidad = new SelectList(getListaEspecialidades(), "CodEspec", "NomEspec", serv.CodEspec);
            ViewBag.ListaEmpresa = new SelectList(getListaEmpresa(), "CodEmp", "RazonEmp");
            ViewBag.ListaSede = new SelectList(getListaSede(), "codSed", "nomSede");
            ViewBag.ListaTipoTarifa = new SelectList(getListaTipoTarifa(), "CodTipTar", "DescTipTar");

            if (!string.IsNullOrWhiteSpace(borrar))
            {
                string[] splite = borrar.Split(',');
                evento = splite[0];
                serv.codTipTar = splite[1];
            }

            if (evento == "1")
            {
                var session = (List<EServicios>)Session["servicios"];
                if (session == null)
                {
                    var objServicios = new List<EServicios>();
                    var objMservicio = new EServicios();
                    objMservicio.codTipTar = serv.codTipTar;
                    objMservicio.descTipTar = getListaTipoTarifa().Where(x => x.codTipTar == serv.codTipTar).FirstOrDefault().descTipTar;
                    objMservicio.porcentaje = serv.porcentaje;
                    objServicios.Add(objMservicio);
                    Session["servicios"] = objServicios;
                    ViewBag.session = objServicios;
                    ViewBag.model = "1";
                    return View(serv);
                }
                else
                {
                    var existe = session.Any(x => x.codTipTar == serv.codTipTar);
                    if (existe == true)
                    {
                        //si encontro
                        var modelServicio = session.Where(x => x.codTipTar == serv.codTipTar).FirstOrDefault();
                        session.Remove(modelServicio);
                        Session["servicios"] = session;
                        var objMservicio1 = new EServicios();
                        objMservicio1.codTipTar = serv.codTipTar;
                        objMservicio1.descTipTar = getListaTipoTarifa().Where(x => x.codTipTar == serv.codTipTar).FirstOrDefault().descTipTar;
                        objMservicio1.porcentaje = serv.porcentaje;
                        session.Add(objMservicio1);
                        Session["servicios"] = session;
                        ViewBag.session = session;
                        ViewBag.model = "1";
                        return View(serv);
                    }
                    else
                    {
                        //no encontro
                        var objMservicio = new EServicios();
                        objMservicio.codTipTar = serv.codTipTar;
                        objMservicio.descTipTar = getListaTipoTarifa().Where(x => x.codTipTar == serv.codTipTar).FirstOrDefault().descTipTar;
                        objMservicio.porcentaje = serv.porcentaje;
                        session.Add(objMservicio);
                        Session["servicios"] = session;
                        ViewBag.session = session;
                        ViewBag.model = "1";
                        return View(serv);
                    }
                }

            }
            else if (evento == "2")
            {

                var session = (List<EServicios>)Session["servicios"];
                var modelServicio = session.Where(x => x.codTipTar == serv.codTipTar).FirstOrDefault();
                session.Remove(modelServicio);
                Session["servicios"] = session;
                ViewBag.session = session;
                ViewBag.model = "1";
                return View(serv);
            }

            int resultado = 0;

            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Mantenimiento_Servicios", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CODSERV", 0);
                    cmd.Parameters.AddWithValue("@NOMSERV", serv.nomServicio);
                    cmd.Parameters.AddWithValue("@CODESPEC", serv.CodEspec);
                    cmd.Parameters.AddWithValue("@CODEMP", serv.CodEmp);
                    cmd.Parameters.AddWithValue("@CODSEDE", serv.codSed);
                    cmd.Parameters.AddWithValue("@ESTADO", 1);
                    cmd.Parameters.AddWithValue("@evento", 1);
                    try
                    {
                        cnn.Open();
                        codser = cmd.ExecuteScalar().ToString();
                        var session1 = (List<EServicios>)Session["servicios"];
                        foreach (var item in session1)
                        {

                            using (SqlCommand cmd1 = new SqlCommand("Usp_Mantenimiento_ServiciosTipoTarifa_Prueba", cnn))
                            {
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@CODSERV", codser);
                                cmd1.Parameters.AddWithValue("@CODTIPTAR", item.codTipTar);
                                cmd1.Parameters.AddWithValue("@PORCENTAJE", item.porcentaje);
                                cmd1.Parameters.AddWithValue("@evento", 1);
                                try
                                {
                                    resultado = cmd1.ExecuteNonQuery();

                                }
                                catch (Exception ex)
                                {
                                    return View(serv);
                                }

                            }
                        }
                        if (resultado != 0)
                        {
                            return RedirectToAction("ListaServicios");
                        }
                    }
                    catch (Exception ex)
                    {
                        return View(serv);
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

            return RedirectToAction("ListaServicios");



        }

        public ActionResult ListaEspecialidades()
        {
            return View(getListaEspecialidades());
        }
        public List<EEspecialidad> getListaEspecialidades()
        {
            List<EEspecialidad> lista = new List<EEspecialidad>();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_Lista_Especialidades_Prueba", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        EEspecialidad esp = new EEspecialidad();
                        esp.codEspec = dr["CodEspec"].ToString();
                        esp.nomEspec = dr["NomEspec"].ToString();

                        lista.Add(esp);
                    }
                }
            }
            return lista;
        }
        public ActionResult ListaEmpresa()
        {
            return View(getListaEmpresa());
        }
        public List<EEmpresa> getListaEmpresa()
        {
            List<EEmpresa> lista = new List<EEmpresa>();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_Lista_Empresa_Tercero_Prueba", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        EEmpresa emp = new EEmpresa();
                        emp.codEmp = dr["CodEmp"].ToString();
                        emp.razonEmp = dr["RazonEmp"].ToString();
                        lista.Add(emp);
                    }
                }
            }
            return lista;
        }
        public ActionResult ListaSede()
        {
            return View(getListaSede());
        }
        public List<ESede> getListaSede()
        {
            List<ESede> lista = new List<ESede>();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_Lista_Sedes_Prueba", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ESede sed = new ESede();
                        sed.codSed = dr["CodSede"].ToString();
                        sed.nomSede = dr["NomSede"].ToString();
                        lista.Add(sed);
                    }
                }
            }
            return lista;
        }
        public List<ETipoTarifa> getListaTipoTarifa()
        {
            List<ETipoTarifa> lista = new List<ETipoTarifa>();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_Listado_TipoTarifa_Prueba", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ETipoTarifa tt = new ETipoTarifa();
                        tt.codTipTar = dr["CodTipTar"].ToString();
                        tt.descTipTar = dr["DescTipTar"].ToString();
                        lista.Add(tt);
                    }
                }
            }
            return lista;
        }

        public List<EServicioTipoTarifa> getListaServiTipoTarifa(string codser)
        {
            List<EServicioTipoTarifa> lista = new List<EServicioTipoTarifa>();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Usp_ListaTipo_Tarifa", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CODSERV", codser);
                    cnn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        EServicioTipoTarifa tt = new EServicioTipoTarifa();
                        tt.codTipTar = dr["CodTipTar"].ToString();
                        tt.Descripcion = dr["DescTipTar"].ToString();
                        tt.porcentaje = (Decimal)dr["PORCENTAJE"];
                        lista.Add(tt);
                    }
                }
            }
            return lista;
        }


        public JsonResult getlista()
        {
            var lista = getListaTipoTarifa();
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult edit(string id)
        {
            ViewBag.ListaEspecialidad = new SelectList(getListaEspecialidades(), "CodEspec", "NomEspec");
            ViewBag.ListaEmpresa = new SelectList(getListaEmpresa(), "CodEmp", "RazonEmp");
            ViewBag.ListaSede = new SelectList(getListaSede(), "codSed", "nomSede");
            var listaServicio = getListaServicios().Where(x => x.codServicio == id).FirstOrDefault();
            ViewBag.listaservicioTipoTarifa = getListaServiTipoTarifa(id).ToList();
            return View(listaServicio);

        }
        [HttpPost]
        public ActionResult edit(EServicios ser)
        {
            ViewBag.ListaEspecialidad = new SelectList(getListaEspecialidades(), "CodEspec", "NomEspec");
            ViewBag.ListaEmpresa = new SelectList(getListaEmpresa(), "CodEmp", "RazonEmp");
            ViewBag.ListaSede = new SelectList(getListaSede(), "codSed", "nomSede");

            ViewBag.listaservicioTipoTarifa = getListaServiTipoTarifa(ser.codServicio).ToList();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Mantenimiento_Servicios", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CODSERV", ser.codServicio);
                    cmd.Parameters.AddWithValue("@NOMSERV", ser.nomServicio);
                    cmd.Parameters.AddWithValue("@CODESPEC", ser.CodEspec);
                    cmd.Parameters.AddWithValue("@CODEMP", ser.CodEmp);
                    cmd.Parameters.AddWithValue("@CODSEDE", ser.codSed);
                    cmd.Parameters.AddWithValue("@ESTADO", ser.Estado);
                    cmd.Parameters.AddWithValue("@evento", 2);
                    try
                    {
                        cnn.Open();
                        cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        cnn.Close();
                    }
                    return RedirectToAction("ListaServicios");
                }
            }
        }
        public ActionResult eliminar(string id)
        {
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Mantenimiento_Servicios",cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CODSERV", id);
                    cmd.Parameters.AddWithValue("@NOMSERV", "");
                    cmd.Parameters.AddWithValue("@CODESPEC", "");
                    cmd.Parameters.AddWithValue("@CODEMP", "");
                    cmd.Parameters.AddWithValue("@CODSEDE","");
                    cmd.Parameters.AddWithValue("@ESTADO", 0);
                    cmd.Parameters.AddWithValue("@evento", 3);

                    try
                    {
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        cnn.Close();
                    }
                    return RedirectToAction("ListaServicios");
                }
            }
        }
        public ActionResult activar(string id)
        {
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn1"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_Mantenimiento_Servicios",cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CODSERV", id);
                    cmd.Parameters.AddWithValue("@NOMSERV","");
                    cmd.Parameters.AddWithValue("@CODESPEC","");
                    cmd.Parameters.AddWithValue("@CODEMP","");
                    cmd.Parameters.AddWithValue("@CODSEDE","");
                    cmd.Parameters.AddWithValue("@ESTADO",1);
                    cmd.Parameters.AddWithValue("@evento", 4);
                    try
                    {
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        cnn.Close();
                    }
                    return RedirectToAction("ListaServicios");
                }
            }

        }
        
    }
}
